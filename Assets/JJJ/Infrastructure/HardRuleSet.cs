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

      if (playerHand.Type == HandType.Alpha || opponentHand.Type == HandType.Alpha) return new(JudgeResultType.Draw, playerHand, opponentHand);
      if (playerHand.Type == HandType.Beta || opponentHand.Type == HandType.Beta) return new(JudgeResultType.Draw, playerHand, opponentHand);

      if (turnContext.IsEvenTurn && RuleSetHelper.IsSpecialTriangle(playerHand) && RuleSetHelper.IsSpecialTriangle(opponentHand))
      {
        return (playerHand.Type, opponentHand.Type) switch
        {
          (HandType.One, HandType.Three) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Three, HandType.Scissors) => new(JudgeResultType.Win, playerHand, opponentHand),
          (HandType.Scissors, HandType.One) => new(JudgeResultType.Win, playerHand, opponentHand),

          (HandType.Three, HandType.One) => new(JudgeResultType.Lose, playerHand, opponentHand),
          (HandType.Scissors, HandType.Three) => new(JudgeResultType.Lose, playerHand, opponentHand),
          (HandType.One, HandType.Scissors) => new(JudgeResultType.Lose, playerHand, opponentHand),

          _ => new(JudgeResultType.Draw, playerHand, opponentHand)
        };
      }

      return RuleSetHelper.DetermineResult(playerHand, opponentHand);
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
