using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using R3;
using ZLogger;

namespace JJJ.UseCase.Turn
{
  /// <summary>
  /// ターンを実行するクラス
  /// </summary>
  public class TurnExecutor : ITurnExecutor, IDisposable
  {
    private readonly IRuleSet _ruleSet;
    private readonly GameStateProvider _gameStateProvider;
    private readonly ICompositeHandAnimationPresenter _compositeHandAnimationPresenter;
    private readonly IJudgeInput _judgeInput;
    private readonly ITimerService _timerService;

    private CompositeDisposable _disposables = new CompositeDisposable();
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<TurnExecutor>();

    public TurnExecutor(
      IRuleSet ruleSet,
      GameStateProvider gameStateProvider,
      ICompositeHandAnimationPresenter compositeHandAnimationPresenter,
      IJudgeInput judgeInput,
      ITimerService timerService
    )
    {
      _ruleSet = ruleSet;
      _gameStateProvider = gameStateProvider;
      _compositeHandAnimationPresenter = compositeHandAnimationPresenter;
      _judgeInput = judgeInput;
      _timerService = timerService;
    }

    /// <summary>
    /// ターンを実行する
    /// </summary>
    /// <returns>ターンの結果と手のアニメーション完了を待つUniTaskのタプル</returns>
    public async UniTask<(TurnOutcome, UniTask)> ExecuteTurn(CancellationToken cancellationToken = default)
    {
      _disposables?.Dispose();
      // 外部キャンセルトークンと連結したトークンソースを作成
      _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      if (_disposables == null)
      {
        _disposables = new CompositeDisposable();
      }

      var context = _gameStateProvider.CurrentTurnContext;
      var judgeTimeLimit = _gameStateProvider.JudgeTimeLimit;

      // CPU hands & truth
      var playerHand = _gameStateProvider.PlayerCpuHandStrategy.GetNextCpuHand(context, PersonType.Player);
      var opponentHand = _gameStateProvider.OpponentCpuHandStrategy.GetNextCpuHand(context, PersonType.Opponent);
      var truthResult = _ruleSet.Judge(playerHand, opponentHand, context);

      // Observables
      var playerWinObservable = _judgeInput.PlayerWinObservable;
      var opponentWinObservable = _judgeInput.OpponentWinObservable;
      var drawObservable = _judgeInput.DrawObservable;

      // Hand Animation Presenters
      var playerHandAnimationPresenter = _compositeHandAnimationPresenter.PlayerHandAnimationPresenter;
      var opponentHandAnimationPresenter = _compositeHandAnimationPresenter.OpponentHandAnimationPresenter;

      // 手のアニメーションを再生するUniTask
      if (playerHandAnimationPresenter == null || opponentHandAnimationPresenter == null)
      {
        throw new InvalidOperationException("HandAnimationPresenter is not set.");
      }
      var playerHandPlayTask = playerHandAnimationPresenter.PlayHand(playerHand.Type, playerHand.IsTimeout, cancellationToken);
      var opponentHandPlayTask = opponentHandAnimationPresenter.PlayHand(opponentHand.Type, opponentHand.IsTimeout, cancellationToken);

      // タイマー開始
      var timerObservable = _timerService.CountdownEveryFrame(judgeTimeLimit, _cancellationTokenSource.Token)
        .Subscribe(remaining =>
        {
          _gameStateProvider.JudgeRemainTime.Value = remaining;
        });

      _logger.ZLogDebug($"Waiting Judge... (Limit {judgeTimeLimit.TotalSeconds} seconds)");
      // 一番最初に発生したイベントを待つ
      var claim = await Observable.Merge(
        playerWinObservable != null ? playerWinObservable.Select(_ => PlayerClaim.PlayerWin) : Observable.Empty<PlayerClaim>(),
        opponentWinObservable != null ? opponentWinObservable.Select(_ => PlayerClaim.OpponentWin) : Observable.Empty<PlayerClaim>(),
        drawObservable != null ? drawObservable.Select(_ => PlayerClaim.Draw) : Observable.Empty<PlayerClaim>(),
        _timerService.After(judgeTimeLimit).Select(_ => PlayerClaim.Timeout)
      ).FirstAsync(cancellationToken: _cancellationTokenSource.Token);

      _cancellationTokenSource.Cancel();
      _logger.ZLogDebug($"Claim - {claim}, Remaining - {_gameStateProvider.JudgeRemainTime.Value.TotalSeconds} seconds");
      _logger.ZLogDebug($"PlayerHand: {playerHand.Type}, OpponentHand: {opponentHand.Type}, Truth: {truthResult.Type}, Claim: {claim}");

      // 入力を無効化
      _gameStateProvider.IsInputEnabled.Value = false;

      // 勝敗の正誤を判定
      bool correct = claim switch
      {
        PlayerClaim.PlayerWin => truthResult.Type is JudgeResultType.Win or JudgeResultType.OpponentViolation,
        PlayerClaim.OpponentWin => truthResult.Type is JudgeResultType.Lose or JudgeResultType.Violation,
        PlayerClaim.Draw => truthResult.Type is JudgeResultType.Draw or JudgeResultType.DoubleViolation,
        PlayerClaim.Timeout => false,
        _ => throw new ArgumentOutOfRangeException(nameof(claim), claim, null)
      };
      _logger.ZLogDebug($"Judgement is {(correct ? "Correct" : "Incorrect")}");
      // ターンの結果を生成
      var outcome = new TurnOutcome(truthResult, claim, correct, (judgeTimeLimit - _gameStateProvider.JudgeRemainTime.Value).TotalSeconds);

      // タイマーをリセット
      _gameStateProvider.JudgeRemainTime.Value = _gameStateProvider.JudgeTimeLimit;

      _disposables = new CompositeDisposable { timerObservable };
      return (outcome, UniTask.WhenAll(playerHandPlayTask, opponentHandPlayTask));
    }

    public void Dispose()
    {
      _cancellationTokenSource.Cancel();
      _cancellationTokenSource.Dispose();
      _disposables?.Dispose();
    }
  }
}
