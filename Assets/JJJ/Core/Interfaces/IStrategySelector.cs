using System.Collections.Generic;

namespace JJJ.Core.Interfaces
{
  public interface IStrategySelector
  {
    public (ICpuHandStrategy player, ICpuHandStrategy opponent) SelectPair(IEnumerable<ICpuHandStrategy> candidates);
  }
}
