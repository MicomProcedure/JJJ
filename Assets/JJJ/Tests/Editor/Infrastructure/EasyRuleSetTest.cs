#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
namespace JJJ.Tests.Infrastructure
{
  using JJJ.Infrastructure;
  using JJJ.Core.Entities;
  using NUnit.Framework;
  using System.Collections.Generic;

  /// <summary>
  /// EasyRuleSetクラスのテスト
  /// 簡単ルールでのじゃんけん判定とタイムアウト処理をテスト
  /// </summary>
  public class EasyRuleSetTest
  {
    private readonly EasyRuleSet _easyRuleSet = new();

    /// <summary>
    /// 通常のじゃんけんテスト（タイムアウトなし）
    /// 基本じゃんけん（グー・チョキ・パー）での勝敗判定が正しく行われることを確認
    /// </summary>
    [TestCaseSource(typeof(TestDataHelper), nameof(TestDataHelper.GetBasicRockPaperScissorsTestCases))]
    public void Judge_BasicRockPaperScissors_ReturnsExpectedResult(HandType playerHand, HandType opponentHand, JudgeResultType expectedResultType)
    {
      // Arrange
      var player = new Hand(playerHand, playerHand.ToString());
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext();

      // Act
      var result = _easyRuleSet.Judge(player, opponent, turnContext);

      // Assert
      Assert.That(result.Type, Is.EqualTo(expectedResultType),
                  $"Result differs from expected. Player: {playerHand} vs Opponent: {opponentHand}");
    }

    /// <summary>
    /// プレイヤータイムアウトテスト
    /// プレイヤーがタイムアウトした場合に反則と判定されることを確認
    /// </summary>
    [TestCaseSource(nameof(GetTimeoutTestCases))]
    public void Judge_WhenPlayerTimesOut_ReturnsViolation(HandType playerHand, HandType opponentHand)
    {
      // Arrange
      var player = new Hand(playerHand, playerHand.ToString(), isTimeout: true);
      var opponent = new Hand(opponentHand, opponentHand.ToString());
      var turnContext = new TurnContext();

      // Act
      var result = _easyRuleSet.Judge(player, opponent, turnContext);

      // Assert
      Assert.That(result.Type, Is.EqualTo(JudgeResultType.Violation),
                  $"Player timeout should result in violation. Player: {playerHand}(timeout) vs Opponent: {opponentHand}");
    }

    /// <summary>
    /// タイムアウトテスト用のテストケースを生成
    /// 基本じゃんけんの手同士の全組み合わせを提供
    /// </summary>
    private static IEnumerable<TestCaseData> GetTimeoutTestCases()
    {
      var handTypes = TestDataHelper.BasicHandTypes;

      foreach (var playerHand in handTypes)
      {
        foreach (var opponentHand in handTypes)
        {
          yield return new TestCaseData(playerHand, opponentHand)
            .SetName($"{playerHand}_vs_{opponentHand}");
        }
      }
    }
  }
}
#endif