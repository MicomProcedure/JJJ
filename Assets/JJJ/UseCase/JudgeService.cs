using System;
using System.Collections.Generic;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using R3;
using VContainer.Unity;

namespace JJJ.UseCase
{
  public class JudgeService : IJudgeService, IDisposable, IStartable
  {
    private IRuleSet ruleSet;
    private IEnumerable<ICpuHandStrategy> strategies;
    private ITimerService timerService;
    private readonly TimeSpan JudgeLimit = TimeSpan.FromSeconds(5);
    private Observable<Unit> playerWinObservable;
    private Observable<Unit> opponentWinObservable;

    private CompositeDisposable currentTurnDisposables;

    private ICpuHandStrategy currentPlayerStrategy;
    private ICpuHandStrategy currentOpponentStrategy;
    private readonly IStrategySelector strategySelector;
    private readonly ITurnExecutor turnExecutor;

    public JudgeService(IRuleSet ruleSet,
                        IEnumerable<ICpuHandStrategy> strategies,
                        ITimerService timerService,
                        Observable<Unit> playerWinObservable,
                        Observable<Unit> opponentWinObservable,
                        IStrategySelector strategySelector,
                        ITurnExecutor turnExecutor)
    {
      this.ruleSet = ruleSet;
      this.strategies = strategies;
      this.timerService = timerService;
      this.playerWinObservable = playerWinObservable;
      this.opponentWinObservable = opponentWinObservable;
      this.strategySelector = strategySelector;
      this.turnExecutor = turnExecutor;
    }

    /// <summary>
    /// 現在のターン情報
    /// </summary>
    private TurnContext currentTurnContext;

    void IStartable.Start()
    {
      currentTurnContext = new TurnContext();
      StartSession();
    }

    /// <summary>
    /// 新しいセッションを開始する
    /// </summary>
    public void StartSession()
    {
      // 戦略を選択
      (currentPlayerStrategy, currentOpponentStrategy) = strategySelector.SelectPair(strategies);
      currentPlayerStrategy.Initialize();
      currentOpponentStrategy.Initialize();

      // 新しいターンを開始
      StartTurn();
    }

    /// <summary>
    /// 新しいターンを開始する
    /// </summary>
    public void StartTurn()
    {
      // 既存ターン用の購読を破棄
      currentTurnDisposables?.Dispose();
      currentTurnDisposables = new CompositeDisposable();

      currentTurnContext.NextTurn();

      // 1ターン実行
      turnExecutor
        .ExecuteTurn(ruleSet, currentPlayerStrategy, currentOpponentStrategy, currentTurnContext,
                      JudgeLimit, playerWinObservable, opponentWinObservable, timerService)
        .Subscribe(outcome =>
        {
          // TODO: Viewへ手・結果通知（outcome.TruthResult, outcome.Claim, outcome.IsPlayerJudgementCorrect）
          if (outcome.TruthResult.Type == JudgeResultType.Draw)
          {
            Observable.TimerFrame(0)
              .Subscribe(_ => StartTurn())
              .AddTo(currentTurnDisposables);
          }
          else
          {
            Observable.TimerFrame(0)
              .Subscribe(_ => StartSession())
              .AddTo(currentTurnDisposables);
          }
        })
        .AddTo(currentTurnDisposables);
    }

    public void Dispose()
    {
      currentTurnDisposables?.Dispose();
    }
  }

}