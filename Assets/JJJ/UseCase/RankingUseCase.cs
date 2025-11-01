using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;

namespace JJJ.UseCase
{
  public class RankingUseCase
  {
    private readonly IRankingRepository _rankingRepository;

    public RankingUseCase(
      IRankingRepository rankingRepository
    )
    {
      _rankingRepository = rankingRepository;
    }

    public UniTask<IReadOnlyList<RankingData>> GetTopNScoresAsync(GameMode gameMode, int n, CancellationToken? cancellationToken = null)
    {
      return _rankingRepository.LoadTopNScores(gameMode, n, cancellationToken);
    }

    public UniTask SaveScoreAsync(GameMode gameMode, RankingData rankingData, CancellationToken? cancellationToken = null)
    {
      return _rankingRepository.SaveScore(gameMode, rankingData, cancellationToken);
    }
  }
}