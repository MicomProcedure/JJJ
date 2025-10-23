using System;
using System.Collections.Generic;
using System.Linq;
using JJJ.Core.Interfaces;

namespace JJJ.UseCase.Strategy
{
  /// <summary>
  /// ランダムに戦略を選択する実装
  /// </summary>
  public class RandomStrategySelector : IStrategySelector
  {
    /// <summary>
    /// 乱数生成サービス
    /// </summary>
    private IRandomService _randomService;

    public RandomStrategySelector(IRandomService randomService)
    {
      _randomService = randomService;
    }

    public (ICpuHandStrategy player, ICpuHandStrategy opponent) SelectPair(IEnumerable<ICpuHandStrategy> candidates)
    {
      if (candidates == null) throw new ArgumentNullException(nameof(candidates));
      var list = candidates.ToList();
      if (list.Count == 0) throw new InvalidOperationException("No strategies provided");

      int p = _randomService.Next(list.Count);
      int o = _randomService.Next(list.Count);
      return (list[p], list[o]);
    }
  }
}
