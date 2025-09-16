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
        .AsUnitObservable()
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
    public Observable<TimeSpan> Countdown(TimeSpan duration, TimeSpan tick, CancellationToken ct = default)
    {
      int steps = Math.Max(1, (int)Math.Ceiling(duration.TotalMilliseconds / Math.Max(1, tick.TotalMilliseconds)));
      return Observable.Range(0, steps + 1)
        .Select(i => duration - TimeSpan.FromMilliseconds(i * duration.TotalMilliseconds))
        .TakeUntil(ct);
    }
  }
}