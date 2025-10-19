using R3;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// プレイヤーの勝利・敗北入力を提供するインタフェース
  /// </summary>
  public interface IJudgeInput
  {
    /// <summary>
    /// プレイヤーの勝利入力のObservable
    /// </summary>
    public Observable<Unit>? PlayerWinObservable { get; }

    /// <summary>
    /// プレイヤーの敗北入力のObservable
    /// </summary>
    public Observable<Unit>? OpponentWinObservable { get; }

    /// <summary>
    /// 引き分け入力のObservable
    /// </summary>
    public Observable<Unit>? DrawObservable { get; }

    /// <summary>
    /// 入力の有効/無効を切り替える
    /// </summary>
    public void SetInputEnabled(bool enabled);
  }
}