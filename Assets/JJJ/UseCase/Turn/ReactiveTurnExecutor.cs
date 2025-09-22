using System;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using R3;

namespace JJJ.UseCase.Turn
{
  /// <summary>
  /// ターン実行のリアクティブ実装
  /// </summary>
  public class ReactiveTurnExecutor : ITurnExecutor
  {
    public Observable<TurnOutcome> ExecuteTurn(IRuleSet ruleSet,
                                                ICpuHandStrategy playerStrategy,
                                                ICpuHandStrategy opponentStrategy,
                                                TurnContext context,
                                                TimeSpan limit,
                                                Observable<Unit> playerWinObservable,
                                                Observable<Unit> opponentWinObservable,
                                                ITimerService timerService)
    {
      return Observable.Create<TurnOutcome>(observer =>
          {
            // CPU hands & truth
            var playerHand = playerStrategy.GetNextCpuHand(context);
            var opponentHand = opponentStrategy.GetNextCpuHand(context);
            var truthResult = ruleSet.Judge(playerHand, opponentHand, context);

            // プレイヤーのジャッジと時間切れを表すSubject
            var claimSubject = new Subject<PlayerClaim>();

            // subscriptions
            var d1 = playerWinObservable.Subscribe(_ => claimSubject.OnNext(PlayerClaim.PlayerWin));
            var d2 = opponentWinObservable.Subscribe(_ => claimSubject.OnNext(PlayerClaim.OpponentWin));
            var d3 = timerService.After(limit).Subscribe(_ => claimSubject.OnNext(PlayerClaim.Timeout));

            // プレイヤー側のボタンを押す、相手側のボタンを押す、タイマーが時間切れになるのうち最初に来たObservableに対して処理を行う
            var dMain = claimSubject
              .Take(1)
              .Subscribe(claim =>
              {
                bool correct = claim switch
                {
                  PlayerClaim.PlayerWin => truthResult.Type is JudgeResultType.Win or JudgeResultType.OpponentViolation,
                  PlayerClaim.OpponentWin => truthResult.Type is JudgeResultType.Lose or JudgeResultType.Violation,
                  PlayerClaim.Timeout => false,
                  _ => false
                };
                observer.OnNext(new TurnOutcome(truthResult, claim, correct));
                observer.OnCompleted();
              });

            var cd = new CompositeDisposable { d1, d2, d3, dMain, claimSubject };
            return cd;
          });
    }
  }
}
