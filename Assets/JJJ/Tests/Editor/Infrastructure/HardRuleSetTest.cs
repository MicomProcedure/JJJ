#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
namespace JJJ.Tests.Infrastructure
{
  using JJJ.Infrastructure;
  using JJJ.Core.Entities;
  using NUnit.Framework;
  using System.Collections.Generic;

  public class HardRuleSetTest
  {
    private readonly HardRuleSet _hardRuleSet = new();

    // Alpha/Beta特殊手による引き分けテスト
    [TestCase(HandType.Alpha, HandType.Rock, JudgeResultType.Draw)]
    [TestCase(HandType.Rock, HandType.Alpha, JudgeResultType.Draw)]
    [TestCase(HandType.Beta, HandType.Scissors, JudgeResultType.Draw)]
    [TestCase(HandType.Paper, HandType.Beta, JudgeResultType.Draw)]
    [TestCase(HandType.Alpha, HandType.Beta, JudgeResultType.Draw)]
    public void HardRuleSet_Judge_AlphaBetaResultsInDraw(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 1);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Alpha/Beta should always result in Draw: {playerHand} vs {opponentHand}");
    }

    // 偶数ターンでの特別な三角形同士の特殊相性テスト
    [TestCase(HandType.One, HandType.Three, JudgeResultType.Win)]
    [TestCase(HandType.Three, HandType.Scissors, JudgeResultType.Win)]
    [TestCase(HandType.Scissors, HandType.One, JudgeResultType.Win)]
    [TestCase(HandType.Three, HandType.One, JudgeResultType.Lose)]
    [TestCase(HandType.Scissors, HandType.Three, JudgeResultType.Lose)]
    [TestCase(HandType.One, HandType.Scissors, JudgeResultType.Lose)]
    public void HardRuleSet_Judge_EvenTurnSpecialTriangleLogic(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 2); // 偶数ターン

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Even turn special triangle logic: {playerHand} vs {opponentHand}");
    }

    // 奇数ターンでは通常ルールが適用されることをテスト
    [TestCase(HandType.One, HandType.Three, JudgeResultType.Lose)] // 通常ルール
    [TestCase(HandType.Three, HandType.One, JudgeResultType.Win)] // 通常ルール
    public void HardRuleSet_Judge_OddTurnUsesNormalRules(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 1); // 奇数ターン

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Odd turn should use normal rules: {playerHand} vs {opponentHand}");
    }

    // Alphaのバリデーションテスト
    [Test]
    public void HardRuleSet_ValidateHand_AlphaRepeatViolation()
    {
      var hand = new Hand(HandType.Alpha, "Alpha");
      var turnContext = new TurnContext().ActivateAlpha(2); // Alpha有効中

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.AlphaRepeat));
    }

    // Betaのバリデーションテスト
    [Test]
    public void HardRuleSet_ValidateHand_BetaRepeatViolation()
    {
      var hand = new Hand(HandType.Beta, "Beta");
      var turnContext = new TurnContext().ActivateBeta(2, HandType.Rock); // Beta有効中

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.BetaRepeat));
    }

    // 封印された手のバリデーションテスト
    [Test]
    public void HardRuleSet_ValidateHand_SealedHandViolation()
    {
      var hand = new Hand(HandType.Rock, "Rock");
      var turnContext = new TurnContext().ActivateBeta(2, HandType.Rock); // Rockが封印

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.SealedHandUsed));
    }

    // タイムアウトのバリデーションテスト
    [Test]
    public void HardRuleSet_ValidateHand_TimeoutViolation()
    {
      var hand = new Hand(HandType.Rock, "Rock", isTimeout: true);
      var turnContext = new TurnContext();

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.Timeout));
    }

    // 正常な手のバリデーションテスト
    [Test]
    public void HardRuleSet_ValidateHand_ValidHand()
    {
      var hand = new Hand(HandType.Rock, "Rock");
      var turnContext = new TurnContext();

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.True);
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.None));
    }

    // Alphaバリデーション違反による敗北テスト
    [Test]
    public void HardRuleSet_Judge_AlphaViolationResultsInViolation()
    {
      var player = new Hand(HandType.Alpha, "Alpha");
      var opponent = new Hand(HandType.Rock, "Rock");
      var turnContext = new TurnContext().ActivateAlpha(2);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Violation));
      Assert.That(result.PlayerViolationType, Is.EqualTo(ViolationType.AlphaRepeat));
    }

    // 双方バリデーション違反による双方違反テスト
    [Test]
    public void HardRuleSet_Judge_DoubleViolationResultsInDoubleViolation()
    {
      var player = new Hand(HandType.Alpha, "Alpha");
      var opponent = new Hand(HandType.Beta, "Beta");
      var turnContext = new TurnContext().ActivateAlpha(2).ActivateBeta(1, HandType.Rock);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.DoubleViolation));
      Assert.That(result.PlayerViolationType, Is.EqualTo(ViolationType.AlphaRepeat));
      Assert.That(result.OpponentViolationType, Is.EqualTo(ViolationType.BetaRepeat));
    }

    // 通常のじゃんけんルール（RuleSetHelperに委譲）
    [TestCaseSource(nameof(GetNormalJudgeTestCases))]
    public void HardRuleSet_Judge_NormalRulesForRegularHands(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 1); // 奇数ターンで通常ルール

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Normal rules: {playerHand} vs {opponentHand}");
    }

    // テストデータ生成メソッド
    private static IEnumerable<TestCaseData> GetNormalJudgeTestCases()
    {
      var regularHandTypes = new[]
      {
        HandType.Rock, HandType.Scissors, HandType.Paper,
        HandType.One, HandType.Two, HandType.Four // Three は特別な三角形なので除外
      };

      foreach (var playerHand in regularHandTypes)
      {
        foreach (var opponentHand in regularHandTypes)
        {
          var expectedResult = DetermineExpectedResult(playerHand, opponentHand);
          yield return new TestCaseData(playerHand, opponentHand, expectedResult)
            .SetName($"{playerHand}_vs_{opponentHand}_ExpectedResult_{expectedResult}");
        }
      }
    }

    private static JudgeResultType DetermineExpectedResult(HandType playerHand, HandType opponentHand)
    {
      // [recommend] RuleSetHelperに委譲して通常のじゃんけんロジックを使用
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      return RuleSetHelper.DetermineResult(player, opponent).Type;
    }
  }
}
#endif
