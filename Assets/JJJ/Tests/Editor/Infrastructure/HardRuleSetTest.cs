#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
using JJJ.Infrastructure;
using JJJ.Core.Entities;
using NUnit.Framework;
namespace JJJ.Tests.Infrastructure
{
  /// <summary>
  /// HardRuleSetクラスのテスト
  /// ハードルールでのじゃんけん判定、特殊手（Alpha/Beta）、バリデーション処理をテスト
  /// </summary>
  public class HardRuleSetTest
  {
    private readonly HardRuleSet _hardRuleSet = new();

    /// <summary>
    /// Alpha/Beta特殊手による引き分けテスト
    /// 特殊効果の発動時間外において、Alpha/Betaが絡む場合は常に引き分けになることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetAlphaBetaDrawTestCases))]
    public void Judge_WithAlphaBeta_ReturnsAlwaysDraw(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 1);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Alpha/Beta should always result in draw: {playerHand} vs {opponentHand}, expected {expectedResult}, got {result.Type}");
    }

    /// <summary>
    /// 偶数ターンでの特別な三角形同士の特殊相性テスト
    /// 偶数ターンで特別な三角形（チョキ・1・3）同士の場合、通常とは逆の結果になることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetSpecialTriangleTestCases))]
    public void Judge_EvenTurnSpecialTriangle_ReturnsReversedResult(HandType playerHand, HandType opponentHand, bool isWinExpected)
    {
      var expectedResult = isWinExpected ? JudgeResultType.Lose : JudgeResultType.Win;
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 2); // Even turn

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Even turn special triangle logic: {playerHand} vs {opponentHand}, expected {expectedResult}, got {result.Type}");
    }

    // 奇数ターンでは通常ルールが適用されることをテスト
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetSpecialTriangleTestCases))]
    public void Judge_OddTurn_UsesNormalRules(HandType playerHand, HandType opponentHand, bool isWinExpected)
    {
      var expectedResult = isWinExpected ? JudgeResultType.Win : JudgeResultType.Lose;
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 1); // Odd turn

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Odd turn should use normal rules: {playerHand} vs {opponentHand}, expected {expectedResult}, got {result.Type}");
    }

    // Alphaのバリデーションテスト
    [Test]
    public void ValidateHand_AlphaRepeat_ReturnsViolation()
    {
      var hand = new Hand(HandType.Alpha, "Alpha");
      var turnContext = new TurnContext().ActivateAlpha(2); // Alpha有効中

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False, 
                  "Alpha hand should not be valid when Alpha is active.");
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.AlphaRepeat), 
                  "Alpha hand should result in AlphaRepeat violation.");
    }

    // Betaのバリデーションテスト
    [Test]
    public void ValidateHand_BetaRepeat_ReturnsViolation()
    {
      var hand = new Hand(HandType.Beta, "Beta");
      var turnContext = new TurnContext().ActivateBeta(2, HandType.Rock); // Beta有効中

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False,
                  "Beta hand should not be valid when Beta is active.");
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.BetaRepeat),
                  "Beta hand should result in BetaRepeat violation.");
    }

    // 封印された手のバリデーションテスト
    [Test]
    public void ValidateHand_SealedHand_ReturnsViolation()
    {
      var hand = new Hand(HandType.Rock, "Rock");
      var turnContext = new TurnContext().ActivateBeta(2, HandType.Rock); // Rockが封印

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False, 
                  "Sealed hand should not be valid when the hand is sealed.");
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.SealedHandUsed),
                  "Sealed hand should result in SealedHandUsed violation.");
    }

    // タイムアウトのバリデーションテスト
    [Test]
    public void ValidateHand_Timeout_ReturnsViolation()
    {
      var hand = new Hand(HandType.Rock, "Rock", isTimeout: true);
      var turnContext = new TurnContext();

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.False,
                  "Timeout hand should not be valid.");
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.Timeout),
                  "Timeout hand should result in Timeout violation.");
    }

    // 正常な手のバリデーションテスト
    [Test]
    public void ValidateHand_ValidHand_ReturnsValid()
    {
      var hand = new Hand(HandType.Rock, "Rock");
      var turnContext = new TurnContext();

      var result = _hardRuleSet.ValidateHand(hand, turnContext);

      Assert.That(result.IsValid, Is.True,
                  "Valid hand should be accepted.");
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.None),
                  "Valid hand should not result in any violation.");
    }

    // Alphaバリデーション違反による敗北テスト
    [Test]
    public void Judge_AlphaViolation_ReturnsViolation()
    {
      var player = new Hand(HandType.Alpha, "Alpha");
      var opponent = new Hand(HandType.Rock, "Rock");
      var turnContext = new TurnContext().ActivateAlpha(2);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Violation),
                  $"Alpha violation should result in violation. Player: {player.Type} vs Opponent: {opponent.Type}, expected {JudgeResultType.Violation}, got {result.Type}");
      Assert.That(result.PlayerViolationType, Is.EqualTo(ViolationType.AlphaRepeat),
                  "Alpha violation should set AlphaRepeat as player violation type.");
    }

    // 双方バリデーション違反による双方違反テスト
    [Test]
    public void Judge_DoubleViolation_ReturnsDoubleViolation()
    {
      var player = new Hand(HandType.Alpha, "Alpha");
      var opponent = new Hand(HandType.Beta, "Beta");
      var turnContext = new TurnContext()
        .ActivateAlpha(2)                 // Alphaを2ターン有効化
        .ActivateBeta(1, HandType.Rock);  // BetaをRockに対して1ターン有効化

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.DoubleViolation),
                  $"Both players violating rules should result in double violation. Player: {player.Type} vs Opponent: {opponent.Type}, expected {JudgeResultType.DoubleViolation}, got {result.Type}");
      Assert.That(result.PlayerViolationType, Is.EqualTo(ViolationType.AlphaRepeat),
                  "Player should have AlphaRepeat violation type.");
      Assert.That(result.OpponentViolationType, Is.EqualTo(ViolationType.BetaRepeat),
                  "Opponent should have BetaRepeat violation type.");
    }

    // 通常のじゃんけんルール（RuleSetHelperに委譲）
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetNormalJudgeTestCases))]
    public void Judge_NormalRules_ReturnsExpectedResult(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext(turnCount: 1); // Odd turn for normal rules

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Normal rules: {playerHand} vs {opponentHand}");
    }
  }
}
#endif