using JJJ.Core.Entities;

namespace JJJ.Core.Interfaces
{
  public interface IHighScoreProvider
  {
    public int HighScoreEasy { get; }
    public int HighScoreNormal { get; }
    public int HighScoreHard { get; }

    public void Set(GameMode gameMode, int score);
  }
}