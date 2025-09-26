using JJJ.Core.Entities;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// 手のアニメーションを表示するインターフェース
  /// </summary>
  public interface IHandAnimationPresenter
  {
    /// <summary>
    /// 手のアニメーションを再生する
    /// </summary>
    public void PlayHand(HandType handType);

    /// <summary>
    /// 手のアニメーションをリセットする
    /// </summary>
    public void ResetHand();

    /// <summary>
    /// 手のアニメーションを初期位置に戻す
    /// </summary>
    public void ReturnInit();
  }
}