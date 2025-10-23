namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// 乱数生成サービスのインターフェース
  /// </summary>
  public interface IRandomService
  {
    /// <summary>
    /// 指定された範囲の乱数を生成する
    /// </summary>
    /// <param name="minValue">最小値</param>
    /// <param name="maxValue">最大値</param>
    /// <returns>生成された乱数</returns>
    /// <remarks> [minValue, maxValue) の範囲で生成される</remarks>
    public int Next(int minValue, int maxValue);

    /// <summary>
    /// 指定された最大値未満の乱数を生成する
    /// </summary>
    /// <param name="maxValue">最大値</param>
    /// <returns>生成された乱数</returns>
    /// <remarks> [0, maxValue) の範囲で生成される</remarks>
    public int Next(int maxValue);

    /// <summary>
    /// 0.0以上1.0未満の乱数を生成する
    /// </summary>
    /// <returns>生成された乱数</returns>
    /// <remarks> [0.0, 1.0) の範囲で生成される</remarks>
    public double NextDouble();
  }
}