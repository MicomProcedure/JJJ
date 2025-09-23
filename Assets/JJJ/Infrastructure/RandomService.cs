using JJJ.Core.Interfaces;

namespace JJJ.Infrastructure
{
  /// <summary>
  /// 乱数生成サービスの実装
  /// </summary>
  public class RandomService : IRandomService
  {
    private System.Random _random = new System.Random();

    public int Next(int minValue, int maxValue)
    {
      return _random.Next(minValue, maxValue);
    }

    public int Next(int maxValue)
    {
      return _random.Next(maxValue);
    }

    public double NextDouble()
    {
      return _random.NextDouble();
    }
  }
}