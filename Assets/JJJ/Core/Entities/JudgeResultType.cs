namespace JJJ.Core.Entities
{
  /// <summary>
  /// じゃんけんの結果の種類を表す列挙型
  /// </summary>
  /// <remarks>
  /// この結果は左側のプレイヤーの結果
  /// </remarks>
  public enum JudgeResultType
  {
    /// <summary>
    /// 勝ち
    /// </summary>
    Win,
    /// <summary>
    /// 負け
    /// </summary>
    Lose,
    /// <summary>
    /// 引き分け
    /// </summary>
    Draw,
    /// <summary>
    /// 反則
    /// </summary>
    Violation,
    /// <summary>
    /// 両プレイヤーが反則
    /// </summary>
    DoubleViolation,
  }
}