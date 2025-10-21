using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using KanKikuchi.AudioManager;
using R3;
using ZLogger;

namespace JJJ.UseCase
{
  /// <summary>
  /// ゲームのターンを管理するクラス
  /// </summary>
  public class GameTurnManager : IDisposable
  {
    private readonly GameStateProvider _gameStateProvider;
    private readonly ITurnExecutor _turnExecutor;

    private readonly ICompositeHandAnimationPresenter _compositeHandAnimationPresenter;
    private readonly IScoreCalculator _scoreCalculator;
    private readonly ResultDataAggregator _resultDataAggregator;

    private CompositeDisposable _disposables = new CompositeDisposable();

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<GameTurnManager>();

    public GameTurnManager(
      GameStateProvider gameStateProvider,
      ITurnExecutor turnExecutor,
      ICompositeHandAnimationPresenter compositeHandAnimationPresenter,
      IScoreCalculator scoreCalculator,
      ResultDataAggregator resultDataAggregator
    )
    {
      _gameStateProvider = gameStateProvider;
      _turnExecutor = turnExecutor;
      _compositeHandAnimationPresenter = compositeHandAnimationPresenter;
      _scoreCalculator = scoreCalculator;
      _resultDataAggregator = resultDataAggregator;
    }

    /// <summary>
    /// ターンを開始する
    /// </summary>
    /// <returns>引き分けの場合はtrue、勝敗がついた場合はfalseを返すUniTask</returns>
    public async UniTask<bool> StartTurn(CancellationToken cancellationToken = default)
    {
      try
      {
        _logger.ZLogDebug($"StartTurn");

        var currentContext = _gameStateProvider.CurrentTurnContext;
        // 前のターンが両者とも反則でなければターンを進める
        if (!currentContext.IsPreviousTurnDoubleViolation)
        {
          currentContext.NextTurn();
        }
        else
        {
          _logger.ZLogDebug($"Previous turn was double violation. Not advancing turn context.");
          currentContext.SetPreviousTurnDoubleViolation(false);
        }

        _gameStateProvider.CurrentTurnContext = currentContext;

        // ユーザからの入力を受け付ける
        _gameStateProvider.IsInputEnabled.Value = true;
        var (outcome, handAnimationTask) = await _turnExecutor.ExecuteTurn(cancellationToken);

        _logger.ZLogTrace($"Turn Outcome: Claim={outcome.Claim}, Truth={outcome.TruthResult.Type}, PlayerJudgementCorrect={outcome.IsPlayerJudgementCorrect}, JudgeTime={outcome.JudgeTime}s");

        // スコアを計算
        int scoreDiff = _scoreCalculator.CalculateScore(outcome.IsPlayerJudgementCorrect, outcome.JudgeTime);
        _gameStateProvider.ScoreDiff.Value = scoreDiff;
        _gameStateProvider.ScoreDiff.ForceNotify();
        _gameStateProvider.CurrentScore.Value = Math.Max(0, _gameStateProvider.CurrentScore.Value + scoreDiff);

        // SEを再生
        if (outcome.IsPlayerJudgementCorrect)
        {
          SEManager.Instance.Play(SEPath.SE3);
        }
        else
        {
          SEManager.Instance.Play(SEPath.SE4);
        }

        // 両者とも反則だった場合はフラグを立てる
        if (outcome.TruthResult.Type == JudgeResultType.DoubleViolation)
        {
          currentContext.SetPreviousTurnDoubleViolation(true);
        }

        // ジャッジ回数をインクリメント
        _gameStateProvider.JudgeCount.Value += 1;
        // 手のアニメーションの完了を待つ
        await handAnimationTask;

        // リザルトシーンに送るデータを集計する
        _resultDataAggregator.Aggregate(outcome);

        // 引き分けまたは両者反則の場合はターンを継続する
        if (outcome.TruthResult.Type is JudgeResultType.Draw or JudgeResultType.DoubleViolation)
        {
          _logger.ZLogDebug($"Turn resulted in a draw or double violation. Retaining current turn context.");
          // 手をリセット
          await _compositeHandAnimationPresenter.ResetHandAll(cancellationToken);
          if (cancellationToken.IsCancellationRequested)
          {
            _logger.ZLogDebug($"Turn cancelled after draw/double violation.");
            return false;
          }
          return true;
        }
        else
        {
          // 勝敗がついたので次のセッションへ進む
          _logger.ZLogDebug($"Turn completed. Proceeding to next turn.");
          // 手を初期位置に戻す
          await _compositeHandAnimationPresenter.ReturnInitAll(cancellationToken);
          if (cancellationToken.IsCancellationRequested)
          {
            _logger.ZLogDebug($"Turn cancelled after completion.");
          }
          return false;
        }
      }
      catch (OperationCanceledException ex)
      {
        try
        {
          // タイトルに戻るボタン押下などでキャンセルされた場合の処理
          if (_gameStateProvider.GameEndCancellationTokenSource.Token.IsCancellationRequested)
          {
            _logger.ZLogDebug($"Turn cancelled due to game end.");
          }
          else
          {
            _logger.ZLogDebug($"Turn cancelled by external request. \n{ex.Message}");
          }
        }
        catch (ObjectDisposedException disposeEx)
        {
          _logger.ZLogDebug($"Turn cancellation token source already disposed. \n{disposeEx.Message}");
        }
        return false;
      }
      finally
      {
        _disposables.Dispose();
      }
    }

    public void Dispose()
    {
      _disposables.Dispose();
    }
  }
}