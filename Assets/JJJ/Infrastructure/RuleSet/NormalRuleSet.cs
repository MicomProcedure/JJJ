using JJJ.Core.Entities;
using JJJ.Core.Interfaces;

namespace JJJ.Infrastructure
{
  /// <summary>
  /// ノーマルモードのRuleSet実装
  /// </summary>
  public class NormalRuleSet : IRuleSet
  {
    /// <summary>
    /// じゃんけんの結果を判定する
    /// </summary>
    /// <param name="playerHand">左側のプレイヤーの手</param>
    /// <param name="opponentHand">右側のプレイヤーの手</param>
    /// <param name="turnContext">現在のターンのコンテキスト</param>
    /// <returns>判定された結果</returns>
    /// <exception cref="System.Exception"></exception>
    public JudgeResult Judge(Hand playerHand, Hand opponentHand, TurnContext turnContext)
    {
      // 両者の手が反則かどうかをチェック
      var playerHandValidation = ValidateHand(playerHand, turnContext, PersonType.Player);
      var opponentHandValidation = ValidateHand(opponentHand, turnContext, PersonType.Opponent);

      // どちらかまたは両方が反則の場合の処理
      if (!playerHandValidation.IsValid || !opponentHandValidation.IsValid)
      {
        // どちらが反則かに応じて結果を返す
        return (playerHandValidation.IsValid, opponentHandValidation.IsValid) switch
        {
          (false, false) => new(JudgeResultType.DoubleViolation, playerHand, opponentHand, ViolationType.Timeout, ViolationType.Timeout),
          (false, true) => new(JudgeResultType.Violation, playerHand, opponentHand, ViolationType.Timeout, ViolationType.None),
          (true, false) => new(JudgeResultType.OpponentViolation, playerHand, opponentHand, ViolationType.None, ViolationType.Timeout),
          // 理論上ありえないが、型の安全性のために追加
          _ => throw new System.Exception("Invalid hand detected.")
        };
      }

      // 両者の手が反則でない場合は、RuleSetHelperを使って通常のじゃんけんの勝敗を判定
      return RuleSetHelper.DetermineResult(playerHand, opponentHand);
    }

    /// <summary>
    /// じゃんけんの手が反則でないかどうかを判定する
    /// </summary>
    /// <param name="hand">手</param>
    /// <param name="turnContext">現在のターンのコンテキスト</param>
    /// <returns>判定結果</returns>
    /// <remarks>ノーマルモードは後出しのみが反則</remarks>
    public HandValidationResult ValidateHand(Hand hand, TurnContext turnContext, PersonType personType)
    {
      return hand.IsTimeout ? new(false, ViolationType.Timeout) : new(true, ViolationType.None);
    }
  }
}
