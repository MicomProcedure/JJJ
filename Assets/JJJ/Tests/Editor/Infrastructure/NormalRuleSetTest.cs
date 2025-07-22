#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
namespace JJJ.Tests.Infrastructure
{
  using JJJ.Infrastructure;
  using JJJ.Core.Entities;
  using NUnit.Framework;
  using System.Collections.Generic;
  using System.Linq;

  public class NormalRuleSetTest
  {
    private readonly NormalRuleSet _normalRuleSet = new();
    private readonly TurnContext _turnContext = new();

    // 通常の判定テスト（タイムアウトなし）
    [TestCaseSource(nameof(GetNormalJudgeTestCases))]
    public void NormalRuleSet_Judge_WithoutTimeout(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = _normalRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Player: {playerHand} vs Opponent: {opponentHand}");
    }

    // プレイヤータイムアウトテスト
    [TestCaseSource(nameof(GetPlayerTimeoutTestCases))]
    public void NormalRuleSet_Judge_WithPlayerTimeout(HandType playerHand, HandType opponentHand)
    {
      var player = new Hand(playerHand, playerHand.ToString(), isTimeout: true);
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = _normalRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Violation));
    }

    // 相手タイムアウトテスト
    [TestCaseSource(nameof(GetOpponentTimeoutTestCases))]
    public void NormalRuleSet_Judge_WithOpponentTimeout(HandType playerHand, HandType opponentHand)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString(), isTimeout: true);

      var result = _normalRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Win));
    }

    // 双方タイムアウトテスト
    [TestCaseSource(nameof(GetDoubleTimeoutTestCases))]
    public void NormalRuleSet_Judge_WithDoubleTimeout(HandType playerHand, HandType opponentHand)
    {
      var player = new Hand(playerHand, playerHand.ToString(), isTimeout: true);
      var opponent = new Hand(opponentHand, opponentHand.ToString(), isTimeout: true);

      var result = _normalRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.DoubleViolation));
    }

    // テストデータ生成メソッド
    private static IEnumerable<TestCaseData> GetNormalJudgeTestCases()
    {
      var allHandTypes = GetAllHandTypes();

      foreach (var playerHand in allHandTypes)
      {
        foreach (var opponentHand in allHandTypes)
        {
          var expectedResult = DetermineExpectedResult(playerHand, opponentHand);
          yield return new TestCaseData(playerHand, opponentHand, expectedResult)
            .SetName($"{playerHand}_vs_{opponentHand}_ExpectedResult_{expectedResult}");
        }
      }
    }

    private static IEnumerable<TestCaseData> GetPlayerTimeoutTestCases()
    {
      return GetAllHandCombinations()
        .Select(combo => new TestCaseData(combo.player, combo.opponent)
          .SetName($"PlayerTimeout_{combo.player}_vs_{combo.opponent}"));
    }

    private static IEnumerable<TestCaseData> GetOpponentTimeoutTestCases()
    {
      return GetAllHandCombinations()
        .Select(combo => new TestCaseData(combo.player, combo.opponent)
          .SetName($"OpponentTimeout_{combo.player}_vs_{combo.opponent}"));
    }

    private static IEnumerable<TestCaseData> GetDoubleTimeoutTestCases()
    {
      return GetAllHandCombinations()
        .Select(combo => new TestCaseData(combo.player, combo.opponent)
          .SetName($"DoubleTimeout_{combo.player}_vs_{combo.opponent}"));
    }

    // ヘルパーメソッド
    private static HandType[] GetAllHandTypes()
    {
      return new[]
      {
        HandType.Rock, HandType.Scissors, HandType.Paper,
        HandType.One, HandType.Two, HandType.Three, HandType.Four
      };
    }

    private static IEnumerable<(HandType player, HandType opponent)> GetAllHandCombinations()
    {
      var allHandTypes = GetAllHandTypes();
      return allHandTypes.SelectMany(p => allHandTypes.Select(o => (p, o)));
    }

    private static JudgeResultType DetermineExpectedResult(HandType playerHand, HandType opponentHand)
    {
      // [recommend] RuleSetHelperTestで基本ロジックをテストしているため、ここでは依存して重複を避ける
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      return RuleSetHelper.DetermineResult(player, opponent).Type;
    }
  }
}
#endif