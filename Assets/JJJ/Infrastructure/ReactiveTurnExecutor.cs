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
  /// ターン実行のリアクティブ実装
  /// </summary>
  public class ReactiveTurnExecutor : ITurnExecutor, IDisposable
  {
    private CompositeDisposable _disposables = new CompositeDisposable();
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<ReactiveTurnExecutor>();

    public async UniTask<(TurnOutcome, UniTask)> ExecuteTurn(IRuleSet ruleSet,
                                                ICpuHandStrategy playerStrategy,
                                                ICpuHandStrategy opponentStrategy,
                                                TurnContext context,
                                                TimeSpan limit,
                                                ICompositeHandAnimationPresenter compositeHandAnimationPresenter,
                                                ITimerRemainsPresenter timerRemainsPresenter,
                                                IJudgeInput judgeInput,
                                                ITimerService timerService,
                                                CancellationToken cancellationToken = default)
    {
      _disposables?.Dispose();
      _cancellationTokenSource.Cancel();
      _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      if (_disposables == null)
      {
        _disposables = new CompositeDisposable();
      }

      // CPU hands & truth
      var playerHand = playerStrategy.GetNextCpuHand(context);
      var opponentHand = opponentStrategy.GetNextCpuHand(context);
      var truthResult = ruleSet.Judge(playerHand, opponentHand, context);

      // Observables
      var playerWinObservable = judgeInput.PlayerWinObservable;
      var opponentWinObservable = judgeInput.OpponentWinObservable;
      var drawObservable = judgeInput.DrawObservable;

      // Hand Animation Presenters
      var playerHandAnimationPresenter = compositeHandAnimationPresenter.PlayerHandAnimationPresenter;
      var opponentHandAnimationPresenter = compositeHandAnimationPresenter.OpponentHandAnimationPresenter;

      // 手のアニメーションを再生するUniTask
      if (playerHandAnimationPresenter == null || opponentHandAnimationPresenter == null)
      {
        throw new InvalidOperationException("HandAnimationPresenter is not set.");
      }
      var playerHandPlayTask = playerHandAnimationPresenter.PlayHand(playerHand.Type, playerHand.IsTimeout);
      var opponentHandPlayTask = opponentHandAnimationPresenter.PlayHand(opponentHand.Type, opponentHand.IsTimeout);

      // 時間切れを表すTimeSpan
      var remainingTime = default(TimeSpan);

      // タイマー開始
      var timerObservable = timerService.CountdownEveryFrame(limit, _cancellationTokenSource.Token)
        .Subscribe(remaining =>
        {
          timerRemainsPresenter.SetTimerRemains((float)remaining.TotalSeconds, (float)limit.TotalSeconds);
          remainingTime = remaining;
        });

      _logger.ZLogDebug($"TurnExecutor: Start Turn - Limit {limit.TotalSeconds} seconds");
      // 一番最初に発生したイベントを待つ
      var claim = await Observable.Merge(
        playerWinObservable != null ? playerWinObservable.Select(_ => PlayerClaim.PlayerWin) : Observable.Empty<PlayerClaim>(),
        opponentWinObservable != null ? opponentWinObservable.Select(_ => PlayerClaim.OpponentWin) : Observable.Empty<PlayerClaim>(),
        drawObservable != null ? drawObservable.Select(_ => PlayerClaim.Draw) : Observable.Empty<PlayerClaim>(),
        timerService.After(limit).Select(_ => PlayerClaim.Timeout)
      ).FirstAsync(cancellationToken: _cancellationTokenSource.Token);

      _logger.ZLogDebug($"TurnExecutor: Claim - {claim}");
      _logger.ZLogDebug($"TurnExecutor: Remaining - {remainingTime.TotalSeconds} seconds");
      
      // 入力を無効化
      judgeInput.SetInputEnabled(false);

      _logger.ZLogDebug($"PlayerHand: {playerHand.Type}, OpponentHand: {opponentHand.Type}, Truth: {truthResult.Type}, Claim: {claim}");

      // 勝敗の正誤を判定
      bool correct = claim switch
      {
        PlayerClaim.PlayerWin => truthResult.Type is JudgeResultType.Win or JudgeResultType.OpponentViolation,
        PlayerClaim.OpponentWin => truthResult.Type is JudgeResultType.Lose or JudgeResultType.Violation,
        PlayerClaim.Draw => truthResult.Type is JudgeResultType.Draw or JudgeResultType.DoubleViolation,
        PlayerClaim.Timeout => false,
        _ => throw new ArgumentOutOfRangeException(nameof(claim), claim, null)
      };
      _logger.ZLogDebug($"TurnExecutor: Judgement - {(correct ? "Correct" : "Incorrect")}");
      // ターンの結果を生成
      var outcome = new TurnOutcome(truthResult, claim, correct, (limit - remainingTime).TotalSeconds);

      // タイマー停止
      if (remainingTime.TotalSeconds > 0)
      {
        _cancellationTokenSource.Cancel();
        timerRemainsPresenter.SetTimerRemains((float)limit.TotalSeconds, (float)limit.TotalSeconds);
      }
      
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
