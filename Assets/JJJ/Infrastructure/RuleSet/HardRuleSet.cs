using JJJ.Core.Entities;
using JJJ.Core.Interfaces;

namespace JJJ.Infrastructure
{
  /// <summary>
  /// ハードモードのルールセットを実装するクラス
  /// </summary>
  public class HardRuleSet : IRuleSet
  {
    private const int AlphaDuration = 3;
    private const int BetaDuration = 3;

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

      JudgeResult result;

      // どちらかまたは両方が反則の場合の処理
      if (!playerHandValidation.IsValid || !opponentHandValidation.IsValid)
      {
        // どちらが反則かに応じて結果を返す
        result = (playerHandValidation.IsValid, opponentHandValidation.IsValid) switch
        {
          (false, false) => new(JudgeResultType.DoubleViolation, playerHand, opponentHand, playerHandValidation.ViolationType, opponentHandValidation.ViolationType),
          (false, true) => new(JudgeResultType.Violation, playerHand, opponentHand, playerHandValidation.ViolationType, opponentHandValidation.ViolationType),
          (true, false) => new(JudgeResultType.OpponentViolation, playerHand, opponentHand, playerHandValidation.ViolationType, opponentHandValidation.ViolationType),
          // 理論上ありえないが、型の安全性のために追加
          _ => throw new System.Exception("Invalid hand detected.")
        };
      }
      else
      {
        // 特殊なルールの適用
        // Alpha または Beta の手が出された場合は、そのターンを引き分けとする
        if (playerHand.Type == HandType.Alpha || opponentHand.Type == HandType.Alpha)
        {
          turnContext.ActivateAlpha(AlphaDuration, playerHand.Type == HandType.Alpha ? PersonType.Player : PersonType.Opponent);
          return new(JudgeResultType.Draw, playerHand, opponentHand);
        }
        if (playerHand.Type == HandType.Beta || opponentHand.Type == HandType.Beta)
        {
          // BetaはThreeには負けるので、ActivateBetaの前に判定
          if (playerHand.Type == HandType.Beta && opponentHand.Type == HandType.Three)
          {
            return new(JudgeResultType.Lose, playerHand, opponentHand);
          }
          else if (opponentHand.Type == HandType.Beta && playerHand.Type == HandType.Three)
          {
            return new(JudgeResultType.Win, playerHand, opponentHand);
          }
          // Betaを発動させる
          // Betaを出した人がPlayerならOpponentの手を、OpponentならPlayerの手を封印する
          bool isPlayerHandIsBeta = playerHand.Type == HandType.Beta;
          turnContext.ActivateBeta(BetaDuration, isPlayerHandIsBeta ? opponentHand.Type : playerHand.Type, isPlayerHandIsBeta ? PersonType.Player : PersonType.Opponent);
          return new(JudgeResultType.Draw, playerHand, opponentHand);
        }

        // 両者が特殊な三すくみの手を出し、かつ偶数ターンの場合は特別な勝敗ルールを適用
        if (turnContext.IsEvenTurn && RuleSetHelper.IsSpecialTriangle(playerHand) && RuleSetHelper.IsSpecialTriangle(opponentHand))
        {
          result = (playerHand.Type, opponentHand.Type) switch
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
        else
        {
          // 通常のじゃんけんの勝敗を判定
          result = RuleSetHelper.DetermineResult(playerHand, opponentHand);
        }
      }

      // Alphaの効果を適用
      if (turnContext.AlphaRemainingTurns > 0)
      {
        // Alphaを発動させた人が勝ちまたは引き分けなら、引き分けにする
        // Alphaを発動させた人が負けなら、そのまま負けにする
        var overrideResultType = result.Type;
        PersonType? winByAlpha = null;
        bool isPlayerWinning = result.Type is JudgeResultType.Win or JudgeResultType.OpponentViolation;
        bool isOpponentWinning = result.Type is JudgeResultType.Lose or JudgeResultType.Violation;
        bool isDraw = result.Type is JudgeResultType.Draw or JudgeResultType.DoubleViolation;
        if (turnContext.AlphaActivatedBy == PersonType.Player && (isPlayerWinning || isDraw))
        {
          // Alphaの残りターンが1ターンのときにあいこの場合は勝ちにする
          overrideResultType = turnContext.AlphaRemainingTurns == 1 ?
              isDraw ? JudgeResultType.Win : result.Type
            : isDraw ? result.Type : JudgeResultType.Draw;
          if (turnContext.AlphaRemainingTurns == 1)
          {
            winByAlpha = PersonType.Player;
          }
        }
        else if (turnContext.AlphaActivatedBy == PersonType.Opponent && (isOpponentWinning || isDraw))
        {
          // Alphaの残りターンが1ターンのときにあいこの場合は負けにする
          overrideResultType = turnContext.AlphaRemainingTurns == 1 ?
              isDraw ? JudgeResultType.Lose : result.Type
            : isDraw ? result.Type : JudgeResultType.Draw;
          if (turnContext.AlphaRemainingTurns == 1)
          {
            winByAlpha = PersonType.Opponent;
          }
        }

        // 上書きした結果を返す
        return new(overrideResultType, playerHand, opponentHand, result.PlayerViolationType, result.OpponentViolationType, winByAlpha);
      }
      return result;
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
