using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// ターンを実行するインターフェース
  /// </summary>
  public interface ITurnExecutor
  {
    /// <summar>
    /// ターンを実行する
    /// </summary>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns>ターンの結果と手のアニメーションが完了するまで待機するUniTaskのタプル</returns>
    public UniTask<(TurnOutcome, UniTask)> ExecuteTurn(CancellationToken cancellationToken = default);
  }
}
