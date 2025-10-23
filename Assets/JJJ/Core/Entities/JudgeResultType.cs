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
    /// 相性による勝ち
    /// </summary>
    Win,
    /// <summary>
    /// 相性による負け
    /// </summary>
    Lose,
    /// <summary>
    /// 同じ手を出すことによる引き分け、または効果による引き分け
    /// </summary>
    Draw,
    /// <summary>
    /// 左側のプレイヤーが反則
    /// </summary>
    Violation,
    /// <summary>
    /// 右側のプレイヤーが反則
    /// </summary>
    OpponentViolation,
    /// <summary>
    /// 両プレイヤーが反則
    /// </summary>
    DoubleViolation,
  }
}