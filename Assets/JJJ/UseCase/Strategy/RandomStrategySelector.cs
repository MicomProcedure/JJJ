using System;
using System.Collections.Generic;
using System.Linq;
using JJJ.Core.Interfaces;

namespace JJJ.UseCase.Strategy
{
  public class RandomStrategySelector : IStrategySelector
  {
    private readonly Random _random = new Random();

    public (ICpuHandStrategy player, ICpuHandStrategy opponent) SelectPair(IEnumerable<ICpuHandStrategy> candidates)
    {
      if (candidates == null) throw new ArgumentNullException(nameof(candidates));
      var list = candidates.ToList();
      if (list.Count == 0) throw new InvalidOperationException("No strategies provided");

      int p = _random.Next(list.Count);
      int o = _random.Next(list.Count);
      return (list[p], list[o]);
    }
  }
}
