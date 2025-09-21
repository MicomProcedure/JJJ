#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
using JJJ.Infrastructure;
using JJJ.Core.Entities;
using NUnit.Framework;

namespace JJJ.Tests.Infrastructure.RuleSet
{
  /// <summary>
  /// EasyRuleSetクラスのテスト
  /// 簡単ルールでのじゃんけん判定とタイムアウト処理をテスト
  /// </summary>
  public class EasyRuleSetTest
  {
    private readonly EasyRuleSet _easyRuleSet = new();
    private TurnContext _turnContext;

    [SetUp]
    public void SetUp()
    {
      // TurnContextの初期化
      _turnContext = new TurnContext();
    }

    /// <summary>
    /// 通常のじゃんけんテスト（タイムアウトなし）
    /// 基本じゃんけん（グー・チョキ・パー）での勝敗判定が正しく行われることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetBasicRockPaperScissorsTestCases))]
    public void Judge_BasicRockPaperScissors_ReturnsExpectedResult(HandType playerHand, HandType opponentHand, JudgeResultType expectedResultType)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = _easyRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResultType),
                  $"Result differs from expected. Player: {playerHand} vs Opponent: {opponentHand}");
    }

    /// <summary>
    /// プレイヤータイムアウトテスト
    /// プレイヤーがタイムアウトした場合に反則と判定されることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetBasicRockPaperScissorsTestCases))]
    public void Judge_WhenPlayerTimesOut_ReturnsViolation(HandType playerHand, HandType opponentHand, JudgeResultType _)
    {
      var player = new Hand(playerHand, playerHand.ToString(), isTimeout: true);
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = _easyRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Violation),
                  $"Player timeout should result in violation. Player: {playerHand}(timeout) vs Opponent: {opponentHand}");
    }

    /// <summary>
    /// 対戦相手タイムアウトテスト
    /// 対戦相手がタイムアウトした場合にプレイヤーの勝利と判定されることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetBasicRockPaperScissorsTestCases))]
    public void Judge_WhenOpponentTimesOut_ReturnsPlayerWin(HandType playerHand, HandType opponentHand, JudgeResultType _)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString(), isTimeout: true);

      var result = _easyRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.OpponentViolation),
                  $"Opponent timeout should result in opponent violation. Player: {playerHand} vs Opponent: {opponentHand}(timeout)");
    }

    /// <summary>
    /// 双方タイムアウトテスト
    /// プレイヤーと対戦相手の両方がタイムアウトした場合に両者反則と判定されることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetBasicRockPaperScissorsTestCases))]
    public void Judge_WhenBothPlayersTimeout_ReturnsBothViolation(HandType playerHand, HandType opponentHand, JudgeResultType _)
    {
      var player = new Hand(playerHand, playerHand.ToString(), isTimeout: true);
      var opponent = new Hand(opponentHand, opponentHand.ToString(), isTimeout: true);

      var result = _easyRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.DoubleViolation),
                  $"Both players timeout should result in both violation. Player: {playerHand}(timeout) vs Opponent: {opponentHand}(timeout)");
    }
  }
}
#endif