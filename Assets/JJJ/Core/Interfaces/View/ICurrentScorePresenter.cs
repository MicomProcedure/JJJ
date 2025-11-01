namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// 現在のスコアを表示するコンポーネントのインターフェース
  /// </summary>
  public interface ICurrentScorePresenter
  {
    /// <summary>
    /// 現在のスコアを設定する
    /// </summary>
    /// <param name="currentScore">現在のスコア</param>
    public void SetCurrentScore(int currentScore);

    /// <summary>
    /// スコアの増減を設定する
    /// </summary>
    /// <param name="scoreDiff">スコアの増減</param>
    public void SetScoreDiff(int scoreDiff);
  }
}