using System.Threading;
using Cysharp.Threading.Tasks;
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
    public UniTask PlayHand(HandType handType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 手のアニメーションをリセットする
    /// </summary>
    public UniTask ResetHand(CancellationToken cancellationToken = default);

    /// <summary>
    /// 手のアニメーションを初期位置に戻す
    /// </summary>
    public UniTask ReturnInit(CancellationToken cancellationToken = default);
  }
}