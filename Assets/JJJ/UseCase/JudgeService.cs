using System;
using System.Collections.Generic;
using System.Linq;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using R3;
using VContainer.Unity;

public class JudgeService : IDisposable, IStartable
{
  private IRuleSet ruleSet;
  private IEnumerable<ICpuHandStrategy> strategies;
  private ITimerService timerService;
  private readonly TimeSpan JudgeLimit = TimeSpan.FromSeconds(5);
  private Observable<Unit> playerWinObservable;
  private Observable<Unit> opponentWinObservable;

  private CompositeDisposable currentTurnDisposables;

  private enum PlayerClaim
  {
    PlayerWin,
    OpponentWin,
    Timeout
  }

  private ICpuHandStrategy currentPlayerStrategy;
  private ICpuHandStrategy currentOpponentStrategy;

  public JudgeService(IRuleSet ruleSet, IEnumerable<ICpuHandStrategy> strategies, ITimerService timerService,
                      Observable<Unit> playerWinObservable, Observable<Unit> opponentWinObservable)
  {
    this.ruleSet = ruleSet;
    this.strategies = strategies;
    this.timerService = timerService;
    this.playerWinObservable = playerWinObservable;
    this.opponentWinObservable = opponentWinObservable;
  }

  /// <summary>
  /// 現在のターン情報
  /// </summary>
  private TurnContext currentTurnContext;

  public void Start()
  {
    currentTurnContext = new TurnContext();
    StartSession();
  }

  /// <summary>
  /// 新しいセッションを開始する
  /// </summary>
  public void StartSession()
  {
    // ランダムに戦略を選択
    var rand = new Random();
    int index = rand.Next(strategies.Count());
    currentPlayerStrategy = strategies.ElementAt(index);
    index = rand.Next(strategies.Count());
    currentOpponentStrategy = strategies.ElementAt(index);
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

    // CPUの手を決定
    var playerHand = currentPlayerStrategy.GetNextCpuHand(currentTurnContext);
    var opponentHand = currentOpponentStrategy.GetNextCpuHand(currentTurnContext);
    var truthResult = ruleSet.Judge(playerHand, opponentHand, currentTurnContext);
    // TODO: Viewへ手の情報を通知 (playerHand, opponentHand)

    // プレイヤーのジャッジと時間切れをraceさせる
    var claimSubject = new Subject<PlayerClaim>();
    var subscription = claimSubject.Take(1).Subscribe(claim =>
    {
      bool isPlayerJudgementCorrect = claim switch
      {
        PlayerClaim.PlayerWin => truthResult.Type == JudgeResultType.Win,
        PlayerClaim.OpponentWin => truthResult.Type == JudgeResultType.Lose,
        PlayerClaim.Timeout => false,
        _ => false
      };

      // TODO: スコア計算や結果通知 (truthResult, claim, isPlayerJudgementCorrect)

      if (truthResult.Type == JudgeResultType.Draw)
      {
        // TODO: 後でアニメーションが終わったら進めるように変更
        Observable.TimerFrame(0)
          .Subscribe(_ => StartTurn())
          .AddTo(currentTurnDisposables);
      }
      else
      {
        // TODO: 後でアニメーションが終わったら進めるように変更
        Observable.TimerFrame(0)
          .Subscribe(_ => StartSession())
          .AddTo(currentTurnDisposables);
      }
    });
    var subPlayer = playerWinObservable.Subscribe(_ => claimSubject.OnNext(PlayerClaim.PlayerWin));
    var subOpponent = opponentWinObservable.Subscribe(_ => claimSubject.OnNext(PlayerClaim.OpponentWin));
    var subTimeout = timerService.After(JudgeLimit).Subscribe(_ => claimSubject.OnNext(PlayerClaim.Timeout));

    currentTurnDisposables.Add(subscription);
    currentTurnDisposables.Add(subPlayer);
    currentTurnDisposables.Add(subOpponent);
    currentTurnDisposables.Add(subTimeout);
    currentTurnDisposables.Add(claimSubject);
  }

  public void Dispose()
  {
    currentTurnDisposables?.Dispose();
  }
}