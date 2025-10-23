using System.Collections.Generic;
using JJJ.Core.Entities;

namespace JJJ.Infrastructure
{
  /// <summary>
  /// じゃんけんのルールセットに関するヘルパークラス
  /// </summary>
  internal static class RuleSetHelper
  {
    /// <summary>
    /// じゃんけんの勝利パターンの集合
    /// </summary>
    internal static readonly HashSet<(HandType, HandType)> WinPatterns = new()
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

    /// <summary>
    /// 特殊な三すくみの手かどうかを判定する
    /// </summary>
    /// <param name="hand">手</param>
    /// <returns>特殊な三すくみの手であれば true、そうでなければ false</returns>
    internal static bool IsSpecialTriangle(Hand hand)
    {
      return hand.Type is HandType.Scissors or HandType.One or HandType.Three;
    }

    /// <summary>
    /// じゃんけんの結果を判定する
    /// </summary>
    /// <param name="playerHand">左側のプレイヤーの手</param>
    /// <param name="opponentHand">右側のプレイヤーの手</param>
    /// <returns>判定された結果</returns>
    internal static JudgeResult DetermineResult(Hand playerHand, Hand opponentHand)
    {
      var player = playerHand.Type;
      var opponent = opponentHand.Type;

      // 両者の手が同じ場合は引き分け
      if (player == opponent) return new(JudgeResultType.Draw, playerHand, opponentHand);

      // 勝利パターンに基づいて勝敗を判定
      if (WinPatterns.Contains((player, opponent))) return new(JudgeResultType.Win, playerHand, opponentHand);
      if (WinPatterns.Contains((opponent, player))) return new(JudgeResultType.Lose, playerHand, opponentHand);

      // ここに到達するのは理論上ありえないが、型の安全性のために追加
      return new(JudgeResultType.Draw, playerHand, opponentHand);
    }
  }
}