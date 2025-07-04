using JJJ.Core.Entities;
using JJJ.Core.Interfaces;

namespace JJJ.Infrastructure
{
  public class HardRuleSet : IRuleSet
  {
    public JudgeResult Judge(Hand playerHand, Hand opponentHand, TurnContext turnContext)
    {
      var playerHandValidation = ValidateHand(playerHand, turnContext);
      var opponentHandValidation = ValidateHand(opponentHand, turnContext);

      if (!playerHandValidation.IsValid || !opponentHandValidation.IsValid)
      {
        return (playerHandValidation.IsValid, opponentHandValidation.IsValid) switch
        {
          (false, false) => new(JudgeResultType.DoubleViolation, playerHand, opponentHand, playerHandValidation.ViolationType, opponentHandValidation.ViolationType),
          (false, true) => new(JudgeResultType.Violation, playerHand, opponentHand, playerHandValidation.ViolationType, opponentHandValidation.ViolationType),
          (true, false) => new(JudgeResultType.Win, playerHand, opponentHand, playerHandValidation.ViolationType, opponentHandValidation.ViolationType),
          _ => throw new System.Exception("Invalid hand detected.")
        };
      }

      if (playerHand.Type == opponentHand.Type) return new(JudgeResultType.Draw, playerHand, opponentHand);
      if (playerHand.Type == HandType.Alpha || opponentHand.Type == HandType.Alpha) return new(JudgeResultType.Draw, playerHand, opponentHand);
      if (playerHand.Type == HandType.Beta || opponentHand.Type == HandType.Beta) return new(JudgeResultType.Draw, playerHand, opponentHand);

      if (turnContext.IsEvenTurn)
      {
        return (playerHand.Type, opponentHand.Type) switch
        {
          (HandType.Rock, HandType.Scissors) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Rock, HandType.Three) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Scissors, HandType.Paper) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Scissors, HandType.One) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Paper, HandType.Rock) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Paper, HandType.Three) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.One, HandType.Two) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.One, HandType.Three) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Two, HandType.Rock) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Two, HandType.Paper) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Three, HandType.Scissors) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Three, HandType.Two) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Three, HandType.Four) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Four, HandType.One) => new(JudgeResultType.Win, playerHand, opponentHand),
          _ => new(JudgeResultType.Lose, playerHand, opponentHand)
        };
      }
      else
      {
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
    }

    public HandValidationResult ValidateHand(Hand hand, TurnContext turnContext)
    {
      if (hand.IsTimeout) return new(false, ViolationType.Timeout);
      if (hand.Type == HandType.Alpha && turnContext.AlphaRemainingTurns > 0) return new(false, ViolationType.AlphaRepeat);
      if (hand.Type == HandType.Beta && turnContext.BetaRemainingTurns > 0) return new(false, ViolationType.BetaRepeat);
      if (hand.Type == turnContext.SealedHandType) return new(false, ViolationType.SealedHandUsed);

      return new(true, ViolationType.None);
    }
  }
}
