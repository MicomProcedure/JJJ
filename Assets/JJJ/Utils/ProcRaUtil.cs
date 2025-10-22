using JJJ.Core.Entities;

namespace JJJ.Utils
{
  public static class ProcRaUtil
  {
    public static string StoreName(GameMode gameMode) => $"JJJ-{gameMode.ToString().ToLower()}-store";
  }
}