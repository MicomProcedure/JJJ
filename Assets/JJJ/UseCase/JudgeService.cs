using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using JJJ.View;
using KanKikuchi.AudioManager;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.Transitions;
using R3;
using VContainer.Unity;
using ZLogger;

namespace JJJ.UseCase
{
  public class JudgeService : IJudgeService, IDisposable, IStartable
  {
    private IRuleSet _ruleSet;
    private IEnumerable<ICpuHandStrategy> _strategies;
    private ITimerService _timerService;
    private readonly IGameModeProvider _gameModeProvider;
    private TimeSpan _gameEndLimit;
    private TimeSpan _judgeLimit;
    private readonly ICompositeHandAnimationPresenter _compositeHandAnimationPresenter;
    private readonly ITimerRemainsPresenter _timerRemainsPresenter;
    private readonly IScoreCalculator _scoreCalculator;
    private readonly CurrentScorePresenter _currentScorePresenter;
    private readonly CurrentJudgesPresenter _currentJudgesPresenter;
    private readonly RemainJudgeTimePresenter _remainJudgeTimePresenter;
    private readonly IJudgeInput _judgeInput;

    private CompositeDisposable _currentTurnDisposables = new CompositeDisposable();

    private ICpuHandStrategy? _currentPlayerStrategy = null;
    private ICpuHandStrategy? _currentOpponentStrategy = null;
    private readonly IStrategySelector _strategySelector;
    private readonly ITurnExecutor _turnExecutor;
    private readonly IGameSettingsProvider _gameSettingsProvider;
    private readonly ITransitionDirector _transitionDirector;
    private readonly IGameReadyAnimationPresenter _gameReadyAnimationPresenter;
    private readonly IGameEndAnimationPresenter _gameEndAnimationPresenter;

    /// <summary>
    /// 現在のターン情報
    /// </summary>
    private TurnContext? _currentTurnContext = null;

    /// <summary>
    /// 現在のスコア
    /// </summary>
    private int _currentScore = 0;

    /// <summary>
    /// 現在のジャッジ回数
    /// </summary>
    private int _judgeCount = 1;

    /// <summary>
    /// リザルトシーンに渡すデータ
    /// </summary>
    private ResultSceneData _resultSceneData = new ResultSceneData();

    private CancellationTokenSource _onGameEndCancellationTokenSource = new CancellationTokenSource();
    private CancellationToken _onGameEndCancellationToken => _onGameEndCancellationTokenSource.Token;

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<JudgeService>();

    public JudgeService(IRuleSet ruleSet,
                        IEnumerable<ICpuHandStrategy> strategies,
                        ITimerService timerService,
                        IJudgeInput judgeInput,
                        ICompositeHandAnimationPresenter compositeHandAnimationPresenter,
                        ITimerRemainsPresenter timerRemainsPresenter,
                        IScoreCalculator scoreCalculator,
                        CurrentScorePresenter currentScorePresenter,
                        CurrentJudgesPresenter currentJudgesPresenter,
                        RemainJudgeTimePresenter remainJudgeTimePresenter,
                        IStrategySelector strategySelector,
                        ITurnExecutor turnExecutor,
                        IGameModeProvider gameModeProvider,
                        IGameSettingsProvider gameSettingsProvider,
                        ITransitionDirector transitionDirector,
                        IGameReadyAnimationPresenter gameReadyAnimationPresenter,
                        IGameEndAnimationPresenter gameEndAnimationPresenter)
    {
      _ruleSet = ruleSet;
      _strategies = strategies;
      _timerService = timerService;
      _compositeHandAnimationPresenter = compositeHandAnimationPresenter;
      _timerRemainsPresenter = timerRemainsPresenter;
      _scoreCalculator = scoreCalculator;
      _currentScorePresenter = currentScorePresenter;
      _currentJudgesPresenter = currentJudgesPresenter;
      _remainJudgeTimePresenter = remainJudgeTimePresenter;
      _judgeInput = judgeInput;
      _strategySelector = strategySelector;
      _turnExecutor = turnExecutor;
      _gameModeProvider = gameModeProvider;
      _gameSettingsProvider = gameSettingsProvider;
      _transitionDirector = transitionDirector;
      _gameReadyAnimationPresenter = gameReadyAnimationPresenter;
      _gameEndAnimationPresenter = gameEndAnimationPresenter;
    }

