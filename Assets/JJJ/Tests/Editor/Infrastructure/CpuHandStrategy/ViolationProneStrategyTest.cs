#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
using JJJ.Core.Entities;
using NUnit.Framework;
using JJJ.Core.Interfaces;
using System.Collections.Generic;
using JJJ.Infrastructure.CpuHandStrategy;
using JJJ.Utils;
using System.Linq;

namespace JJJ.Tests.Infrastructure.CpuHandStrategy
{
  public class ViolationProneStrategyTest
  {
    // テスト用の乱数生成サービスのモック
    private class MockRandomService : IRandomService
    {
      private readonly Queue<double> _predefinedValues;
      public MockRandomService(IEnumerable<double> predefinedValues)
      {
        _predefinedValues = new Queue<double>(predefinedValues);
      }

      public double NextDouble()
      {
        return _predefinedValues.Count > 0 ? _predefinedValues.Dequeue() : 0.0;
      }

      public int Next(int max)
      {
        return (int)(NextDouble() * max);
      }

      public int Next(int minValue, int maxValue)
      {
        return minValue + Next(maxValue - minValue);
      }
    }

    private readonly GameMode _gameMode = GameMode.Hard;

    private static double TimeoutProbability;
    private static double AlphaViolationProbability;
    private static double BetaViolationProbability;

    [SetUp]
    public void Setup()
    {
      // ViolationProneStrategyの静的フィールドをリフレクションで取得
      var type = typeof(ViolationProneStrategy);
      TimeoutProbability = (double)type.GetField("TimeoutProbability", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
      AlphaViolationProbability = (double)type.GetField("AlphaViolationProbability", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
      BetaViolationProbability = (double)type.GetField("BetaViolationProbability", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
    }


    // 後出しをする場合のテスト
    [Test]
    public void GetNextCpuHand_WhenTimeoutOccurs_ReturnsTimeoutHand()
    {
      // 後出し確率を強制的に発生させるため、最初の乱数を0.0に設定
      // 有効な手の中から最初の手を選択するため、次の乱数を0に設定
      var mockRandomService = new MockRandomService(new[] { 0.0, 0 });
      var strategy = new ViolationProneStrategy(_gameMode, mockRandomService);
      var turnContext = new TurnContext();

      var validHands = HandUtil.GetValidHandTypesFromContext(_gameMode, turnContext).ToList();
      var expectedHandType = validHands[0];
      var expectedHand = new Hand(expectedHandType, HandUtil.GetHandName(expectedHandType), isTimeout: true);
      var hand = strategy.GetNextCpuHand(turnContext);

      Assert.AreEqual(expectedHand.Type, hand.Type);
      Assert.IsTrue(hand.IsTimeout);
    }

    // alphaの効果が発動中にalphaを出す場合のテスト
    [Test]
    public void GetNextCpuHand_WhenAlphaViolationOccurs_ReturnsAlpha()
    {
      // 後出し確率を発生させずにalphaの反則を出すため、
      // 最初の乱数をTimeoutProbability + AlphaViolationProbability / 2.0に設定
      var mockRandomService = new MockRandomService(new[] { TimeoutProbability + AlphaViolationProbability / 2.0, 0 });
      var strategy = new ViolationProneStrategy(_gameMode, mockRandomService);
      var turnContext = new TurnContext(alphaRemainingTurns: 1);

      var hand = strategy.GetNextCpuHand(turnContext);

      Assert.AreEqual(Hand.Alpha, hand);
    }

    // betaの効果が発動中にbetaを出す場合のテスト
    [Test]
    public void GetNextCpuHand_WhenBetaViolationOccursAndBetaChosen_ReturnsBeta()
    {
      // 後出し確率とalphaの反則を発生させずにbetaの反則を出すため、
      // 最初の乱数をTimeoutProbability + AlphaViolationProbability + BetaViolationProbability / 2.0に設定
      // betaまたは封印された手を出す場合にbetaを出す確率を発生させるため、次の乱数を0.0に設定
      var mockRandomService = new MockRandomService(new[] { TimeoutProbability + AlphaViolationProbability + BetaViolationProbability / 2.0, 0 });
      var strategy = new ViolationProneStrategy(_gameMode, mockRandomService);
      var turnContext = new TurnContext(betaRemainingTurns: 1, sealedHandType: HandType.Rock);

      var hand = strategy.GetNextCpuHand(turnContext);

      Assert.AreEqual(Hand.Beta.Type, hand.Type);
    }

    // betaの効果が発動中に封印された手を出す場合のテスト
    [Test]
    public void GetNextCpuHand_WhenBetaViolationOccursAndSealedHandChosen_ReturnsSealedHand()
    {
      // 後出し確率とalphaの反則を発生させずにbetaの反則を出すため、
      // 最初の乱数をTimeoutProbability + AlphaViolationProbability + BetaViolationProbability / 2.0に設定
      // betaまたは封印された手を出す場合に封印された手を出す確率を発生させるため、次の乱数を1.0に設定
      var mockRandomService = new MockRandomService(new[] { TimeoutProbability + AlphaViolationProbability + BetaViolationProbability / 2.0, 1 });
      var strategy = new ViolationProneStrategy(_gameMode, mockRandomService);
      var turnContext = new TurnContext(betaRemainingTurns: 1, sealedHandType: HandType.Paper);

      var hand = strategy.GetNextCpuHand(turnContext);

      Assert.AreEqual(Hand.Paper.Type, hand.Type);
    }

    // 通常の手を出す場合のテスト
    [Test]
    public void GetNextCpuHand_WhenNoViolationOccurs_ReturnsValidHand()
    {
      // 反則確率を発生させないため、最初の乱数を1.0に設定
      // 有効な手の中から最初の手を選択するため、次の乱数を0.0に設定
      var mockRandomService = new MockRandomService(new[] { 1.0, 0 });
      var strategy = new ViolationProneStrategy(_gameMode, mockRandomService);
      var turnContext = new TurnContext();

      var validHands = HandUtil.GetValidHandTypesFromContext(_gameMode, turnContext).ToList();
      var expectedHandType = validHands[0];
      var expectedHand = new Hand(expectedHandType, HandUtil.GetHandName(expectedHandType));
      var hand = strategy.GetNextCpuHand(turnContext);

      Assert.AreEqual(expectedHand.Type, hand.Type);
      Assert.IsFalse(hand.IsTimeout);
    }
  }
}

#endif