namespace JJJ.Core.Entities
{
  /// <summary>
  /// 手が反則でないかどうか判定した結果を表す構造体
  /// </summary>
  public readonly struct HandValidationResult
  {
    /// <summary>
    /// 手が反則でないかどうか
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// 反則時の理由
    /// </summary>
    /// <remarks>
    /// 反則がない場合は ViolationType.None になる
    /// </remarks>
    public ViolationType ViolationType { get; }

    /// <summary>
    /// HandValidationResult 構造体のコンストラクタ
    /// </summary>
    /// <param name="isValid">手が反則でないかどうか</param>
    /// <param name="violationType">反則時の理由</param>
    public HandValidationResult(bool isValid, ViolationType violationType)
    {
      IsValid = isValid;
      ViolationType = violationType;
    }
  }

}