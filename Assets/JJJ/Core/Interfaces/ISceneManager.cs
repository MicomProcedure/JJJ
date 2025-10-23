using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena.SceneManagement;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// シーン遷移を提供するインターフェース
  /// </summary>
  public interface ISceneManager
  {
    /// <summary>
    /// フェード付きでシーンをプッシュする
    /// </summary>
    /// <param name="sceneIdentifier">シーン識別子</param>
    /// <param name="sceneData">シーンデータ</param>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns>完了を待機できるUniTask</returns>
    public UniTask PushWithFade(ISceneIdentifier sceneIdentifier, ISceneData? sceneData = null, CancellationToken cancellationToken = default);
  }
}