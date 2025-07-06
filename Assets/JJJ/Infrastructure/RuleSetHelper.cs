using System.Collections.Generic;
using JJJ.Core.Entities;

namespace JJJ.Infrastructure
{
  internal static class RuleSetHelper
  {
    private static readonly HashSet<(HandType, HandType)> WinPatterns = new()
    {
      (HandType.Rock, HandType.Scissors),
      (HandType.Rock, HandType.Three),
      (HandType.Scissors, HandType.Paper),
      (HandType.Scissors, HandType.Three),
      (HandType.Paper, HandType.Rock),
      (HandType.Paper, HandType.Three),
      (HandType.One, HandType.Scissors),
      (HandType.One, HandType.Two),
      (HandType.Two, HandType.Rock),
      (HandType.Two, HandType.Paper),
      (HandType.Three, HandType.One),
      (HandType.Three, HandType.Two),
      (HandType.Three, HandType.Four),
      (HandType.Four, HandType.One),
    };

    internal static bool IsSpecialTriangle(Hand hand)
    {
      return hand.Type is HandType.Scissors or HandType.One or HandType.Three;
    }

    internal static JudgeResult DetermineResult(Hand playerHand, Hand opponentHand)
    {
      var player = playerHand.Type;
      var opponent = opponentHand.Type;

      if (player == opponent) return new(JudgeResultType.Draw, playerHand, opponentHand);

      if (WinPatterns.Contains((player, opponent))) return new(JudgeResultType.Win, playerHand, opponentHand);

      if (WinPatterns.Contains((opponent, player))) return new(JudgeResultType.Lose, playerHand, opponentHand);

      return new(JudgeResultType.Draw, playerHand, opponentHand);
    }
  }
}