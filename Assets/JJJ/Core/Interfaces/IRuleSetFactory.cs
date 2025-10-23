namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// GameMode 等のコンテキストに応じて IRuleSet を生成 / 選択するファクトリ
  /// </summary>
  public interface IRuleSetFactory
  {
    public IRuleSet Create();
  }
}