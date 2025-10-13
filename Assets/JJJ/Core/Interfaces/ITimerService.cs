using System;
using System.Threading;
using R3;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// タイマーサービスのインターフェース
  /// </summary>
  public interface ITimerService
  {
    /// <summary>
    /// 現在の日時を取得する
    /// </summary>
    public DateTimeOffset Now { get; }

    /// <summary>
    /// 指定した時間後に一度だけ通知するObservableを取得する
    /// </summary>
    /// <param name="dueTime">通知するまでの時間</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>指定した時間後に一度だけ通知するObservable</returns>
    public Observable<Unit> After(TimeSpan dueTime, CancellationToken ct = default);

    /// <summary>
    /// 指定した周期で通知するObservableを取得する
    /// </summary>
    /// <param name="period">通知する周期</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>指定した周期で通知するObservable</returns>
    public Observable<Unit> Interval(TimeSpan period, CancellationToken ct = default);

    /// <summary>
    /// 指定した時間からカウントダウンするObservableを取得する
    /// </summary>
    /// <param name="duration">カウントダウンする時間</param>
    /// <param name="tick">通知する周期</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>指定した時間からカウントダウンするObservable</returns>
    /// <remarks>通知される値は残り時間を表すTimeSpan</remarks>
    public Observable<TimeSpan> Countdown(TimeSpan duration, TimeSpan tick, CancellationToken ct = default);

    /// <summary>
    /// 指定した時間からカウントダウンするObservableを取得する
    /// </summary>
    /// <param name="duration">カウントダウンする時間</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>指定した時間からカウントダウンするObservable</returns>
    /// <remarks>通知される値は残り時間を表すTimeSpan</remarks>
    public Observable<TimeSpan> CountdownEveryFrame(TimeSpan duration, CancellationToken ct = default);
  }
}