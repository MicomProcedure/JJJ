namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// タイマーの残り時間を表示するインターフェース
  /// </summary>
  public interface ITimerRemainsPresenter
  {
    /// <summary>
    /// タイマーの残り時間を表示する
    /// </summary>
    /// <param name="remainTime">残り時間（秒）</param>
    /// <param name="totalTime">合計時間（秒）</param>
    public void SetTimerRemains(float remainTime, float totalTime);
  }
}