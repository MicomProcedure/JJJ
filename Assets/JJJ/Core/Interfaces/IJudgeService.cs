using System.Threading;
using Cysharp.Threading.Tasks;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// じゃんけんの結果を判定するサービスのインターフェース
  /// </summary>
  public interface IJudgeService
  {
    /// <summary>
    /// 新しいセッションを開始する
    /// </summary>
    public UniTask StartSession(CancellationToken cancellationToken = default);

    /// <summary>
    /// 新しいターンを開始する
    /// </summary>
    public UniTask StartTurn(CancellationToken cancellationToken = default);
  }
}