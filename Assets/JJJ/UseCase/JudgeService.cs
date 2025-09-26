using System;
using System.Collections.Generic;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace JJJ.UseCase
{
  public class JudgeService : IJudgeService, IDisposable, IStartable
  {
    private IRuleSet _ruleSet;
    private IEnumerable<ICpuHandStrategy> _strategies;
    private ITimerService _timerService;
    private readonly TimeSpan _judgeLimit = TimeSpan.FromSeconds(5);
    private Observable<Unit> _playerWinObservable;
    private Observable<Unit> _opponentWinObservable;
    private Observable<Unit> _drawObservable;

    private CompositeDisposable _currentTurnDisposables;

    private ICpuHandStrategy _currentPlayerStrategy;
    private ICpuHandStrategy _currentOpponentStrategy;
    private readonly IStrategySelector _strategySelector;
    private readonly ITurnExecutor _turnExecutor;

    /// <summary>
    /// 現在のターン情報
    /// </summary>
    private TurnContext _currentTurnContext;

    public JudgeService(IRuleSet ruleSet,
                        IEnumerable<ICpuHandStrategy> strategies,
                        ITimerService timerService,
                        IJudgeInput judgeInput,
                        IStrategySelector strategySelector,
                        ITurnExecutor turnExecutor)
    {
      _ruleSet = ruleSet;
      _strategies = strategies;
      _timerService = timerService;
      _playerWinObservable = judgeInput.PlayerWinObservable;
      _opponentWinObservable = judgeInput.OpponentWinObservable;
      _drawObservable = judgeInput.DrawObservable;
      _strategySelector = strategySelector;
      _turnExecutor = turnExecutor;
    }

    void IStartable.Start()
    {
      _currentTurnContext = new TurnContext();
      StartSession();
    }

    /// <summary>
    /// 新しいセッションを開始する
    /// </summary>
    public void StartSession()
    {
      Debug.Log("StartSession");
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
      Debug.Log("StartTurn");
      // 既存ターン用の購読を破棄
      _currentTurnDisposables?.Dispose();
      _currentTurnDisposables = new CompositeDisposable();

      _currentTurnContext.NextTurn();

      // 1ターン実行
      _turnExecutor
        .ExecuteTurn(_ruleSet, _currentPlayerStrategy, _currentOpponentStrategy, _currentTurnContext,
                      _judgeLimit, _playerWinObservable, _opponentWinObservable, _drawObservable, _timerService)
        .Subscribe(outcome =>
        {
          // TODO: Viewへ手・結果通知（outcome.TruthResult, outcome.Claim, outcome.IsPlayerJudgementCorrect）
          if (outcome.TruthResult.Type == JudgeResultType.Draw)
          {
            Observable.TimerFrame(0)
              .Subscribe(_ => StartTurn())
              .AddTo(_currentTurnDisposables);
          }
          else
          {
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