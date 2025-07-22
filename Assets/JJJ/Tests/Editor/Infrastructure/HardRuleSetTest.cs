#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
namespace JJJ.Tests.Infrastructure
{
  using JJJ.Infrastructure;
  using JJJ.Core.Entities;
  using NUnit.Framework;
  using System.Collections.Generic;

  /// <summary>
  /// HardRuleSetクラスのテスト
  /// ハードルールでのじゃんけん判定、特殊手（Alpha/Beta）、バリデーション処理をテスト
  /// </summary>
  public class HardRuleSetTest
  {
    private readonly HardRuleSet _hardRuleSet = new();

    /// <summary>
    /// Alpha/Beta特殊手による引き分けテスト
    /// Alpha/Betaが絡む場合は常に引き分けになることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetAlphaBetaDrawTestCases))]
    public void Judge_WithAlphaBeta_ReturnsAlwaysDraw(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      // Arrange
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 1);

      // Act
      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      // Assert
      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Alpha/Beta should always result in draw: {playerHand} vs {opponentHand}");
    }

    /// <summary>
    /// 偶数ターンでの特別な三角形同士の特殊相性テスト
    /// 偶数ターンで特別な三角形（チョキ・1・3）同士の場合、通常とは逆の結果になることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetEvenTurnSpecialTriangleTestCases))]
    public void Judge_EvenTurnSpecialTriangle_ReturnsReversedResult(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      // Arrange
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 2); // Even turn

      // Act
      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      // Assert
      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Even turn special triangle logic: {playerHand} vs {opponentHand}");
    }

    // 奇数ターンでは通常ルールが適用されることをテスト
    [TestCaseSource(nameof(GetOddTurnNormalRuleTestCases))]
    public void Judge_OddTurn_UsesNormalRules(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      // Arrange
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 1); // Odd turn

      // Act
      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      // Assert
      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Odd turn should use normal rules: {playerHand} vs {opponentHand}");
    }

    // Alphaのバリデーションテスト
    [Test]
    public void ValidateHand_AlphaRepeat_ReturnsViolation()
    {
      var hand = new Hand(HandType.Alpha, "Alpha");
      var turnContext = new TurnContext().ActivateAlpha(2); // Alpha有効中

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.AlphaRepeat));
    }

    // Betaのバリデーションテスト
    [Test]
    public void ValidateHand_BetaRepeat_ReturnsViolation()
    {
      var hand = new Hand(HandType.Beta, "Beta");
      var turnContext = new TurnContext().ActivateBeta(2, HandType.Rock); // Beta有効中

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.BetaRepeat));
    }

    // 封印された手のバリデーションテスト
    [Test]
    public void ValidateHand_SealedHand_ReturnsViolation()
    {
      var hand = new Hand(HandType.Rock, "Rock");
      var turnContext = new TurnContext().ActivateBeta(2, HandType.Rock); // Rockが封印

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.SealedHandUsed));
    }

    // タイムアウトのバリデーションテスト
    [Test]
    public void ValidateHand_Timeout_ReturnsViolation()
    {
      var hand = new Hand(HandType.Rock, "Rock", isTimeout: true);
      var turnContext = new TurnContext();

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False);
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.Timeout));
    }

    // 正常な手のバリデーションテスト
    [Test]
    public void ValidateHand_ValidHand_ReturnsValid()
    {
      var hand = new Hand(HandType.Rock, "Rock");
      var turnContext = new TurnContext();

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.True);
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.None));
    }

    // Alphaバリデーション違反による敗北テスト
    [Test]
    public void Judge_AlphaViolation_ReturnsViolation()
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
    public void Judge_DoubleViolation_ReturnsDoubleViolation()
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
    public void Judge_NormalRules_ReturnsExpectedResult(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 1); // Odd turn for normal rules

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Normal rules: {playerHand} vs {opponentHand}");
    }

    // ローカルテストデータ生成メソッド
    private static IEnumerable<TestCaseData> GetOddTurnNormalRuleTestCases()
    {
      var testCases = new[]
      {
        (HandType.One, HandType.Three, JudgeResultType.Lose),
        (HandType.Three, HandType.One, JudgeResultType.Win)
      };

      foreach (var (player, opponent, expected) in testCases)
      {
        yield return new TestCaseData(player, opponent, expected)
          .SetName($"OddTurn_{player}_vs_{opponent}_{expected}");
      }
    }

    private static IEnumerable<TestCaseData> GetNormalJudgeTestCases()
    {
      var regularHandTypes = new[]
      {
        HandType.Rock, HandType.Scissors, HandType.Paper,
        HandType.One, HandType.Two, HandType.Four // Three除く（特別な三角形のため）
      };

      foreach (var playerHand in regularHandTypes)
      {
        foreach (var opponentHand in regularHandTypes)
        {
          var expectedResult = TestDataHelper.DetermineExpectedResult(playerHand, opponentHand);
          yield return new TestCaseData(playerHand, opponentHand, expectedResult)
            .SetName($"{playerHand}_vs_{opponentHand}_ExpectedResult_{expectedResult}");
        }
      }
    }
  }
}
#endif