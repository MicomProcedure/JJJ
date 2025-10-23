using System.Collections.Generic;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// CPUの手の戦略を選択するインターフェース
  /// </summary>
  public interface IStrategySelector
  {
    /// <summary>
    /// 戦略の候補から、プレイヤーと対戦相手の戦略のペアを選択する
    /// </summary>
    /// <param name="candidates">戦略の候補</param>
    /// <returns>選択されたプレイヤーと対戦相手の戦略のペア</returns>
    public (ICpuHandStrategy player, ICpuHandStrategy opponent) SelectPair(IEnumerable<ICpuHandStrategy> candidates);
  }
}
