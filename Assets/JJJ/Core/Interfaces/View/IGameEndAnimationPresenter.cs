using System.Threading;
using Cysharp.Threading.Tasks;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// ゲーム終了時のアニメーションを担当するプレゼンターのインターフェース
  /// </summary>
  public interface IGameEndAnimationPresenter
  {
    /// <summary>
    /// ゲーム終了時のアニメーションを再生する
    /// </summary>
    /// <returns>アニメーション再生完了を待機するためのUniTask</returns>
    public UniTask PlayGameEndAnimation(CancellationToken cancellationToken = default);
  }
}