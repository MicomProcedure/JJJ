using System;
using JJJ.Core.Entities;
using R3;

namespace JJJ.Core.Interfaces
{
  public interface ITurnExecutor
  {
    public Observable<TurnOutcome> ExecuteTurn(IRuleSet ruleSet,
                                         ICpuHandStrategy playerStrategy,
                                         ICpuHandStrategy opponentStrategy,
                                         TurnContext context,
                                         TimeSpan limit,
                                         Observable<Unit> playerWinObservable,
                                         Observable<Unit> opponentWinObservable,
                                         ITimerService timerService);
  }
}