    public void ApplyGameSettings()
    {
      switch (_gameModeProvider.Current)
      {
        case GameMode.Easy:
          _judgeLimit = TimeSpan.FromSeconds(_gameSettingsProvider.EasyJudgeTimeLimit);
          _gameEndLimit = TimeSpan.FromSeconds(_gameSettingsProvider.EasyGameTimeLimit);
          break;
        case GameMode.Normal:
          _judgeLimit = TimeSpan.FromSeconds(_gameSettingsProvider.NormalJudgeTimeLimit);
          _gameEndLimit = TimeSpan.FromSeconds(_gameSettingsProvider.NormalGameTimeLimit);
          break;
        case GameMode.Hard:
          _judgeLimit = TimeSpan.FromSeconds(_gameSettingsProvider.HardJudgeTimeLimit);
          _gameEndLimit = TimeSpan.FromSeconds(_gameSettingsProvider.HardGameTimeLimit);
          break;
        default:
          _logger.ZLogWarning($"JudgeService: Unknown GameMode {_gameModeProvider.Current}, defaulting judge limit to 10 seconds.");
          _logger.ZLogWarning($"JudgeService: Unknown GameMode {_gameModeProvider.Current}, defaulting game end limit to 60 seconds.");
          _judgeLimit = TimeSpan.FromSeconds(10);
          _gameEndLimit = TimeSpan.FromSeconds(60);
          break;
      }
      _logger.ZLogInformation($"JudgeService: GameMode {_gameModeProvider.Current}, JudgeLimit {_judgeLimit.TotalSeconds}s, GameEndLimit {_gameEndLimit.TotalSeconds}s");
    }

    void IStartable.Start()
    {
      _logger.ZLogTrace($"JudgeService: Start");
      _currentTurnContext = new TurnContext();
      _currentScore = 0;
      _currentScorePresenter.SetCurrentScore(_currentScore);
      _currentJudgesPresenter.SetCurrentJudges(_judgeCount);
      _remainJudgeTimePresenter.SetRemainJudgeTime((int)_gameEndLimit.TotalSeconds);
      _judgeInput.SetInputEnabled(false);

      // ゲーム開始前のアニメーションを再生
      _gameReadyAnimationPresenter.PlayGameReadyAnimation(_onGameEndCancellationToken).ContinueWith(() =>
      {
        _timerService.Countdown(_gameEndLimit, TimeSpan.FromSeconds(1), _onGameEndCancellationToken)
          .Subscribe(t =>
          {
            _remainJudgeTimePresenter.SetRemainJudgeTime((int)t.TotalSeconds);
          }, async _ =>
          {
            if (_onGameEndCancellationToken.IsCancellationRequested)
            {
              _logger.ZLogWarning($"JudgeService: Game ended before timer has expired. Stopping game end process.");
              return;
            }
            // ゲーム終了処理
            _logger.ZLogWarning($"JudgeService: Game Ended");
            _judgeInput.SetInputEnabled(false);
            _onGameEndCancellationTokenSource?.Cancel();

            await _gameEndAnimationPresenter.PlayGameEndAnimation();

            // リザルトシーンへ遷移
            _resultSceneData.Score = _currentScore;
            await GlobalSceneNavigator.Instance.Push(SceneNavigationUtil.ResultSceneIdentifier, _transitionDirector, _resultSceneData);
          });

        // BGMを再生
        BGMManager.Instance.Play(BGMPath.BGM2);
        _judgeInput.SetInputEnabled(true);
        StartSession(_onGameEndCancellationToken).Forget();
      });
    }

