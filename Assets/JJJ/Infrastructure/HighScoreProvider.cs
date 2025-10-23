using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using VContainer.Unity;

namespace JJJ.Infrastructure
{
  public class HighScoreProvider : IHighScoreProvider, IStartable
  {

    private int _highScoreEasy = -1;
    public int HighScoreEasy
    {
      get => _highScoreEasy;
      private set
      {
        if (_highScoreEasy < value) _highScoreEasy = value;
      }
    }

    private int _highScoreNormal = -1;
    public int HighScoreNormal
    {
      get => _highScoreNormal;
      private set
      {
        if (_highScoreNormal < value) _highScoreNormal = value;
      }
    }


    private int _highScoreHard = -1;
    public int HighScoreHard
    {
      get => _highScoreHard;
      private set
      {
        if (_highScoreHard < value) _highScoreHard = value;
      }
    }

    public void Start()
    {
      if (SaveFileHandler.TryLoad<HighScore>(out var data) && data != null)
      {
        Set(GameMode.Easy, data.HighScoreEasy);
        Set(GameMode.Normal, data.HighScoreNormal);
        Set(GameMode.Hard, data.HighScoreHard);
      }
      else
      {
        var highScore = new HighScore();
        SaveFileHandler.Save(highScore);
      }
    }

    public void Set(GameMode gameMode, int score)
    {
      switch (gameMode)
      {
        case GameMode.Easy:
          HighScoreEasy = score;
          break;
        case GameMode.Normal:
          HighScoreNormal = score;
          break;
        case GameMode.Hard:
          HighScoreHard = score;
          break;
      }
    }
  }
}