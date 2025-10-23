namespace JJJ.Core.Entities
{
  /// <summary>
  /// じゃんけんの反則の種類を定義する列挙型
  /// </summary>
  public enum ViolationType
  {
    /// <summary>
    /// 反則なし
    /// </summary>
    None,
    /// <summary>
    /// 後出し
    /// </summary>
    Timeout,
    /// <summary>
    /// αの効果中にαを出す
    /// </summary>
    AlphaRepeat,
    /// <summary>
    /// βの効果中にβを出す
    /// </summary>
    BetaRepeat,
    /// <summary>
    /// βで封印された手を出す
    /// </summary>
    SealedHandUsed,
  }
}