    /// <summary>
    /// 新しいセッションを開始する
    /// </summary>
    public async UniTask StartSession(CancellationToken cancellationToken = default)
    {
      _logger.ZLogTrace($"JudgeService: StartSession");
      // 戦略を選択
      (_currentPlayerStrategy, _currentOpponentStrategy) = _strategySelector.SelectPair(_strategies);
      _currentPlayerStrategy.Initialize();
      _currentOpponentStrategy.Initialize();

      // 新しいターンを開始
      _currentTurnContext = new TurnContext();
      await StartTurn(cancellationToken);
    }

    /// <summary>
    /// 新しいターンを開始する
    /// </summary>
    public async UniTask StartTurn(CancellationToken cancellationToken = default)
    {
      try
      {
        _logger.ZLogTrace($"JudgeService: StartTurn");
        // 既存ターン用の購読を破棄
        _currentTurnDisposables?.Dispose();
        _currentTurnDisposables = new CompositeDisposable();

        if (_currentTurnContext == null)
        {
          _currentTurnContext = new TurnContext();
        }
        _currentTurnContext.NextTurn();

        if (_currentPlayerStrategy == null || _currentOpponentStrategy == null)
        {
          throw new InvalidOperationException("Player or Opponent strategy is not set.");
        }

        // 1ターン実行
        var (outcome, handAnimationTask) = await _turnExecutor.ExecuteTurn(_ruleSet, _currentPlayerStrategy, _currentOpponentStrategy, _currentTurnContext,
                        _judgeLimit, _compositeHandAnimationPresenter, _timerRemainsPresenter, _judgeInput, _timerService, cancellationToken);


        _logger.ZLogTrace($"JudgeService: TurnOutcome - Truth: {outcome.TruthResult.Type}, Claim: {outcome.Claim}, Correct: {outcome.IsPlayerJudgementCorrect}, JudgeTime: {outcome.JudgeTime}");

        // 点数を加算
        int scoreDiff = _scoreCalculator.CalculateScore(outcome.IsPlayerJudgementCorrect, outcome.JudgeTime);
        _currentScore += scoreDiff;
        if (_currentScore < 0) _currentScore = 0;
        _currentScorePresenter.SetCurrentScore(_currentScore);
        _currentScorePresenter.SetScoreDiff(scoreDiff);

        // 正解/不正解音の再生
        if (outcome.IsPlayerJudgementCorrect)
        {
          SEManager.Instance.Play(SEPath.SE3);
        }
        else
        {
          SEManager.Instance.Play(SEPath.SE4);
        }

        // 手のアニメーションが完了するまで待機
        await handAnimationTask;

        // ジャッジ回数を更新
        _judgeCount++;
        _currentJudgesPresenter.SetCurrentJudges(_judgeCount);

        // リザルトシーン用データ更新
        UpdateResultSceneData(outcome);

        // 引き分けならターン継続、勝敗がついたらセッション再開
        if (outcome.TruthResult.Type is JudgeResultType.Draw or JudgeResultType.DoubleViolation)
        {
          _logger.ZLogTrace($"JudgeService: Draw - Restart Turn");
          await _compositeHandAnimationPresenter.ResetHandAll();
          _judgeInput.SetInputEnabled(true);
          if (cancellationToken.IsCancellationRequested)
          {
            _logger.ZLogWarning($"JudgeService: Cancellation requested. Ending session.");
            await UniTask.CompletedTask;
          }
          StartTurn(cancellationToken).Forget();
        }
        else
        {
          _logger.ZLogTrace($"JudgeService: {outcome.TruthResult.Type} - Restart Session");
          await _compositeHandAnimationPresenter.ReturnInitAll();
          _judgeInput.SetInputEnabled(true);
          if (cancellationToken.IsCancellationRequested)
          {
            _logger.ZLogWarning($"JudgeService: Cancellation requested. Ending session.");
            await UniTask.CompletedTask;
          }
          StartSession(cancellationToken).Forget();
        }
      }
      catch (OperationCanceledException ex)
      {
        try
        {
          if (_onGameEndCancellationToken.IsCancellationRequested)
          {
            _logger.ZLogWarning($"JudgeService: Game end timeout reached. Ending session.");
          }
          else
          {
            _logger.ZLogWarning($"JudgeService: Operation canceled - {ex.Message}");
          }
          await UniTask.CompletedTask;
        }
        catch (ObjectDisposedException disposeEx)
        {
          _logger.ZLogWarning($"JudgeService: Object disposed during cancellation handling - {disposeEx.Message}");
        }
      }
    }

