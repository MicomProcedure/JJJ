namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// じゃんけんのスコアを計算するインターフェース
  /// </summary>
  public interface IScoreCalculator
  {
    /// <summary>
    /// スコアを計算する
    /// </summary>
    /// <param name="isJudgeCorrect">じゃんけんの判定が正しいかどうか</param>
    /// <param name="judgeTime">じゃんけんの判定にかかった時間（秒）</param>
    public int CalculateScore(bool isJudgeCorrect, double judgeTime);
  }
}