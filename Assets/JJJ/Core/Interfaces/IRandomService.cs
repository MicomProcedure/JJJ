namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// 乱数生成サービスのインターフェース
  /// </summary>
  public interface IRandomService
  {
    public int Next(int minValue, int maxValue);

    public int Next(int maxValue);

    public double NextDouble();
  }
}