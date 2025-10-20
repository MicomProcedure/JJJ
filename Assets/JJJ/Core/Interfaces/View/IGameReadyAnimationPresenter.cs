using System.Threading;
using Cysharp.Threading.Tasks;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// ゲーム開始前のアニメーションを担当するプレゼンターのインターフェース
  /// </summary>
  public interface IGameReadyAnimationPresenter
  {
    /// <summary>
    /// ゲーム開始前のアニメーションを再生する
    /// </summary>
    /// <returns>アニメーション再生完了を待機するためのUniTask</returns>
    public UniTask PlayGameReadyAnimation(CancellationToken cancellationToken = default);
  }
}