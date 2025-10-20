using System;
using System.Threading;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using R3;

namespace JJJ.UseCase
{
  /// <summary>
  /// ゲーム内の状態を提供するクラス
  /// </summary>
  public sealed class GameStateProvider
  {
    public ICpuHandStrategy PlayerCpuHandStrategy { get; set; } = null!;
    public ICpuHandStrategy OpponentCpuHandStrategy { get; set; } = null!;
    public TurnContext CurrentTurnContext { get; set; } = new TurnContext();
    public ResultSceneData CurrentResultSceneData { get; set; } = new ResultSceneData();
    public CancellationTokenSource GameEndCancellationTokenSource { get; set; } = new CancellationTokenSource();
    public TimeSpan GameEndLimit { get; set; } = TimeSpan.FromSeconds(60);
    public TimeSpan JudgeTimeLimit { get; set; } = TimeSpan.FromSeconds(10);

    public ReactiveProperty<TimeSpan> GameRemainTime { get; } = new(TimeSpan.FromSeconds(0));
    public ReactiveProperty<TimeSpan> JudgeRemainTime { get; } = new(TimeSpan.FromSeconds(0));
    public ReactiveProperty<int> CurrentScore { get; } = new(0);
    public ReactiveProperty<int> ScoreDiff { get; } = new(0);
    public ReactiveProperty<int> JudgeCount { get; } = new(0);
    public ReactiveProperty<bool> IsInputEnabled { get; } = new(false);

    /// <summary>
    /// ゲーム終了時に発行されるイベント
    /// </summary>
    /// <remarks>時間切れ時とタイトルに戻るボタン押下時に発行される</remarks>
    /// <seealso cref="OnTimerHasExpired"/>
    public Subject<Unit> OnGameEnd { get; } = new Subject<Unit>();

    /// <summary>
    /// タイマーが時間切れになったときに発行されるイベント
    /// </summary>
    public Subject<Unit> OnTimerHasExpired { get; } = new Subject<Unit>();
  }
}