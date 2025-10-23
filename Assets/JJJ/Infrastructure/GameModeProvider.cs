using JJJ.Core.Entities;
using JJJ.Core.Interfaces;

namespace JJJ.Infrastructure
{
  /// <summary>
  /// GameMode を提供する実装
  /// </summary>
  public class GameModeProvider : IGameModeProvider
  {
    public GameMode Current { get; private set; } = GameMode.Normal;
    public void Set(GameMode mode) => Current = mode;
  }
}