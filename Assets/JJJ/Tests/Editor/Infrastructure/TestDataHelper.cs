#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
using JJJ.Infrastructure;
using JJJ.Core.Entities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
namespace JJJ.Tests.Infrastructure
{
  /// <summary>
  /// テストデータ生成のヘルパークラス
  /// 各テストクラス間での重複コードを削減
  /// </summary>
  public static class TestDataHelper
  {
    // 基本的な手の種類
    public static readonly HandType[] BasicHandTypes = 
    {
      HandType.Rock, HandType.Scissors, HandType.Paper
    };

    // 数字の手の種類
    public static readonly HandType[] NumberHandTypes = 
    {
      HandType.One, HandType.Two, HandType.Three, HandType.Four
    };

    // 特殊な手の種類
    public static readonly HandType[] SpecialHandTypes = 
    {
      HandType.Alpha, HandType.Beta
    };

    // 全ての手の種類
    public static readonly HandType[] AllHandTypes = 
      BasicHandTypes.Concat(NumberHandTypes).Concat(SpecialHandTypes).ToArray();

    // 通常のじゃんけんで使用する手の種類（Alpha/Beta除く）
    public static readonly HandType[] RegularHandTypes = 
      BasicHandTypes.Concat(NumberHandTypes).ToArray();

    /// <summary>
    /// 全ての手の組み合わせを生成
    /// </summary>
    public static IEnumerable<(HandType player, HandType opponent)> GetAllHandCombinations(HandType[] handTypes = null)
    {
      handTypes ??= AllHandTypes;
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
    /// 通常ルールでの勝利パターンを生成
    /// </summary>
    public static IEnumerable<TestCaseData> GetWinPatternTestCases()
    {
      foreach (var (player, opponent) in WinPatterns)
      {
        yield return new TestCaseData(player, opponent, JudgeResultType.Win)
          .SetName($"{player}_vs_{opponent}_Win");
      }
    }

    /// <summary>
    /// 引き分けパターンを生成
    /// </summary>
    public static IEnumerable<TestCaseData> GetDrawPatternTestCases()
    {
      foreach (var (player, opponent) in DrawPatterns)
      {
        yield return new TestCaseData(player, opponent, JudgeResultType.Draw)
          .SetName($"{player}_vs_{opponent}_Draw");
      }
    }

    /// <summary>
    /// 敗北パターンを生成
    /// </summary>
    public static IEnumerable<TestCaseData> GetLosePatternTestCases()
    {
      foreach (var (player, opponent) in LosePatterns)
      {
        yield return new TestCaseData(player, opponent, JudgeResultType.Lose)
          .SetName($"{player}_vs_{opponent}_Lose");
      }
    }

    /// <summary>
    /// 通常ルールでの全ての判定パターンを生成
    /// Alpha, Betaを除く
    /// </summary>
    public static IEnumerable<TestCaseData> GetNormalJudgeTestCases()
    {
      foreach (var (player, opponent) in GetAllHandCombinations(RegularHandTypes))
      {
        var expectedResult = DetermineExpectedResult(player, opponent);
        yield return new TestCaseData(player, opponent, expectedResult)
          .SetName($"{player}_vs_{opponent}_ExpectedResult_{expectedResult}");
      }
    }

    /// <summary>
    /// タイムアウトテストケースを生成
    /// </summary>
    public static IEnumerable<TestCaseData> GetTimeoutTestCases(HandType[] handTypes = null)
    {
      return GetAllHandCombinations(handTypes ?? RegularHandTypes)
        .Select(combo =>
          new TestCaseData(combo.player, combo.opponent).SetName($"Timeout_{combo.player}_vs_{combo.opponent}")
        );
    }

    /// <summary>
    /// RuleSetHelperを使用して期待結果を決定
    /// </summary>
    public static JudgeResultType DetermineExpectedResult(HandType playerHand, HandType opponentHand)
    {
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      return RuleSetHelper.DetermineResult(player, opponent).Type;
    }

    /// <summary>
    /// 特別な三角形テストケースを生成
    /// </summary>
    public static IEnumerable<TestCaseData> GetSpecialTriangleTestCases()
    {
      var testCases = new[]
      {
        (HandType.Scissors, true), (HandType.One, true), (HandType.Three, true),
        (HandType.Rock, false), (HandType.Paper, false), (HandType.Two, false), (HandType.Four, false)
      };

      foreach (var (handType, expected) in testCases)
      {
        yield return new TestCaseData(handType, expected)
          .SetName($"{handType}_IsSpecialTriangle_{expected}");
      }
    }

    /// <summary>
    /// Alpha/Betaの引き分けテストケースを生成
    /// </summary>
    public static IEnumerable<TestCaseData> GetAlphaBetaDrawTestCases()
    {
      var testCases = new[]
      {
        (HandType.Alpha, HandType.Rock), (HandType.Rock, HandType.Alpha),
        (HandType.Beta, HandType.Scissors), (HandType.Paper, HandType.Beta),
        (HandType.Alpha, HandType.Beta)
      };

      foreach (var (player, opponent) in testCases)
      {
        yield return new TestCaseData(player, opponent, JudgeResultType.Draw)
          .SetName($"{player}_vs_{opponent}_AlphaBeta_Draw");
      }
    }

    /// <summary>
    /// 偶数ターン特殊三角形テストケースを生成
    /// </summary>
    public static IEnumerable<TestCaseData> GetEvenTurnSpecialTriangleTestCases()
    {
      var testCases = new[]
      {
        (HandType.One, HandType.Three, JudgeResultType.Win),
        (HandType.Three, HandType.Scissors, JudgeResultType.Win),
        (HandType.Scissors, HandType.One, JudgeResultType.Win),
        (HandType.Three, HandType.One, JudgeResultType.Lose),
        (HandType.Scissors, HandType.Three, JudgeResultType.Lose),
        (HandType.One, HandType.Scissors, JudgeResultType.Lose)
      };

      foreach (var (player, opponent, expected) in testCases)
      {
        yield return new TestCaseData(player, opponent, expected)
          .SetName($"EvenTurn_{player}_vs_{opponent}_{expected}");
      }
    }

    /// <summary>
    /// EasyRuleSet用の基本じゃんけんテストケースを生成
    /// </summary>
    public static IEnumerable<TestCaseData> GetBasicRockPaperScissorsTestCases()
    {
      var testCases = new[]
      {
        (HandType.Rock, HandType.Scissors, JudgeResultType.Win),
        (HandType.Scissors, HandType.Paper, JudgeResultType.Win),
        (HandType.Paper, HandType.Rock, JudgeResultType.Win),
        (HandType.Scissors, HandType.Rock, JudgeResultType.Lose),
        (HandType.Paper, HandType.Scissors, JudgeResultType.Lose),
        (HandType.Rock, HandType.Paper, JudgeResultType.Lose),
        (HandType.Rock, HandType.Rock, JudgeResultType.Draw),
        (HandType.Scissors, HandType.Scissors, JudgeResultType.Draw),
        (HandType.Paper, HandType.Paper, JudgeResultType.Draw)
      };

      foreach (var (player, opponent, expected) in testCases)
      {
        yield return new TestCaseData(player, opponent, expected)
          .SetName($"{player}_vs_{opponent}_{expected}");
      }
    }
  }
}
#endif
