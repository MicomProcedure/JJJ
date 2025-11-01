#if UNITY_EDITOR || UNITY_INCLUDE_TESTS

using System.Collections.Generic;
using System.Linq;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Infrastructure;
using JJJ.Infrastructure.CpuHandStrategy;
using JJJ.Utils;
using NUnit.Framework;
using UnityEngine;

namespace JJJ.Tests.Infrastructure.CpuHandStrategy
{
  public class CyclicStrategyTest
  {
    /// <summary>
    /// テスト用の乱数生成サービスのモック（決定論的）
    /// </summary>
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
    private readonly GameModeProvider _gameModeProvider = new GameModeProvider();
    private readonly GameSettingsProvider _gameSettingsProvider = ScriptableObject.CreateInstance<GameSettingsProvider>();

    [SetUp]
    public void Setup()
    {
      _gameModeProvider.Set(_gameMode);
    }

    /// <summary>
    /// 無効な手が存在しない状況で反則する場合は、後出しを選択する
    /// </summary>
    [Test]
    public void GetNextCpuHand_WhenViolationOccursAndNoInvalid_ChoosesTimeoutFromValid()
    {
      // 反則確率を強制的に発生させるため、最初の乱数を0.0に設定
      // 無効手選択は無視されるため、次の乱数を0.0に設定
      // 有効な手の中から最初の手を選択するため、次の乱数を0に設定
      var mock = new MockRandomService(new[] { 0.0, 0.0, 0.0 });
      var strategy = new CyclicStrategy(_gameModeProvider, mock, _gameSettingsProvider);
      var ctx = new TurnContext();
      strategy.Initialize();

      var valid = HandUtil.GetValidHandTypesFromContext(_gameMode, ctx, PersonType.Player).ToList();
      var expectedType = valid[0]; // 先頭（グー）

      var hand = strategy.GetNextCpuHand(ctx, PersonType.Player);

      Assert.IsTrue(hand.IsTimeout, $"Hand should be marked as timeout, expect: {true} but was {hand.IsTimeout}");
      Assert.AreEqual(expectedType, hand.Type, $"Hand type should be {expectedType} but was {hand.Type}");
    }

    /// <summary>
    /// alphaの無効手が存在する場合（α発動中）、alphaを選択する
    /// </summary>
    [Test]
    public void GetNextCpuHand_WhenViolationOccursAndInvalidExists_ChoosesInvalidHand()
    {
      // 反則確率を強制的に発生させるため、最初の乱数を0.0に設定
      // 無効手選択でαを選ばせるため、次の乱数を0.0に設定
      var mock = new MockRandomService(new[] { 0.0, 0.0 });
      var strategy = new CyclicStrategy(_gameModeProvider, mock, _gameSettingsProvider);
      var ctx = new TurnContext(playerAlphaRemainingTurns: 1);
      strategy.Initialize();

      var hand = strategy.GetNextCpuHand(ctx, PersonType.Player);

      Assert.IsFalse(hand.IsTimeout, $"Hand should not be marked as timeout, expect: {false} but was {hand.IsTimeout}");
      Assert.AreEqual(HandType.Alpha, hand.Type, $"Hand type should be {HandType.Alpha} but was {hand.Type}");
    }

    /// <summary>
    /// 違反しない場合、初回はランダム、以降は循環する
    /// </summary>
    [Test]
    public void GetNextCpuHand_WhenNoViolation_FirstRandomThenCyclic()
    {
      // 反則しないので、最初の乱数を1に設定（有効手の中からランダムに選択）
      // 初回は先頭インデックス(0)を選ばせるため、次の乱数を0.0に設定
      var mock = new MockRandomService(new[] { 1, 0.0, 1 });
      var strategy = new CyclicStrategy(_gameModeProvider, mock, _gameSettingsProvider);
      var ctx = new TurnContext();
      strategy.Initialize();

      var valid = HandUtil.GetValidHandTypesFromContext(_gameMode, ctx, PersonType.Player).ToList();

      var first = strategy.GetNextCpuHand(ctx, PersonType.Player);
      var second = strategy.GetNextCpuHand(ctx, PersonType.Player);

      Assert.IsFalse(first.IsTimeout, $"First hand should not be marked as timeout, expect: {false} but was {first.IsTimeout}");
      Assert.AreEqual(valid[0], first.Type, $"First hand type should be {valid[0]} but was {first.Type}");

      Assert.IsFalse(second.IsTimeout, $"Second hand should not be marked as timeout, expect: {false} but was {second.IsTimeout}");
      Assert.AreEqual(valid[1], second.Type, $"Second hand type should be {valid[1]} but was {second.Type}");
    }

    /// <summary>
    /// 違反しない場合、末尾の次は先頭に戻る
    /// </summary>
    [Test]
    public void GetNextCpuHand_WhenNoViolation_WrapsAroundToFirst()
    {
      var ctx = new TurnContext();
      var valid = HandUtil.GetValidHandTypesFromContext(_gameMode, ctx, PersonType.Player).ToList();
      double pickIndex = (double)(valid.Count - 1) / valid.Count; // 初回に末尾インデックス

      // 反則しないので、最初の乱数を1に設定（有効手の中からランダムに選択）
      // 初回に末尾インデックスを選ばせるため、次の乱数を(count-1)/countに設定
      var mock = new MockRandomService(new[] { 1.0, pickIndex, 1 });
      var strategy = new CyclicStrategy(_gameModeProvider, mock, _gameSettingsProvider);
      strategy.Initialize();

      var first = strategy.GetNextCpuHand(ctx, PersonType.Player);
      var second = strategy.GetNextCpuHand(ctx, PersonType.Player);

      Assert.AreEqual(valid[^1], first.Type, $"First hand type should be {valid[^1]} but was {first.Type}");
      Assert.AreEqual(valid[0], second.Type, $"Second hand type should be {valid[0]} but was {second.Type}");
    }

    /// <summary>
    /// Initialize により内部インデックスがリセットされる
    /// </summary>
    [Test]
    public void Initialize_ResetsIndexAndRandomizesAgain()
    {
      var ctx = new TurnContext();
      var valid = HandUtil.GetValidHandTypesFromContext(_gameMode, ctx, PersonType.Player).ToList();
      int count = valid.Count;
      double pickIndex2 = 2.0 / valid.Count; // 初回にインデックス2

      // 反則しないので、最初の乱数を1に設定（有効手の中からランダムに選択）
      // 初回にインデックス2を選ばせるため、次の乱数を2に設定
      // その後、2回循環させてからInitializeし、再度ランダムに戻る
      // その際、再度先頭インデックス(0)を選ばせるため、最後の乱数を0.0に設定
      var mock = new MockRandomService(new[] { 1, pickIndex2, 1, 1, 0.0 });
      var strategy = new CyclicStrategy(_gameModeProvider, mock, _gameSettingsProvider);
      strategy.Initialize();

      var first = strategy.GetNextCpuHand(ctx, PersonType.Player);  // idx=2
      var second = strategy.GetNextCpuHand(ctx, PersonType.Player); // idx=3
      strategy.Initialize();
      var third = strategy.GetNextCpuHand(ctx, PersonType.Player);  // 再度ランダム idx=0

      Assert.AreEqual(valid[2], first.Type, $"First hand type should be {valid[2]} but was {first.Type}");
      Assert.AreEqual(valid[3 % count], second.Type, $"Second hand type should be {valid[3 % count]} but was {second.Type}");
      Assert.AreEqual(valid[0], third.Type, $"After Initialize, hand type should be {valid[0]} but was {third.Type}");
    }
  }
}
#endif