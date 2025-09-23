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
    public void StartSession();

    /// <summary>
    /// 新しいターンを開始する
    /// </summary>
    public void StartTurn();
  }
}