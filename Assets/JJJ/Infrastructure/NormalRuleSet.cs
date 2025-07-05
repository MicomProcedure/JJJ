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

      return RuleSetHelper.DetermineResult(playerHand, opponentHand);
    }

    public HandValidationResult ValidateHand(Hand hand, TurnContext turnContext)
    {
      return hand.IsTimeout ? new(false, ViolationType.Timeout) : new(true, ViolationType.None);
    }
  }
}
