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

    public async UniTask<bool> StartTurn(CancellationToken cancellationToken = default)
    {
      try
      {
        _logger.ZLogDebug($"[GameTurnManager] StartTurn");

        var currentContext = _gameStateProvider.CurrentTurnContext;
        if (!currentContext.IsPreviousTurnDoubleViolation)
        {
          currentContext.NextTurn();
        }
        else
        {
          _logger.ZLogDebug($"[GameTurnManager] Previous turn was double violation. Not advancing turn context.");
          currentContext.SetPreviousTurnDoubleViolation(false);
        }

        _gameStateProvider.CurrentTurnContext = currentContext;
        _gameStateProvider.IsInputEnabled.Value = true;

        var (outcome, handAnimationTask) = await _turnExecutor.ExecuteTurn(cancellationToken);

        _logger.ZLogTrace($"[GameTurnManager] Turn Outcome: {outcome}");

        int scoreDiff = _scoreCalculator.CalculateScore(outcome.IsPlayerJudgementCorrect, outcome.JudgeTime);
        _gameStateProvider.ScoreDiff.Value = scoreDiff;
        _gameStateProvider.ScoreDiff.ForceNotify();
        _gameStateProvider.CurrentScore.Value = Math.Max(0, _gameStateProvider.CurrentScore.Value + scoreDiff);

        if (outcome.IsPlayerJudgementCorrect)
        {
          SEManager.Instance.Play(SEPath.SE3);
        }
        else
        {
          SEManager.Instance.Play(SEPath.SE4);
        }

        if (outcome.TruthResult.Type == JudgeResultType.DoubleViolation)
        {
          currentContext.SetPreviousTurnDoubleViolation(true);
        }

        _gameStateProvider.JudgeCount.Value += 1;
        await handAnimationTask;

        _resultDataAggregator.Aggregate(outcome);

        if (outcome.TruthResult.Type is JudgeResultType.Draw or JudgeResultType.DoubleViolation)
        {
          _logger.ZLogDebug($"[GameTurnManager] Turn resulted in a draw or double violation. Retaining current turn context.");
          await _compositeHandAnimationPresenter.ResetHandAll(cancellationToken);
          if (cancellationToken.IsCancellationRequested)
          {
            _logger.ZLogDebug($"[GameTurnManager] Turn cancelled after draw/double violation.");
            return false;
          }
          return true;
        }
        else
        {
          _logger.ZLogDebug($"[GameTurnManager] Turn completed. Proceeding to next turn.");
          await _compositeHandAnimationPresenter.ReturnInitAll(cancellationToken);
          if (cancellationToken.IsCancellationRequested)
          {
            _logger.ZLogDebug($"[GameTurnManager] Turn cancelled after completion.");
          }
          return false;
        }
      }
      catch (OperationCanceledException ex)
      {
        try
        {
          if (_gameStateProvider.GameEndCancellationTokenSource.Token.IsCancellationRequested)
          {
            _logger.ZLogDebug($"[GameTurnManager] Turn cancelled due to game end.");
          }
          else
          {
            _logger.ZLogDebug($"[GameTurnManager] Turn cancelled by external request. \n{ex.Message}");
          }
        }
        catch (ObjectDisposedException disposeEx)
        {
          _logger.ZLogDebug($"[GameTurnManager] Turn cancellation token source already disposed. \n{disposeEx.Message}");
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