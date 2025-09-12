#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
using JJJ.Infrastructure;
using JJJ.Core.Entities;
using JJJ.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace JJJ.Tests.Infrastructure
{
  /// <summary>
  /// テストデータ生成のヘルパークラス
  /// 各テストクラス間での重複コードを削減
  /// </summary>
  public static class TestDataHelper
  {
    /// <summary>
    /// 通常ルールでの勝利パターンを生成
    /// </summary>
    public static IEnumerable<TestCaseData> GetWinPatternTestCases()
    {
      foreach (var (player, opponent) in HandUtil.WinPatterns)
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
      foreach (var (player, opponent) in HandUtil.DrawPatterns)
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
      foreach (var (player, opponent) in HandUtil.LosePatterns)
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
      foreach (var (player, opponent) in HandUtil.GetAllHandCombinations(HandUtil.RegularHandTypes))
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
      return HandUtil.GetAllHandCombinations(handTypes ?? HandUtil.RegularHandTypes)
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
    public static IEnumerable<TestCaseData> GetIsSpecialTriangleTestCases()
    {
      var testCases = HandUtil.AllHandTypes.Where(ht => HandUtil.SpecialTriangleHandTypes.Contains(ht))
        .Select(ht => (ht, true))
        .Concat(
          HandUtil.AllHandTypes.Where(ht => !HandUtil.SpecialTriangleHandTypes.Contains(ht))
            .Select(ht => (ht, false))
        );

      foreach (var (handType, expected) in testCases)
      {
        yield return new TestCaseData(handType, expected)
          .SetName($"{handType}_IsSpecialTriangle_{expected}");
      }
    }

    /// <summary>
    /// Alpha/Betaの引き分けテストケースを生成
    /// Alpha/Betaが絡む場合は常に引き分けになることを確認
    /// </summary>
    public static IEnumerable<TestCaseData> GetAlphaBetaDrawTestCases()
    {
      var testCases = HandUtil.GetAllHandCombinations()
        .Where(combo => HandUtil.SpecialHandTypes.Contains(combo.player) || HandUtil.SpecialHandTypes.Contains(combo.opponent))
        .Where(combo => combo.player != combo.opponent); // 同じ手は除外

      foreach (var (player, opponent) in testCases)
      {
        yield return new TestCaseData(player, opponent, JudgeResultType.Draw)
          .SetName($"{player}_vs_{opponent}_AlphaBeta_Draw");
      }
    }

    /// <summary>
    /// 偶数ターン特殊三角形テストケースを生成
    /// </summary>
    public static IEnumerable<TestCaseData> GetSpecialTriangleTestCases()
    {
      var testCases = HandUtil.GetAllHandCombinations(HandUtil.SpecialTriangleHandTypes)
        .Where(combo => combo.player != combo.opponent)
        .Select(combo => (combo.player, combo.opponent, HandUtil.WinPatterns.Contains(combo)));

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
