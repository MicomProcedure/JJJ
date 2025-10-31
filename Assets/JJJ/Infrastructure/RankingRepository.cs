using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using ProcRanking;

namespace JJJ.Infrastructure
{
  public class RankingRepository : IRankingRepository
  {
    private static string StoreName(GameMode gameMode) => $"JJJ-{gameMode.ToString().ToLower()}-store";

    public UniTask SaveScore(GameMode gameMode, RankingData rankingData)
    {
      string name = rankingData.PlayerName;
      int score = rankingData.Score;

      if (string.IsNullOrEmpty(name)) name = "名無しさん";

      var data = new ProcRaData(StoreName(gameMode))
        .Add("name", name)
        .Add("score", score);

      var tcs = new UniTaskCompletionSource();

      data.SaveAsync(e =>
      {
        if (e != null)
        {
          tcs.TrySetException(new InvalidOperationException(e.Message, e));
        }
        else
        {
          tcs.TrySetResult();
        }
      });

      return tcs.Task;
    }

    public UniTask<IReadOnlyList<RankingData>> LoadTopNScores(GameMode gameMode, int n)
    {
      var query = new ProcRaQuery<ProcRaData>(StoreName(gameMode))
        .SetLimit(n)
        .SetDescSort("score");

      var tcs = new UniTaskCompletionSource<IReadOnlyList<RankingData>>();
      query.FindAsync((list, e) =>
      {
        if (e != null)
        {
          tcs.TrySetException(new InvalidOperationException(e.Message, e));
        }
        else
        {
          var result = list.Select(data => ConvertToRankingData(data)).ToList();
          tcs.TrySetResult(result);
        }
      });


      return tcs.Task;
    }

    private RankingData ConvertToRankingData(ProcRaData data)
    {
      string name = Convert.ToString(data["name"]);
      int score = Convert.ToInt32(data["score"]);
      return new RankingData(name, score);
    }
  }
}