    public void Dispose()
    {
      _onGameEndCancellationTokenSource.Cancel();
      _onGameEndCancellationTokenSource.Dispose();
      _currentTurnDisposables?.Dispose();
    }

    private void UpdateResultSceneData(TurnOutcome outcome)
    {
      switch (outcome.TruthResult.Type)
      {
        // 相性による勝利/敗北/引き分けのジャッジ
        case JudgeResultType.Win or JudgeResultType.Lose or JudgeResultType.Draw:
          // ジャッジが時間切れの場合
          if (outcome.Claim == PlayerClaim.Timeout)
          {
            _resultSceneData.TimeoutCount++;
            break;
          }
          // αの効果による勝利の場合
          if (outcome.TruthResult.WinByAlpha.HasValue)
          {
            _resultSceneData.AlphaCount = AddCountByJudgement(_resultSceneData.AlphaCount, outcome.IsPlayerJudgementCorrect);
            break;
          }
          // 通常の相性による勝利/敗北/引き分けの場合
          _resultSceneData.CompatibilityCount = AddCountByJudgement(_resultSceneData.CompatibilityCount, outcome.IsPlayerJudgementCorrect);
          break;
        // 反則による勝利/敗北/引き分けのジャッジ
        case JudgeResultType.Violation or JudgeResultType.OpponentViolation:
          // 後出しによる反則
          if (AnyPlayerHasViolationType(outcome.TruthResult, ViolationType.Timeout))
          {
            _resultSceneData.TimeoutViolationCount = AddCountByJudgement(_resultSceneData.TimeoutViolationCount, outcome.IsPlayerJudgementCorrect);
          }
          // αの連続使用による反則
          if (AnyPlayerHasViolationType(outcome.TruthResult, ViolationType.AlphaRepeat))
          {
            _resultSceneData.AlphaRepeatCount = AddCountByJudgement(_resultSceneData.AlphaRepeatCount, outcome.IsPlayerJudgementCorrect);
          }
          // βの連続使用による反則
          if (AnyPlayerHasViolationType(outcome.TruthResult, ViolationType.BetaRepeat))
          {
            _resultSceneData.BetaRepeatCount = AddCountByJudgement(_resultSceneData.BetaRepeatCount, outcome.IsPlayerJudgementCorrect);
          }
          // 封印された手の使用による反則
          if (AnyPlayerHasViolationType(outcome.TruthResult, ViolationType.SealedHandUsed))
          {
            _resultSceneData.SealedHandUsedCount = AddCountByJudgement(_resultSceneData.SealedHandUsedCount, outcome.IsPlayerJudgementCorrect);
          }
          break;
        case JudgeResultType.DoubleViolation:
          _resultSceneData.DoubleViolationCount = AddCountByJudgement(_resultSceneData.DoubleViolationCount, outcome.IsPlayerJudgementCorrect);
          break;
        default:
          throw new ArgumentOutOfRangeException("Unknown JudgeResultType", nameof(outcome.TruthResult.Type));
      }
    }

    private (int, int) AddCountByJudgement((int, int) currentCount, bool correct)
    {
      return correct ? (currentCount.Item1 + 1, currentCount.Item2) : (currentCount.Item1, currentCount.Item2 + 1);
    }

    private bool AnyPlayerHasViolationType(JudgeResult result, ViolationType violationType)
    {
      return result.PlayerViolationType == violationType || result.OpponentViolationType == violationType;
    }
  }
}