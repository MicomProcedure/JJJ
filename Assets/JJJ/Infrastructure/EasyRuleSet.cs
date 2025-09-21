using JJJ.Core.Entities;
using JJJ.Core.Interfaces;

namespace JJJ.Infrastructure
{
  /// <summary>
  /// イージーモードのRuleSet実装
  /// </summary>
  public class EasyRuleSet : IRuleSet
  {
    /// <summary>
    /// じゃんけんの結果を判定する
    /// </summary>
    /// <param name="playerHand">左側のプレイヤーの手</param>
    /// <param name="opponentHand">右側のプレイヤーの手</param>
    /// <param name="turnContext">現在のターンのコンテキスト</param>
    /// <returns>判定された結果</returns>
    public JudgeResult Judge(Hand playerHand, Hand opponentHand, TurnContext turnContext)
    {
      // 両者の手が反則かどうかをチェック
      var playerHandValidation = ValidateHand(playerHand, turnContext);
      var opponentHandValidation = ValidateHand(opponentHand, turnContext);

      // どちらかまたは両方が反則の場合の処理
      if (!playerHandValidation.IsValid || !opponentHandValidation.IsValid)
      {
        return (playerHandValidation.IsValid, opponentHandValidation.IsValid) switch
        {
          (false, false) => new(JudgeResultType.DoubleViolation, playerHand, opponentHand, ViolationType.Timeout, ViolationType.Timeout),
          (false, true) => new(JudgeResultType.Violation, playerHand, opponentHand, ViolationType.Timeout, ViolationType.None),
          (true, false) => new(JudgeResultType.OpponentViolation, playerHand, opponentHand, ViolationType.None, ViolationType.Timeout),
          // 理論上ありえないが、型の安全性のために追加
          _ => throw new System.Exception("Invalid hand detected.")
        };
      }

      // 両者の手が同じ場合は引き分け
      if (playerHand.Type == opponentHand.Type) return new(JudgeResultType.Draw, playerHand, opponentHand);

      // 通常のじゃんけんの勝敗判定
      return (playerHand.Type, opponentHand.Type) switch
      {
        (HandType.Rock, HandType.Scissors) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Scissors, HandType.Paper) => new(JudgeResultType.Win, playerHand, opponentHand),
        (HandType.Paper, HandType.Rock) => new(JudgeResultType.Win, playerHand, opponentHand),
        _ => new(JudgeResultType.Lose, playerHand, opponentHand)
      };
    }

    /// <summary>
    /// じゃんけんの手が反則でないかどうかを判定する
    /// </summary>
    /// <param name="hand">手</param>
    /// <param name="turnContext">現在のターンのコンテキスト</param>
    /// <returns>判定結果</returns>
    /// <remarks>イージーモードは後出しのみが反則</remarks>
    public HandValidationResult ValidateHand(Hand hand, TurnContext turnContext)
    {
      return hand.IsTimeout ? new(false, ViolationType.Timeout) : new(true, ViolationType.None);
    }
  }
}
