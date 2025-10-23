using JJJ.Core.Interfaces;
using JJJ.Utils;
using UnityEngine;
using ZLogger;

namespace JJJ.Infrastructure
{
  public class ScoreCalculator : IScoreCalculator
  {
    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<ScoreCalculator>();

    public int CalculateScore(bool isJudgeCorrect, double judgeTime)
    {
      if (isJudgeCorrect)
      {
        // 判定が正しい場合、基礎点100点に加えて、判定時間に応じたボーナス点を加算
        int baseScore = 100;
        int timeBonus = (int)(Mathf.Max(0, 5.0f - (float)judgeTime) * 20); // 5秒以内ならボーナス
        _logger.ZLogTrace($"ScoreCalculator: Correct judgement. Base Score: {baseScore}, Time Bonus: {timeBonus}, Total: {baseScore + timeBonus}");
        return baseScore + timeBonus;
      }
      else
      {
        // 判定が間違っている場合、ペナルティとして200点減点
        _logger.ZLogTrace($"ScoreCalculator: Incorrect judgement. Penalty: -200");
        return -200;
      }
    }
  }
}