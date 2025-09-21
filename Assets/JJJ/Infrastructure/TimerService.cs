using System;
using System.Threading;
using JJJ.Core.Interfaces;
using R3;

namespace JJJ.Infrastructure
{
  /// <summary>
  /// タイマーサービスの実装
  /// </summary>
  public sealed class TimerService : ITimerService
  {
    /// <summary>
    /// 現在の日時を取得する
    /// </summary>
    public DateTimeOffset Now => DateTimeOffset.UtcNow;

    /// <summary>
    /// 指定した時間後に通知を発行する
    /// </summary>
    /// <param name="dueTime">通知を発行するまでの時間</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>指定した時間後に1回だけ通知を発行するObservable</returns>
    public Observable<Unit> After(TimeSpan dueTime, CancellationToken ct = default)
      => Observable.Timer(dueTime)
        .TakeUntil(ct);

    /// <summary>
    /// 指定した周期で通知を発行する
    /// </summary>
    /// <param name="period">通知を発行する周期</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>指定した周期で通知を発行するObservable</returns>
    public Observable<Unit> Interval(TimeSpan period, CancellationToken ct = default)
      => Observable.Interval(period).TakeUntil(ct);

    /// <summary>
    /// 指定した期間をカウントダウンする
    /// </summary>
    /// <param name="duration">カウントダウンする期間</param>
    /// <param name="tick">通知を発行する周期</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>指定した期間をカウントダウンするObservable</returns>
    /// <remarks>
    /// 通知の値は残り時間を表すTimeSpanで、durationから0まで減少する
    /// 最初の通知は即座に発行され、最後の通知は0を表すTimeSpanとなる
    /// </remarks>
    public Observable<TimeSpan> Countdown(TimeSpan duration, TimeSpan tick, CancellationToken ct = default)
    {
      if (duration <= TimeSpan.Zero)
      {
        UnityEngine.Debug.LogError("duration must be greater than zero. Falling back to TimeSpan.FromSeconds(1).");
        duration = TimeSpan.FromSeconds(1);
      }
      if (tick <= TimeSpan.Zero)
      {
        UnityEngine.Debug.LogError("tick must be greater than zero. Falling back to TimeSpan.FromSeconds(1).");
        tick = TimeSpan.FromSeconds(1);
      }

      int steps = (int)Math.Ceiling(duration.Ticks / (double)tick.Ticks);

      return Observable
        .Timer(TimeSpan.Zero, tick)
        .Index()
        .Select(i =>
        {
          long elapsedTicks = Math.Min(duration.Ticks, (i + 1) * tick.Ticks);
          long remainingTicks = duration.Ticks - elapsedTicks;
          return TimeSpan.FromTicks(remainingTicks);
        })
        .Take(steps + 1)
        .TakeUntil(ct);
    }
  }
}