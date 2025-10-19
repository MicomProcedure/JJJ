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
    /// ゲーム設定を適用する
    /// </summary>
    public void ApplyGameSettings();

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