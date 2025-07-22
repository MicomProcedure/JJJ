#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
namespace JJJ.Tests.Infrastructure
{
  using JJJ.Infrastructure;
  using JJJ.Core.Entities;
  using NUnit.Framework;

  public class EasyRuleSetTest
  {
    private readonly EasyRuleSet _easyRuleSet = new();
    private readonly TurnContext _turnContext = new();

    [TestCase(HandType.Rock, HandType.Scissors, JudgeResultType.Win)]
    [TestCase(HandType.Scissors, HandType.Paper, JudgeResultType.Win)]
    [TestCase(HandType.Paper, HandType.Rock, JudgeResultType.Win)]
    [TestCase(HandType.Scissors, HandType.Rock, JudgeResultType.Lose)]
    [TestCase(HandType.Paper, HandType.Scissors, JudgeResultType.Lose)]
    [TestCase(HandType.Rock, HandType.Paper, JudgeResultType.Lose)]
    [TestCase(HandType.Rock, HandType.Rock, JudgeResultType.Draw)]
    [TestCase(HandType.Scissors, HandType.Scissors, JudgeResultType.Draw)]
    [TestCase(HandType.Paper, HandType.Paper, JudgeResultType.Draw)]
    public void EasyRuleSet_TestWithoutTimeout(HandType playerHand, HandType opponentHand, JudgeResultType expectedResultType)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = _easyRuleSet.Judge(player, opponent, _turnContext);
      Assert.That(result.Type == expectedResultType,
                    $"Expected {expectedResultType}, but got {result.Type} for player hand {playerHand} and opponent hand {opponentHand}.");
    }

    [TestCase(HandType.Rock, HandType.Scissors, JudgeResultType.Violation)]
    [TestCase(HandType.Scissors, HandType.Paper, JudgeResultType.Violation)]
    [TestCase(HandType.Paper, HandType.Rock, JudgeResultType.Violation)]
    [TestCase(HandType.Rock, HandType.Rock, JudgeResultType.Violation)]
    [TestCase(HandType.Scissors, HandType.Scissors, JudgeResultType.Violation)]
    [TestCase(HandType.Paper, HandType.Paper, JudgeResultType.Violation)]
    public void EasyRuleSet_TestWithPlayerTimeout(HandType playerHand, HandType opponentHand, JudgeResultType expectedResultType)
    {
      var player = new Hand(playerHand, playerHand.ToString(), isTimeout: true);
      var opponent = new Hand(opponentHand, opponentHand.ToString());

      var result = _easyRuleSet.Judge(player, opponent, _turnContext);
      Assert.That(result.Type == expectedResultType,
                    $"Expected {expectedResultType}, but got {result.Type} for player hand {playerHand} and opponent hand {opponentHand}.");
    }
  }
}
#endif