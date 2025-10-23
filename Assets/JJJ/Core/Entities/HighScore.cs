namespace JJJ.Core.Entities
{
  /// <summary>
  /// ユーザのローカルのハイスコアのデータを表すクラス
  /// </summary>
  public class HighScore
  {
    /// <summary>
    /// 難易度：カンタン のハイスコア
    /// </summary>
    public int HighScoreEasy;

    /// <summary>
    /// 難易度:フツウ のハイスコア
    /// </summary>
    public int HighScoreNormal;

    /// <summary>
    /// 難易度:ムズカシイ のハイスコア
    /// </summary>
    public int HighScoreHard;

    public HighScore(int highScoreEasy = -1, int highScoreNormal = -1, int highScoreHard = -1)
    {
      HighScoreEasy = highScoreEasy;
      HighScoreNormal = highScoreNormal;
      HighScoreHard = highScoreHard;
    }
  }
}