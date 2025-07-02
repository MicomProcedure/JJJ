using JJJ.Core.Entities;
using JJJ.Core.Interfaces;

namespace JJJ.Infrastructure
{
  public class NormalRuleSet : IRuleSet
  {
    public JudgeResult Judge(Hand playerHand, Hand opponentHand, TurnContext turnContext)
    {
      var playerHandValidation = ValidateHand(playerHand, turnContext);
      var opponentHandValidation = ValidateHand(opponentHand, turnContext);

      if (!playerHandValidation.IsValid || !opponentHandValidation.IsValid)
      {
        return (playerHandValidation.IsValid, opponentHandValidation.IsValid) switch
        {
          (false, false) => new(JudgeResultType.DoubleViolation, playerHand, opponentHand, ViolationType.Timeout, ViolationType.Timeout),
          (false, true) => new(JudgeResultType.Violation, playerHand, opponentHand, ViolationType.Timeout, ViolationType.None),
          (true, false) => new(JudgeResultType.Win, playerHand, opponentHand, ViolationType.None, ViolationType.Timeout),
          _ => throw new System.Exception("Invalid hand detected.")
        };
      }

      if (playerHand.Type == opponentHand.Type) return new(JudgeResultType.Draw, playerHand, opponentHand);

      return (playerHand.Type, opponentHand.Type) switch
      {
        (HandType.Rock, HandType.Scissors) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Rock, HandType.Three) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Scissors, HandType.Paper) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Scissors, HandType.Three) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Paper, HandType.Rock) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Paper, HandType.Three) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.One, HandType.Scissors) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.One, HandType.Two) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Two, HandType.Rock) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Two, HandType.Paper) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Three, HandType.One) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Three, HandType.Two) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Three, HandType.Four) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Four, HandType.One) => new(JudgeResultType.Win, playerHand, opponentHand),
        _ => new(JudgeResultType.Lose, playerHand, opponentHand)
      };
    }

    public HandValidationResult ValidateHand(Hand hand, TurnContext turnContext)
    {
      return hand.IsTimeout ? new(false, ViolationType.Timeout) : new(true, ViolationType.None);
    }
  }
}
