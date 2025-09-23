using System;
using JJJ.Core.Entities;
using R3;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// ターンを実行するインターフェース
  /// </summary>
  public interface ITurnExecutor
  {
    /// <summar>
    /// ターンを実行する
    /// </summary>
    /// <param name="ruleSet">ルールセット</param>
    /// <param name="playerStrategy">プレイヤーの戦略</param>
    /// <param name="opponentStrategy">対戦相手の戦略</param>
    /// <param name="context">ターンのコンテキスト</param>
    /// <param name="limit">制限時間</param>
    /// <param name="playerWinObservable">プレイヤーが勝利するとジャッジしたことを通知するObservable</param>
    /// <param name="opponentWinObservable">対戦相手が勝利するとジャッジしたことを通知するObservable</param>
    /// <param name="timerService">タイマーサービス</param>
    /// <returns>ターンの結果を通知するObservable</returns>
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
