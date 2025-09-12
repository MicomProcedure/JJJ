using JJJ.Core.Entities;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// CPUの手を決定する戦略のインターフェース
  /// </summary>
  public interface ICpuHandStrategy
  {
    /// <summary>
    /// 戦略の初期化
    /// </summary>
    /// <remarks>
    /// 1セッションが始まる前の1回だけ呼ばれる
    /// </remarks>
    public void Initialize();

    /// <summary>
    /// CPUの次の手を決定する
    /// </summary>
    /// <param name="turnContext">現在のターンのコンテキスト</param>
    /// <returns>CPUの次の手</returns>
    public Hand GetNextCpuHand(TurnContext turnContext);
  }
}