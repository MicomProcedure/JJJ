using System;
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
  public class ReactiveTurnExecutor : ITurnExecutor
  {
    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<ReactiveTurnExecutor>();

    public Observable<TurnOutcome> ExecuteTurn(IRuleSet ruleSet,
                                                ICpuHandStrategy playerStrategy,
                                                ICpuHandStrategy opponentStrategy,
                                                TurnContext context,
                                                TimeSpan limit,
                                                ICompositeHandAnimationPresenter compositeHandAnimationPresenter,
                                                ITimerRemainsPresenter timerRemainsPresenter,
                                                IJudgeInput judgeInput,
                                                ITimerService timerService)
    {
      return Observable.Create<TurnOutcome>(observer =>
          {
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

            // 手のアニメーションを再生
            playerHandAnimationPresenter.PlayHand(playerHand.Type);
            opponentHandAnimationPresenter.PlayHand(opponentHand.Type);

            // プレイヤーのジャッジと時間切れを表すSubject
            var claimSubject = new Subject<PlayerClaim>();
            var remainingTimeSubject = new Subject<TimeSpan>();

            // subscriptions
            var d1 = playerWinObservable.Subscribe(_ => claimSubject.OnNext(PlayerClaim.PlayerWin));
            var d2 = opponentWinObservable.Subscribe(_ => claimSubject.OnNext(PlayerClaim.OpponentWin));
            var d3 = drawObservable.Subscribe(_ => claimSubject.OnNext(PlayerClaim.Draw));
            var d4 = timerService.After(limit).Subscribe(_ => claimSubject.OnNext(PlayerClaim.Timeout));

            timerService.CountdownEveryFrame(limit)
              .Subscribe(remaining =>
              {
                timerRemainsPresenter.SetTimerRemains((float)remaining.TotalSeconds, (float)limit.TotalSeconds);
                remainingTimeSubject.OnNext(remaining);
              });

            // プレイヤー側のボタンを押す、相手側のボタンを押す、タイマーが時間切れになるのうち最初に来たObservableに対して処理を行う
            var dMain = claimSubject
              .Take(1)
              .WithLatestFrom(remainingTimeSubject, (claim, remaining) => (claim, remaining))
              .Subscribe(result =>
              {
                var (claim, remaining) = result;
                _logger.ZLogDebug($"PlayerHand: {playerHand.Type}, OpponentHand: {opponentHand.Type}, Truth: {truthResult.Type}, Claim: {claim}");
                bool correct = claim switch
                {
                  PlayerClaim.PlayerWin => truthResult.Type is JudgeResultType.Win or JudgeResultType.OpponentViolation,
                  PlayerClaim.OpponentWin => truthResult.Type is JudgeResultType.Lose or JudgeResultType.Violation,
                  PlayerClaim.Draw => truthResult.Type == JudgeResultType.Draw,
                  PlayerClaim.Timeout => false,
                  _ => throw new ArgumentOutOfRangeException(nameof(claim), claim, null)
                };
                observer.OnNext(new TurnOutcome(truthResult, claim, correct, (limit - remaining).TotalSeconds));
                observer.OnCompleted();
              });

            var cd = new CompositeDisposable { d1, d2, d3, d4, dMain, claimSubject };
            return cd;
          });
    }
  }
}
