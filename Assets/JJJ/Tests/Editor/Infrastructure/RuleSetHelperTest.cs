#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
namespace JJJ.Tests.Infrastructure
{
  using JJJ.Infrastructure;
  using JJJ.Core.Entities;
  using NUnit.Framework;

  public class RuleSetHelperTest
  {
    [TestCase(HandType.Rock, HandType.Scissors, JudgeResultType.Win)]
    [TestCase(HandType.Rock, HandType.Three, JudgeResultType.Win)]
    [TestCase(HandType.Scissors, HandType.Paper, JudgeResultType.Win)]
    [TestCase(HandType.Scissors, HandType.Three, JudgeResultType.Win)]
    [TestCase(HandType.Paper, HandType.Rock, JudgeResultType.Win)]
    [TestCase(HandType.Paper, HandType.Three, JudgeResultType.Win)]
    [TestCase(HandType.One, HandType.Scissors, JudgeResultType.Win)]
    [TestCase(HandType.One, HandType.Two, JudgeResultType.Win)]
    [TestCase(HandType.Two, HandType.Rock, JudgeResultType.Win)]
    [TestCase(HandType.Two, HandType.Paper, JudgeResultType.Win)]
    [TestCase(HandType.Three, HandType.One, JudgeResultType.Win)]
    [TestCase(HandType.Three, HandType.Two, JudgeResultType.Win)]
    [TestCase(HandType.Three, HandType.Four, JudgeResultType.Win)]
    [TestCase(HandType.Four, HandType.One, JudgeResultType.Win)]
    public void DetermineResult_WinPatterns_ReturnsWin(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = RuleSetHelper.DetermineResult(player, opponent);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Player: {playerHand} vs Opponent: {opponentHand}");
    }

    [TestCase(HandType.Rock, HandType.Rock)]
    [TestCase(HandType.Scissors, HandType.Scissors)]
    [TestCase(HandType.Paper, HandType.Paper)]
    [TestCase(HandType.One, HandType.One)]
    [TestCase(HandType.Two, HandType.Two)]
    [TestCase(HandType.Three, HandType.Three)]
    [TestCase(HandType.Four, HandType.Four)]
    public void DetermineResult_SameHands_ReturnsDraw(HandType handType, HandType opponentHandType)
    {
      var player = new Hand(handType, handType.ToString());
      var opponent = new Hand(opponentHandType, opponentHandType.ToString());

      var result = RuleSetHelper.DetermineResult(player, opponent);

      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Draw));
    }

    [TestCase(HandType.Scissors, HandType.One, JudgeResultType.Lose)]
    [TestCase(HandType.Rock, HandType.Two, JudgeResultType.Lose)]
    [TestCase(HandType.Paper, HandType.Scissors, JudgeResultType.Lose)]
    public void DetermineResult_LosePatterns_ReturnsLose(HandType playerHand, HandType opponentHand, JudgeResultType expectedResult)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = RuleSetHelper.DetermineResult(player, opponent);

      Assert.That(result.Type, Is.EqualTo(expectedResult),
                  $"Player: {playerHand} vs Opponent: {opponentHand}");
    }

    [TestCase(HandType.Scissors, true)]
    [TestCase(HandType.One, true)]
    [TestCase(HandType.Three, true)]
    [TestCase(HandType.Rock, false)]
    [TestCase(HandType.Paper, false)]
    [TestCase(HandType.Two, false)]
    [TestCase(HandType.Four, false)]
    public void IsSpecialTriangle_ValidatesCorrectly(HandType handType, bool expectedResult)
    {
      var hand = new Hand(handType, handType.ToString());

      bool result = RuleSetHelper.IsSpecialTriangle(hand);

      Assert.That(result, Is.EqualTo(expectedResult));
    }
  }
}
#endif
