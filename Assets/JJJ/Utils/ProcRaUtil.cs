using JJJ.Core.Entities;
using ProcRanking;

namespace JJJ.Utils
{
  public static class ProcRaUtil
  {
    public static string StoreName(GameMode gameMode) => $"JJJ-{gameMode.ToString().ToLower()}-store";

    public static void Save(GameMode gameMode, string name, int score, ProcRaCallback? callback = null)
    {
      if (string.IsNullOrEmpty(name)) name = "名無しさん";

      var data = new ProcRaData(StoreName(gameMode))
                    .Add("name", name)
                    .Add("score", score);

      data.SaveAsync(callback);
    }
  }
}