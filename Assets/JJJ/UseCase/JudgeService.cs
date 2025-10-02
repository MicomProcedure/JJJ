using System;
using System.Collections.Generic;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using JJJ.View;
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
    private readonly TimeSpan _judgeLimit = TimeSpan.FromSeconds(5);
    private readonly ICompositeHandAnimationPresenter _compositeHandAnimationPresenter;
    private readonly ITimerRemainsPresenter _timerRemainsPresenter;
    private readonly IScoreCalculator _scoreCalculator;
    private readonly CurrentScorePresenter _currentScorePresenter;
    private readonly IJudgeInput _judgeInput;

    private CompositeDisposable _currentTurnDisposables = new CompositeDisposable();

    private ICpuHandStrategy? _currentPlayerStrategy = null;
    private ICpuHandStrategy? _currentOpponentStrategy = null;
    private readonly IStrategySelector _strategySelector;
    private readonly ITurnExecutor _turnExecutor;

    /// <summary>
    /// 現在のターン情報
    /// </summary>
    private TurnContext? _currentTurnContext = null;

    /// <summary>
    /// 現在のスコア
    /// </summary>
    private int _currentScore = 0;

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<JudgeService>();

    public JudgeService(IRuleSet ruleSet,
                        IEnumerable<ICpuHandStrategy> strategies,
                        ITimerService timerService,
                        IJudgeInput judgeInput,
                        ICompositeHandAnimationPresenter compositeHandAnimationPresenter,
                        ITimerRemainsPresenter timerRemainsPresenter,
                        IScoreCalculator scoreCalculator,
                        CurrentScorePresenter currentScorePresenter,
                        IStrategySelector strategySelector,
                        ITurnExecutor turnExecutor)
    {
      _ruleSet = ruleSet;
      _strategies = strategies;
      _timerService = timerService;
      _compositeHandAnimationPresenter = compositeHandAnimationPresenter;
      _timerRemainsPresenter = timerRemainsPresenter;
      _scoreCalculator = scoreCalculator;
      _currentScorePresenter = currentScorePresenter;
      _judgeInput = judgeInput;
      _strategySelector = strategySelector;
      _turnExecutor = turnExecutor;
    }

    void IStartable.Start()
    {
      _logger.ZLogTrace($"JudgeService: Start");
      _currentTurnContext = new TurnContext();
      _currentScore = 0;
      _currentScorePresenter.SetCurrentScore(_currentScore);
      StartSession();
    }

    /// <summary>
    /// 新しいセッションを開始する
    /// </summary>
    public void StartSession()
    {
      _logger.ZLogTrace($"JudgeService: StartSession");
      // 戦略を選択
      (_currentPlayerStrategy, _currentOpponentStrategy) = _strategySelector.SelectPair(_strategies);
      _currentPlayerStrategy.Initialize();
      _currentOpponentStrategy.Initialize();

      // 新しいターンを開始
      StartTurn();
    }

    /// <summary>
    /// 新しいターンを開始する
    /// </summary>
    public void StartTurn()
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

      // 1ターン実行
      _turnExecutor
        .ExecuteTurn(_ruleSet, _currentPlayerStrategy, _currentOpponentStrategy, _currentTurnContext,
                      _judgeLimit, _compositeHandAnimationPresenter, _timerRemainsPresenter, _judgeInput, _timerService)
        .Subscribe(outcome =>
        {
          // TODO: Viewへ手・結果通知（outcome.TruthResult, outcome.Claim, outcome.IsPlayerJudgementCorrect）
          _logger.ZLogTrace($"JudgeService: TurnOutcome - Truth: {outcome.TruthResult.Type}, Claim: {outcome.Claim}, Correct: {outcome.IsPlayerJudgementCorrect}, JudgeTime: {outcome.JudgeTime}");

          // 点数を加算
          int scoreDiff = _scoreCalculator.CalculateScore(outcome.IsPlayerJudgementCorrect, outcome.JudgeTime);
          _currentScore += scoreDiff;
          if (_currentScore < 0) _currentScore = 0;
          _currentScorePresenter.SetCurrentScore(_currentScore);
          _currentScorePresenter.SetScoreDiff(scoreDiff);

          // 引き分けならターン継続、勝敗がついたらセッション再開
          if (outcome.TruthResult.Type == JudgeResultType.Draw)
          {
            _logger.ZLogTrace($"JudgeService: Draw - Restart Turn");
            _compositeHandAnimationPresenter.ResetHandAll();
            Observable.TimerFrame(0)
              .Subscribe(_ => StartTurn())
              .AddTo(_currentTurnDisposables);
          }
          else
          {
            _logger.ZLogTrace($"JudgeService: {outcome.TruthResult.Type} - Restart Session");
            _compositeHandAnimationPresenter.ReturnInitAll();
            Observable.TimerFrame(0)
              .Subscribe(_ => StartSession())
              .AddTo(_currentTurnDisposables);
          }
        })
        .AddTo(_currentTurnDisposables);
    }

    public void Dispose()
    {
      _currentTurnDisposables?.Dispose();
    }
  }

}