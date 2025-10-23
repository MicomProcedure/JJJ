namespace JJJ.Core.Interfaces
{
  public interface IVisible
  {
    public void Show();
    public void Hide();
  }

  public interface IHelpsView : IVisible { }
  public interface IRankingsView : IVisible { }
}