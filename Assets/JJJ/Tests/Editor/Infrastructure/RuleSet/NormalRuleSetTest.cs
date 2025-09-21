#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
using JJJ.Infrastructure;
using JJJ.Core.Entities;
using NUnit.Framework;

namespace JJJ.Tests.Infrastructure.RuleSet
{
  /// <summary>
  /// NormalRuleSetクラスのテスト
  /// 通常ルールでのじゃんけん判定とタイムアウト処理をテスト
  /// </summary>
  public class NormalRuleSetTest
  {
    private readonly NormalRuleSet _normalRuleSet = new();
    private TurnContext _turnContext;

    [SetUp]
    public void SetUp()
    {
      // TurnContextの初期化
      _turnContext = new TurnContext();
    }

    /// <summary>
    /// 通常の判定テスト（タイムアウトなし）
    /// 各手の組み合わせでの勝敗判定が正しく行われることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetNormalJudgeTestCases))]
    public void Judge_NormalCombinations_ReturnsExpectedResult(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = _normalRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Result differs from expected. Player: {playerHand} vs Opponent: {opponentHand}");
    }

    /// <summary>
    /// プレイヤータイムアウトテスト
    /// プレイヤーがタイムアウトした場合に反則と判定されることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetNormalJudgeTestCases))]
    public void Judge_WhenPlayerTimesOut_ReturnsViolation(HandType playerHand, HandType opponentHand, JudgeResultType _)
    {
      var player = new Hand(playerHand, playerHand.ToString(), isTimeout: true);
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = _normalRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Violation),
                  $"Player timeout should result in violation. Player: {playerHand}(timeout) vs Opponent: {opponentHand}");
    }

    /// <summary>
    /// 相手タイムアウトテスト
    /// 相手がタイムアウトした場合にプレイヤーが勝利と判定されることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetNormalJudgeTestCases))]
    public void Judge_WhenOpponentTimesOut_ReturnsWin(HandType playerHand, HandType opponentHand, JudgeResultType _)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString(), isTimeout: true);

      var result = _normalRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.OpponentViolation),
                  $"Opponent timeout should result in opponent violation. Player: {playerHand} vs Opponent: {opponentHand}(timeout)");
    }

    /// <summary>
    /// 双方タイムアウトテスト
    /// 両者がタイムアウトした場合に両者反則と判定されることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetNormalJudgeTestCases))]
    public void Judge_WhenBothTimeout_ReturnsDoubleViolation(HandType playerHand, HandType opponentHand, JudgeResultType _)
    {
      var player = new Hand(playerHand, playerHand.ToString(), isTimeout: true);
      var opponent = new Hand(opponentHand, opponentHand.ToString(), isTimeout: true);

      var result = _normalRuleSet.Judge(player, opponent, _turnContext);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.DoubleViolation),
                  $"Both timeout should result in double violation. Player: {playerHand}(timeout) vs Opponent: {opponentHand}(timeout)");
    }
  }
}
#endif