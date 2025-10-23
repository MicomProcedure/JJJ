using JJJ.Core.Entities;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// GameMode を提供するインターフェース
  /// </summary>
  public interface IGameModeProvider
  {
    public GameMode Current { get; }
    public void Set(GameMode mode);
  }
}