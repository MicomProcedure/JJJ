namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// ゲームの残り時間を表示するコンポーネントのインターフェース
  /// </summary>
  public interface IGameRemainTimePresenter
  {
    /// <summary>
    /// 残り時間を設定する
    /// </summary>
    /// <param name="remainTime">残り時間（秒）</param>
    public void SetRemainGameTime(int remainTime);
  }
}