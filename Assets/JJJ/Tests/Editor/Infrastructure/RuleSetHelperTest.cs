#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
namespace JJJ.Tests.Infrastructure
{
  using JJJ.Infrastructure;
  using JJJ.Core.Entities;
  using NUnit.Framework;

  public class RuleSetHelperTest
  {
    // 勝利パターンのテスト
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetWinPatternTestCases))]
    public void DetermineResult_WinPatterns_ReturnsWin(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = RuleSetHelper.DetermineResult(player, opponent);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Player: {playerHand} vs Opponent: {opponentHand}, expected {expectedResult}, got {result.Type}");
    }

    // 引き分けパターンのテスト
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetDrawPatternTestCases))]
    public void DetermineResult_SameHands_ReturnsDraw(HandType handType, HandType opponentHandType, JudgeResultType expectedResult)
    {
      var player = new Hand(handType, handType.ToString());
      var opponent = new Hand(handType, handType.ToString());

      var result = RuleSetHelper.DetermineResult(player, opponent);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Same hands should result in draw. {handType} vs {handType}, expected {expectedResult}, got {result.Type}");
    }

    // 敗北パターンのテスト
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetLosePatternTestCases))]
    public void DetermineResult_LosePatterns_ReturnsLose(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = RuleSetHelper.DetermineResult(player, opponent);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Player: {playerHand} vs Opponent: {opponentHand}, expected {expectedResult}, got {result.Type}");
    }

    // 特別な三角形の判定テスト
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetIsSpecialTriangleTestCases))]
    public void IsSpecialTriangle_ValidatesCorrectly(HandType handType, bool expectedResult)
    {
      var hand = new Hand(handType, handType.ToString());

      bool result = RuleSetHelper.IsSpecialTriangle(hand);

      Assert.That(result, Is.EqualTo(expectedResult),
              $"Hand {handType} should {(expectedResult ? "" : "not ")}be a special triangle.");
    }
  }
}
#endif