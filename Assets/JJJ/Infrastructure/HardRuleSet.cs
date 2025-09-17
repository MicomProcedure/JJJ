using JJJ.Core.Entities;
using JJJ.Core.Interfaces;

namespace JJJ.Infrastructure
{
  /// <summary>
  /// ハードモードのルールセットを実装するクラス
  /// </summary>
  public class HardRuleSet : IRuleSet
  {
    /// <summary>
    /// じゃんけんの結果を判定する
    /// </summary>
    /// <param name="playerHand">プレイヤーの手</param>
    /// <param name="opponentHand">対戦相手の手</param>
    /// <param name="turnContext">ターン情報のコンテキスト</param>
    /// <returns>じゃんけんの結果</returns>
    /// <exception cref="System.Exception"></exception>
    public JudgeResult Judge(Hand playerHand, Hand opponentHand, TurnContext turnContext)
    {
      // 両者の手が反則かどうかをチェック
      var playerHandValidation = ValidateHand(playerHand, turnContext);
      var opponentHandValidation = ValidateHand(opponentHand, turnContext);

      // どちらかまたは両方が反則の場合の処理
      if (!playerHandValidation.IsValid || !opponentHandValidation.IsValid)
      {
        // どちらが反則かに応じて結果を返す
        return (playerHandValidation.IsValid, opponentHandValidation.IsValid) switch
        {
          (false, false) => new(JudgeResultType.DoubleViolation, playerHand, opponentHand, playerHandValidation.ViolationType, opponentHandValidation.ViolationType),
          (false, true) => new(JudgeResultType.Violation, playerHand, opponentHand, playerHandValidation.ViolationType, opponentHandValidation.ViolationType),
          (true, false) => new(JudgeResultType.OpponentViolation, playerHand, opponentHand, playerHandValidation.ViolationType, opponentHandValidation.ViolationType),
          // 理論上ありえないが、型の安全性のために追加
          _ => throw new System.Exception("Invalid hand detected.")
        };
      }

      // 特殊なルールの適用
      // Alpha または Beta の手が出された場合は、そのターンを引き分けとする
      if (playerHand.Type == HandType.Alpha || opponentHand.Type == HandType.Alpha) return new(JudgeResultType.Draw, playerHand, opponentHand);
      if (playerHand.Type == HandType.Beta || opponentHand.Type == HandType.Beta) return new(JudgeResultType.Draw, playerHand, opponentHand);

      // 両者が特殊な三すくみの手を出し、かつ偶数ターンの場合は特別な勝敗ルールを適用
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

      // 通常のじゃんけんの勝敗を判定
      return RuleSetHelper.DetermineResult(playerHand, opponentHand);
    }

    /// <summary>
    /// じゃんけんの手が反則でないかどうかを判定する
    /// </summary>
    /// <param name="hand">手</param>
    /// <param name="turnContext">現在のターンのコンテキスト</param>
    /// <returns>判定結果</returns>
    /// <remarks> ハードモードは後出し、Alphaの連続使用、Betaの連続使用、封印された手の使用が反則</remarks>
    public HandValidationResult ValidateHand(Hand hand, TurnContext turnContext)
    {
      // 後出しの判定
      if (hand.IsTimeout) return new(false, ViolationType.Timeout);
      // Alphaの連続使用
      if (hand.Type == HandType.Alpha && turnContext.AlphaRemainingTurns > 0) return new(false, ViolationType.AlphaRepeat);
      // Betaの連続使用
      if (hand.Type == HandType.Beta && turnContext.BetaRemainingTurns > 0) return new(false, ViolationType.BetaRepeat);
      // 封印された手の使用
      if (hand.Type == turnContext.SealedHandType) return new(false, ViolationType.SealedHandUsed);

      // 反則でない場合
      return new(true, ViolationType.None);
    }
  }
}
