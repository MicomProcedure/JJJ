#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
using JJJ.Infrastructure;
using JJJ.Core.Entities;
using NUnit.Framework;

namespace JJJ.Tests.Infrastructure.RuleSet
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
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetAlphaBetaTestCases))]
    public void Judge_WithAlphaBeta_ReturnsAlwaysDraw(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand);
      var opponent = new Hand(opponentHand);
      var turnContext = new TurnContext(turnCount: 1);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Alpha/Beta should always result in draw: {playerHand} vs {opponentHand}, expected {expectedResult}, got {result.Type}");
    }

    /// <summary>
    /// Alphaのバリデーションテスト
    /// </summary>
    [Test]
    public void ValidateHand_AlphaRepeat_ReturnsViolation()
    {
      var hand = new Hand(HandType.Alpha);
      var turnContext = new TurnContext().ActivateAlpha(2, PersonType.Player); // Alpha有効中

      var result = _hardRuleSet.ValidateHand(hand, turnContext, PersonType.Player);

      Assert.That(result.IsValid, Is.False,
                  "Alpha hand should not be valid when Alpha is active.");
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.AlphaRepeat),
                  "Alpha hand should result in AlphaRepeat violation.");
    }

    /// <summary>
    /// Betaのバリデーションテスト
    /// </summary>
    [Test]
    public void ValidateHand_BetaRepeat_ReturnsViolation()
    {
      var hand = new Hand(HandType.Beta);
      var turnContext = new TurnContext().ActivateBeta(2, HandType.Rock, PersonType.Player); // Beta有効中

      var result = _hardRuleSet.ValidateHand(hand, turnContext, PersonType.Player);

      Assert.That(result.IsValid, Is.False,
                  "Beta hand should not be valid when Beta is active.");
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.BetaRepeat),
                  "Beta hand should result in BetaRepeat violation.");
    }

    /// <summary>
    /// 封印された手のバリデーションテスト
    /// </summary>
    [Test]
    public void ValidateHand_SealedHand_ReturnsViolation()
    {
      var hand = new Hand(HandType.Rock);
      var turnContext = new TurnContext().ActivateBeta(2, HandType.Rock, PersonType.Player); // Rockが封印

      var result = _hardRuleSet.ValidateHand(hand, turnContext, PersonType.Player);

      Assert.That(result.IsValid, Is.False,
                  "Sealed hand should not be valid when the hand is sealed.");
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.SealedHandUsed),
                  "Sealed hand should result in SealedHandUsed violation.");
    }

    /// <summary>
    /// タイムアウトのバリデーションテスト
    /// </summary>
    [Test]
    public void ValidateHand_Timeout_ReturnsViolation()
    {
      var hand = new Hand(HandType.Rock, isTimeout: true);
      var turnContext = new TurnContext();

      var result = _hardRuleSet.ValidateHand(hand, turnContext, PersonType.Player);

      Assert.That(result.IsValid, Is.False,
                  "Timeout hand should not be valid.");
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.Timeout),
                  "Timeout hand should result in Timeout violation.");
    }

    /// <summary>
    /// 正常な手のバリデーションテスト
    /// </summary>
    [Test]
    public void ValidateHand_ValidHand_ReturnsValid()
    {
      var hand = new Hand(HandType.Rock);
      var turnContext = new TurnContext();

      var result = _hardRuleSet.ValidateHand(hand, turnContext, PersonType.Player);

      Assert.That(result.IsValid, Is.True,
                  "Valid hand should be accepted.");
      Assert.That(result.ViolationType, Is.EqualTo(ViolationType.None),
                  "Valid hand should not result in any violation.");
    }

    /// <summary>
    /// Alphaバリデーション違反による敗北テスト
    /// </summary>
    [Test]
    public void Judge_AlphaViolation_ReturnsViolation()
    {
      var player = new Hand(HandType.Alpha);
      var opponent = new Hand(HandType.Rock);
      var turnContext = new TurnContext().ActivateAlpha(2, PersonType.Player);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Violation),
                  $"Alpha violation should result in violation. Player: {player.Type} vs Opponent: {opponent.Type}, expected {JudgeResultType.Violation}, got {result.Type}");
      Assert.That(result.PlayerViolationType, Is.EqualTo(ViolationType.AlphaRepeat),
                  "Alpha violation should set AlphaRepeat as player violation type.");
    }

    /// <summary>
    /// 双方バリデーション違反による双方違反テスト
    /// </summary>
    [Test]
    public void Judge_DoubleViolation_ReturnsDoubleViolation()
    {
      var player = new Hand(HandType.Alpha);
      var opponent = new Hand(HandType.Beta);
      var turnContext = new TurnContext()
        .ActivateAlpha(2, PersonType.Player)                 // Alphaを2ターン有効化
        .ActivateBeta(1, HandType.Rock, PersonType.Opponent);  // BetaをRockに対して1ターン有効化

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.DoubleViolation),
                  $"Both players violating rules should result in double violation. Player: {player.Type} vs Opponent: {opponent.Type}, expected {JudgeResultType.DoubleViolation}, got {result.Type}");
      Assert.That(result.PlayerViolationType, Is.EqualTo(ViolationType.AlphaRepeat),
                  "Player should have AlphaRepeat violation type.");
      Assert.That(result.OpponentViolationType, Is.EqualTo(ViolationType.BetaRepeat),
                  "Opponent should have BetaRepeat violation type.");
    }

    /// <summary>
    /// 通常のじゃんけんルール（RuleSetHelperに委譲）
    /// </summary>/
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetNormalJudgeTestCases))]
    public void Judge_NormalRules_ReturnsExpectedResult(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand);
      var opponent = new Hand(opponentHand);
      var turnContext = new TurnContext(turnCount: 1); // Odd turn for normal rules

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Normal rules: {playerHand} vs {opponentHand}");
    }

    /// <summary>
    /// Alpha効果中にプレイヤーが勝利または引き分けの場合、引き分けになることをテスト
    /// </summary>
    [TestCase(HandType.Rock, HandType.Scissors)]
    [TestCase(HandType.Rock, HandType.Rock)]
    public void Judge_AlphaActive_PlayerWinBecomesDrawExceptLastTurn(HandType playerHand, HandType opponentHand)
    {
      var player = new Hand(playerHand);
      var opponent = new Hand(opponentHand);
      var turnContext = new TurnContext(turnCount: 1).ActivateAlpha(2, PersonType.Player);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Draw),
                  "Player win should become draw when Alpha is active (not last turn).");
    }

    /// <summary>
    /// Alpha効果の最終ターンにプレイヤーが勝利または引き分けの場合、勝利になることをテスト
    /// </summary>
    [TestCase(HandType.Rock, HandType.Scissors)]
    [TestCase(HandType.Rock, HandType.Rock)]
    public void Judge_AlphaActive_PlayerWinOnLastTurnBecomesWin(HandType playerHand, HandType opponentHand)
    {
      var player = new Hand(playerHand);
      var opponent = new Hand(opponentHand);
      var turnContext = new TurnContext(turnCount: 1).ActivateAlpha(1, PersonType.Player);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Win),
                  "Player win should remain win when Alpha is on last turn.");
    }

    /// <summary>
    /// Alpha効果中にプレイヤーが敗北の場合、そのまま敗北になることをテスト
    /// </summary>
    [Test]
    public void Judge_AlphaActive_PlayerLoseRemainsLose()
    {
      var player = new Hand(HandType.Scissors);
      var opponent = new Hand(HandType.Rock);
      var turnContext = new TurnContext(turnCount: 1).ActivateAlpha(2, PersonType.Player);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Lose),
                  "Player lose should remain lose even when Alpha is active.");
    }

    /// <summary>
    /// 相手のAlpha効果中に相手が勝利または引き分けの場合、引き分けになることをテスト
    /// </summary>
    [TestCase(HandType.Scissors, HandType.Rock)]
    [TestCase(HandType.Rock, HandType.Rock)]
    public void Judge_AlphaActive_OpponentWinBecomesDrawExceptLastTurn(HandType playerHand, HandType opponentHand)
    {
      var player = new Hand(playerHand);
      var opponent = new Hand(opponentHand);
      var turnContext = new TurnContext(turnCount: 1).ActivateAlpha(2, PersonType.Opponent);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Draw),
                  "Opponent win should become draw when Alpha is active (not last turn).");
    }

    /// <summary>
    /// 相手のAlpha効果の最終ターンに相手が勝利または引き分けの場合、敗北になることをテスト
    /// </summary>
    [TestCase(HandType.Scissors, HandType.Rock)]
    [TestCase(HandType.Rock, HandType.Rock)]
    public void Judge_AlphaActive_OpponentWinOnLastTurnBecomesLose(HandType playerHand, HandType opponentHand)
    {
      var player = new Hand(playerHand);
      var opponent = new Hand(opponentHand);
      var turnContext = new TurnContext(turnCount: 1).ActivateAlpha(1, PersonType.Opponent);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Lose),
                  "Opponent win should become lose when Alpha is on last turn.");
    }

    /// <summary>
    /// Alpha効果中に相手が敗北の場合、そのまま勝利になることをテスト
    /// </summary>
    [Test]
    public void Judge_AlphaActive_OpponentLoseRemainsWin()
    {
      var player = new Hand(HandType.Rock);
      var opponent = new Hand(HandType.Scissors);
      var turnContext = new TurnContext(turnCount: 1).ActivateAlpha(2, PersonType.Opponent);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Win),
                  "Player win should remain win when opponent's Alpha is active and player wins.");
    }

    /// <summary>
    /// BetaとThreeの相性テスト - プレイヤーがBeta、相手がThreeの場合は敗北
    /// </summary>
    [Test]
    public void Judge_PlayerBetaVsOpponentThree_ReturnsLose()
    {
      var player = new Hand(HandType.Beta);
      var opponent = new Hand(HandType.Three);
      var turnContext = new TurnContext(turnCount: 1);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Lose),
                  "Player Beta should lose to Opponent Three.");
    }

    /// <summary>
    /// BetaとThreeの相性テスト - 相手がBeta、プレイヤーがThreeの場合は勝利
    /// </summary>
    [Test]
    public void Judge_PlayerThreeVsOpponentBeta_ReturnsWin()
    {
      var player = new Hand(HandType.Three);
      var opponent = new Hand(HandType.Beta);
      var turnContext = new TurnContext(turnCount: 1);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Win),
                  "Player Three should win against Opponent Beta.");
    }

    /// <summary>
    /// Alpha発動時にActivateAlphaが呼ばれることをテスト（プレイヤー）
    /// </summary>
    [Test]
    public void Judge_PlayerAlpha_ActivatesAlphaContext()
    {
      var player = new Hand(HandType.Alpha);
      var opponent = new Hand(HandType.Rock);
      var turnContext = new TurnContext(turnCount: 1);

      _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(turnContext.GetAlphaRemainingTurns(PersonType.Player), Is.GreaterThan(0),
                  "Alpha should be activated for more than 0 turns.");
    }

    /// <summary>
    /// Alpha発動時にActivateAlphaが呼ばれることをテスト（相手）
    /// </summary>
    [Test]
    public void Judge_OpponentAlpha_ActivatesAlphaContext()
    {
      var player = new Hand(HandType.Rock);
      var opponent = new Hand(HandType.Alpha);
      var turnContext = new TurnContext(turnCount: 1);

      _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(turnContext.GetAlphaRemainingTurns(PersonType.Opponent), Is.GreaterThan(0),
                  "Alpha should be activated for more than 0 turns.");
    }

    /// <summary>
    /// Beta発動時にActivateBetaが呼ばれることをテスト（プレイヤー、Three以外）
    /// </summary>
    [Test]
    public void Judge_PlayerBeta_ActivatesBetaContext()
    {
      var player = new Hand(HandType.Beta);
      var opponent = new Hand(HandType.Rock);
      var turnContext = new TurnContext(turnCount: 1);

      _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(turnContext.GetBetaRemainingTurns(PersonType.Player), Is.GreaterThan(0),
                  "Beta should be activated for more than 0 turns.");
      Assert.That(turnContext.GetSealedHandType(PersonType.Player), Is.EqualTo(HandType.Rock),
                  "Opponent's hand should be sealed.");
    }

    /// <summary>
    /// Beta発動時にActivateBetaが呼ばれることをテスト（相手、Three以外）
    /// </summary>
    [Test]
    public void Judge_OpponentBeta_ActivatesBetaContext()
    {
      var player = new Hand(HandType.Paper);
      var opponent = new Hand(HandType.Beta);
      var turnContext = new TurnContext(turnCount: 1);

      _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(turnContext.GetBetaRemainingTurns(PersonType.Opponent), Is.GreaterThan(0),
                  "Beta should be activated for more than 0 turns.");
      Assert.That(turnContext.GetSealedHandType(PersonType.Opponent), Is.EqualTo(HandType.Paper),
                  "Player's hand should be sealed.");
    }

    /// <summary>
    /// Alpha効果中の反則処理テスト - 相手プレイヤーが反則でOpponentViolationの場合、引き分けになる
    /// </summary>
    [Test]
    public void Judge_AlphaActive_PlayerWinByOpponentViolationBecomesDrawExceptLastTurn()
    {
      var player = new Hand(HandType.Rock);
      var opponent = new Hand(HandType.Beta);
      var turnContext = new TurnContext(turnCount: 1)
        .ActivateAlpha(2, PersonType.Player)
        .ActivateBeta(1, HandType.Paper, PersonType.Opponent);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Draw),
                  "Player win by opponent violation should become draw when Alpha is active (not last turn).");
    }

    /// <summary>
    /// Alpha効果中の反則処理テスト - 相手が反則でViolationの場合、引き分けになる
    /// </summary>
    [Test]
    public void Judge_AlphaActive_OpponentWinByPlayerViolationBecomesDrawExceptLastTurn()
    {
      var player = new Hand(HandType.Alpha);
      var opponent = new Hand(HandType.Rock);
      var turnContext = new TurnContext(turnCount: 1)
        .ActivateAlpha(2, PersonType.Opponent);

      var result = _hardRuleSet.Judge(player, opponent, turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Draw),
                  "Opponent win by player violation should become draw when opponent's Alpha is active (not last turn).");
    }
  }
}
#endif