using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
using ProcRanking;

namespace JJJ.Utils
{
  public static class ProcRaUtil
  {
    public static string StoreName(GameMode gameMode) => $"JJJ-{gameMode.ToString().ToLower()}-store";

    public static UniTask SaveAsync(GameMode gameMode, string name, int score)
    {
      if (string.IsNullOrEmpty(name)) name = "名無しさん";

      var data = new ProcRaData(StoreName(gameMode))
        .Add("name", name)
        .Add("score", score);

      var tcs = new UniTaskCompletionSource();

      data.SaveAsync(e =>
      {
        if (e != null)
        {
          tcs.TrySetException(new InvalidOperationException(e.Message));
        }
        else
        {
          tcs.TrySetResult();
        }
      });

      return tcs.Task;
    }

    public static UniTask<IReadOnlyList<T>> LoadTopNAsync<T>(GameMode gameMode, int n) where T : ProcRaData
    {
      var query = new ProcRaQuery<T>(StoreName(gameMode))
        .SetLimit(n)
        .SetDescSort("score");

      var tcs = new UniTaskCompletionSource<IReadOnlyList<T>>();
      query.FindAsync((list, e) =>
      {
        if (e != null)
        {
          tcs.TrySetException(new InvalidOperationException(e.Message));
        }
        else
        {
          tcs.TrySetResult(list);
        }
      });

      return tcs.Task;
    }
  }
}