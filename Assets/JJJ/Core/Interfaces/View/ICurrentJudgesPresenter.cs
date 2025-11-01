namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// 現在のジャッジ数を表示するコンポーネントのインターフェース
  /// </summary>
  public interface ICurrentJudgesPresenter
  {
    /// <summary>
    /// 現在のジャッジ数を設定する
    /// </summary>
    /// <param name="currentJudges">現在のジャッジ数</param>
    public void SetCurrentJudges(int currentJudges);
  }
}