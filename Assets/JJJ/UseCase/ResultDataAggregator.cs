using System;
using JJJ.Core.Entities;

namespace JJJ.UseCase
{
  public class ResultDataAggregator
  {
    private readonly GameStateProvider _gameStateProvider;

    public ResultDataAggregator(GameStateProvider gameStateProvider)
    {
      _gameStateProvider = gameStateProvider;
    }

    public void Aggregate(TurnOutcome outcome)
    {
      var _resultSceneData = _gameStateProvider.CurrentResultSceneData;
      switch (outcome.TruthResult.Type)
      {
        // 相性による勝利/敗北/引き分けのジャッジ
        case JudgeResultType.Win or JudgeResultType.Lose or JudgeResultType.Draw:
          // ジャッジが時間切れの場合
          if (outcome.Claim == PlayerClaim.Timeout)
          {
            _resultSceneData.TimeoutCount++;
            break;
          }
          // αの効果による勝利の場合
          if (outcome.TruthResult.WinByAlpha.HasValue)
          {
            _resultSceneData.AlphaCount = AddCountByJudgement(_resultSceneData.AlphaCount, outcome.IsPlayerJudgementCorrect);
            break;
          }
          // 通常の相性による勝利/敗北/引き分けの場合
          _resultSceneData.CompatibilityCount = AddCountByJudgement(_resultSceneData.CompatibilityCount, outcome.IsPlayerJudgementCorrect);
          break;
        // 反則による勝利/敗北/引き分けのジャッジ
        case JudgeResultType.Violation or JudgeResultType.OpponentViolation:
          // 後出しによる反則
          if (AnyPlayerHasViolationType(outcome.TruthResult, ViolationType.Timeout))
          {
            _resultSceneData.TimeoutViolationCount = AddCountByJudgement(_resultSceneData.TimeoutViolationCount, outcome.IsPlayerJudgementCorrect);
          }
          // αの連続使用による反則
          if (AnyPlayerHasViolationType(outcome.TruthResult, ViolationType.AlphaRepeat))
          {
            _resultSceneData.AlphaRepeatCount = AddCountByJudgement(_resultSceneData.AlphaRepeatCount, outcome.IsPlayerJudgementCorrect);
          }
          // βの連続使用による反則
          if (AnyPlayerHasViolationType(outcome.TruthResult, ViolationType.BetaRepeat))
          {
            _resultSceneData.BetaRepeatCount = AddCountByJudgement(_resultSceneData.BetaRepeatCount, outcome.IsPlayerJudgementCorrect);
          }
          // 封印された手の使用による反則
          if (AnyPlayerHasViolationType(outcome.TruthResult, ViolationType.SealedHandUsed))
          {
            _resultSceneData.SealedHandUsedCount = AddCountByJudgement(_resultSceneData.SealedHandUsedCount, outcome.IsPlayerJudgementCorrect);
          }
          break;
        case JudgeResultType.DoubleViolation:
          _resultSceneData.DoubleViolationCount = AddCountByJudgement(_resultSceneData.DoubleViolationCount, outcome.IsPlayerJudgementCorrect);
          break;
        default:
          throw new ArgumentOutOfRangeException("Unknown JudgeResultType", nameof(outcome.TruthResult.Type));
      }
      _gameStateProvider.CurrentResultSceneData = _resultSceneData;
    }
    private (int, int) AddCountByJudgement((int, int) currentCount, bool correct)
    {
      return correct ? (currentCount.Item1 + 1, currentCount.Item2) : (currentCount.Item1, currentCount.Item2 + 1);
    }

    private bool AnyPlayerHasViolationType(JudgeResult result, ViolationType violationType)
    {
      return result.PlayerViolationType == violationType || result.OpponentViolationType == violationType;
    }
  }
}