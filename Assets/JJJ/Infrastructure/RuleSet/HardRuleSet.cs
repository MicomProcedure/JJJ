using System;
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
    /// <exception cref="Exception"></exception>
    public JudgeResult Judge(Hand playerHand, Hand opponentHand, TurnContext turnContext)
    {
      // 両者の手が反則かどうかをチェック
      var playerHandValidation = ValidateHand(playerHand, turnContext, PersonType.Player);
      var opponentHandValidation = ValidateHand(opponentHand, turnContext, PersonType.Opponent);

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
          _ => throw new Exception("Invalid hand detected.")
        };
      }
      else
      {
        bool isDrawConfirmed = false;
        // 特殊なルールの適用
        // Alpha または Beta の手が出された場合は、そのターンを引き分けとする
        if (playerHand.Type == HandType.Alpha)
        {
          turnContext.ActivateAlpha(AlphaDuration, PersonType.Player);
          isDrawConfirmed = true;
        }
        if (opponentHand.Type == HandType.Alpha)
        {
          turnContext.ActivateAlpha(AlphaDuration, PersonType.Opponent);
          isDrawConfirmed = true;
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
          if (playerHand.Type == HandType.Beta)
          {
            turnContext.ActivateBeta(BetaDuration, opponentHand.Type, PersonType.Player);
            isDrawConfirmed = true;
          }
          if (opponentHand.Type == HandType.Beta)
          {
            turnContext.ActivateBeta(BetaDuration, playerHand.Type, PersonType.Opponent);
            isDrawConfirmed = true;
          }
        }

        // 通常のじゃんけんの勝敗を判定
        result = RuleSetHelper.DetermineResult(playerHand, opponentHand);
        if (isDrawConfirmed)
        {
          // AlphaまたはBetaの効果で引き分けが確定している場合は引き分けにする
          result = new(JudgeResultType.Draw, playerHand, opponentHand);
        }
      }

      bool isPlayerWinning = result.Type is JudgeResultType.Win or JudgeResultType.OpponentViolation;
      bool isOpponentWinning = result.Type is JudgeResultType.Lose or JudgeResultType.Violation;
      bool isDraw = result.Type is JudgeResultType.Draw;
      var overrideResultType = result.Type;
      var winByAlpha = (PersonType?)null;

      // PlayerとOpponentの両方がAlphaを使用中で、かつ残りターンが1ターンの場合の特別処理
      if (turnContext.GetAlphaRemainingTurns(PersonType.Player) == 1 && turnContext.GetAlphaRemainingTurns(PersonType.Opponent) == 1)
      {
        if (isPlayerWinning)
        {
          overrideResultType = JudgeResultType.Win;
          winByAlpha = PersonType.Player;
        }
        else if (isOpponentWinning)
        {
          overrideResultType = JudgeResultType.Lose;
          winByAlpha = PersonType.Opponent;
        }
        else
        {
          overrideResultType = JudgeResultType.Draw;
        }
      }
      else if (turnContext.GetAlphaRemainingTurns(PersonType.Player) == 1)
      {
        // PlayerのAlphaが残り1ターンのときの特別処理
        if (isDraw)
        {
          overrideResultType = JudgeResultType.Win;
          winByAlpha = PersonType.Player;
        }
        else if (isPlayerWinning)
        {
          overrideResultType = JudgeResultType.Win;
          winByAlpha = PersonType.Player;
        }
      }
      else if (turnContext.GetAlphaRemainingTurns(PersonType.Opponent) == 1)
      {
        // OpponentのAlphaが残り1ターンのときの特別処理
        if (isDraw)
        {
          overrideResultType = JudgeResultType.Lose;
          winByAlpha = PersonType.Opponent;
        }
        else if (isOpponentWinning)
        {
          overrideResultType = JudgeResultType.Lose;
          winByAlpha = PersonType.Opponent;
        }
      }
      else if (turnContext.GetAlphaRemainingTurns(PersonType.Player) > 0)
      {
        overrideResultType = isPlayerWinning || isDraw ? JudgeResultType.Draw : result.Type;
      }
      else if (turnContext.GetAlphaRemainingTurns(PersonType.Opponent) > 0)
      {
        overrideResultType = isOpponentWinning || isDraw ? JudgeResultType.Draw : result.Type;
      }

      return new(overrideResultType, playerHand, opponentHand, result.PlayerViolationType, result.OpponentViolationType, winByAlpha);
    }

    /// <summary>
    /// じゃんけんの手が反則でないかどうかを判定する
    /// </summary>
    /// <param name="hand">手</param>
    /// <param name="turnContext">現在のターンのコンテキスト</param>
    /// <param name="personType">手を出した人の種類</param>
    /// <returns>判定結果</returns>
    /// <remarks> ハードモードは後出し、Alphaの連続使用、Betaの連続使用、封印された手の使用が反則</remarks>
    public HandValidationResult ValidateHand(Hand hand, TurnContext turnContext, PersonType personType)
    {
      // 後出しの判定
      if (hand.IsTimeout) return new(false, ViolationType.Timeout);
      // Alphaの連続使用
      if (hand.Type == HandType.Alpha)
      {
        if (personType == PersonType.Player && turnContext.GetAlphaRemainingTurns(PersonType.Player) > 0) return new(false, ViolationType.AlphaRepeat);
        if (personType == PersonType.Opponent && turnContext.GetAlphaRemainingTurns(PersonType.Opponent) > 0) return new(false, ViolationType.AlphaRepeat);
      }
      // Betaの連続使用
      if (hand.Type == HandType.Beta)
      {
        if (personType == PersonType.Player && turnContext.GetBetaRemainingTurns(PersonType.Player) > 0) return new(false, ViolationType.BetaRepeat);
        if (personType == PersonType.Opponent && turnContext.GetBetaRemainingTurns(PersonType.Opponent) > 0) return new(false, ViolationType.BetaRepeat);
      }
      // 封印された手の使用
      if (hand.Type == turnContext.GetSealedHandType(PersonType.Player) || hand.Type == turnContext.GetSealedHandType(PersonType.Opponent)) return new(false, ViolationType.SealedHandUsed);

      // 反則でない場合
      return new(true, ViolationType.None);
    }
  }
}
