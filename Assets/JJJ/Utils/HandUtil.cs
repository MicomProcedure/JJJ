using System.Collections.Generic;
using System.Linq;
using JJJ.Core.Entities;

namespace JJJ.Utils
{
  public static class HandUtil
  {
    /// <summary>
    /// 基本的な手の種類
    /// グー・チョキ・パーの3種類
    /// </summary>
    public static readonly HandType[] BasicHandTypes =
    {
      HandType.Rock, HandType.Scissors, HandType.Paper
    };

    /// <summary>
    /// 数字の手の種類
    /// 1・2・3・4の4種類
    /// </summary>
    public static readonly HandType[] NumberHandTypes =
    {
      HandType.One, HandType.Two, HandType.Three, HandType.Four
    };

    /// <summary>
    /// 特殊な手の種類
    /// Alpha/Betaの2種類
    /// </summary>
    public static readonly HandType[] SpecialHandTypes =
    {
      HandType.Alpha, HandType.Beta
    };

    /// <summary>
    /// 勝敗がターンの偶奇で変わる特殊な手の種類
    /// 特別な三角形（チョキ・1・3）の3種類
    /// </summary>
    public static readonly HandType[] SpecialTriangleHandTypes =
    {
      HandType.Scissors, HandType.One, HandType.Three
    };

    /// <summary>
    /// 全ての手の種類
    /// 基本的な手、数字の手、特殊な手を全て含む
    /// </summary>
    public static readonly HandType[] AllHandTypes =
      BasicHandTypes.Concat(NumberHandTypes).Concat(SpecialHandTypes).ToArray();

    /// <summary>
    /// Normalのじゃんけんで使用する手の種類（Alpha/Beta除く）
    /// </summary>
    public static readonly HandType[] RegularHandTypes =
      BasicHandTypes.Concat(NumberHandTypes).ToArray();

    /// <summary>
    /// 全ての手の組み合わせを生成
    /// </summary>
    public static IEnumerable<(HandType player, HandType opponent)> GetAllHandCombinations(HandType[] handTypes = default)
    {
      handTypes = handTypes ?? AllHandTypes;
      return handTypes.SelectMany(p => handTypes.Select(o => (p, o)));
    }

    /// <summary>
    /// 勝利パターンの手の組み合わせを定義
    /// </summary>
    public static readonly IEnumerable<(HandType player, HandType opponent)> WinPatterns = new[]
      {
        (HandType.Rock, HandType.Scissors), (HandType.Rock, HandType.Three),
        (HandType.Scissors, HandType.Paper), (HandType.Scissors, HandType.Three),
        (HandType.Paper, HandType.Rock), (HandType.Paper, HandType.Three),
        (HandType.One, HandType.Scissors), (HandType.One, HandType.Two),
        (HandType.Two, HandType.Rock), (HandType.Two, HandType.Paper),
        (HandType.Three, HandType.One), (HandType.Three, HandType.Two),
        (HandType.Three, HandType.Four), (HandType.Four, HandType.One)
      };

    /// <summary>
    /// 敗北パターンの手の組み合わせを定義
    /// </summary>
    public static readonly IEnumerable<(HandType player, HandType opponent)> LosePatterns =
      WinPatterns.Select(p => (p.opponent, p.player));

    /// <summary>
    /// 引き分けパターンの手の組み合わせを定義
    /// 全ての手の組み合わせから勝利パターンと敗北パターンを除外したもの
    /// </summary>
    public static readonly IEnumerable<(HandType player, HandType opponent)> DrawPatterns =
      GetAllHandCombinations(RegularHandTypes)
        .Where(combo => !WinPatterns.Contains(combo) && !LosePatterns.Contains(combo));

    /// <summary>
    /// 勝敗がターンの偶奇で変わる特殊な三角形の手の組み合わせを定義
    /// </summary>
    public static readonly IEnumerable<(HandType player, HandType opponent)> SpecialTrianglePatterns =
      GetAllHandCombinations()
        .Where(combo => SpecialTriangleHandTypes.Contains(combo.player) && SpecialTriangleHandTypes.Contains(combo.opponent))
        .Where(combo => combo.player != combo.opponent); // 同じ手は除外

    /// <summary>
    /// ゲームモードに基づいて使用可能な手の種類を取得
    /// </summary>
    /// <param name="gameMode">ゲームモード</param>
    /// <returns>使用可能な手の種類の列挙</returns>
    public static IEnumerable<HandType> GetAvailableHandTypesFromGameMode(GameMode gameMode)
    {
      return gameMode switch
      {
        GameMode.Easy => BasicHandTypes.AsEnumerable(),
        GameMode.Normal => RegularHandTypes.AsEnumerable(),
        GameMode.Hard => AllHandTypes.AsEnumerable(),
        _ => throw new System.ArgumentOutOfRangeException(nameof(gameMode), $"Unsupported game mode: {gameMode}"),
      };
    }

    /// <summary>
    /// コンテキストに基づいて使用可能な手の種類を取得
    /// </summary>
    /// <param name="context">ターンのコンテキスト</param>
    /// <returns>使用可能な手の種類の列挙</returns>
    public static IEnumerable<HandType> GetValidHandTypesFromContext(
        GameMode gameMode,
        TurnContext context
      )
    {
      if (gameMode == GameMode.Easy)
      {
        return BasicHandTypes.AsEnumerable();
      }
      else if (gameMode == GameMode.Normal)
      {
        return RegularHandTypes.AsEnumerable();
      }
      else
      {
        var availableHandTypes = AllHandTypes.ToList();

        if (context.AlphaRemainingTurns > 0) // αが発動中
        {
          availableHandTypes.Remove(HandType.Alpha);
        }
        if (context.BetaRemainingTurns > 0) // βが発動中
        {
          availableHandTypes.Remove(HandType.Beta);
          if (context.SealedHandType.HasValue)
          {
            availableHandTypes.Remove(context.SealedHandType.Value);
          }
        }

        return availableHandTypes.AsEnumerable();
      }
    }

  }
}