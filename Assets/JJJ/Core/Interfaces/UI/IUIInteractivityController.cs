namespace JJJ.Core.Interfaces.UI
{
  /// <summary>
  /// UIのインタラクティビティを制御するためのインターフェース
  /// </summary>
  public interface IUIInteractivityController
  {
    public void DisableAllInteractivity();
    public void EnableAllInteractivity();
  }
}