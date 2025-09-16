using System;
using System.Threading;
using R3;
namespace JJJ.Core.Interfaces
{
  public interface ITimerService
  {
    public DateTimeOffset Now { get; }

    public Observable<Unit> After(TimeSpan dueTime, CancellationToken ct = default);

    public Observable<Unit> Interval(TimeSpan period, CancellationToken ct = default);

    public Observable<TimeSpan> Countdown(TimeSpan duration, TimeSpan tick, CancellationToken ct = default);
  }
}