namespace JJJ.Infrastructure.CpuHandStrategy
{
  using System;
  using System.Linq;
  using JJJ.Core.Entities;
  using JJJ.Core.Interfaces;
  using JJJ.Utils;

  /// <summary>
  /// 違反しやすい戦略を実装するクラス
  /// </summary>
  public class ViolationProneStrategy : ICpuHandStrategy
  {
    /// <summary>
    /// 後出しする確率
    /// </summary>
    private static readonly double TimeoutProbability = 0.1;

    /// <summary>
    /// αの効果が発動中にαを出す確率
    /// </summary>
    private static readonly double AlphaViolationProbability = 0.2;

    /// <summary>
    /// βの効果が発動中にβまたは封印された手を出す確率
    /// </summary>
    /// <remarks>αが発動中の場合はAlphaViolationProbabilityの抽選が優先される</remarks>
    private static readonly double BetaViolationProbability = 0.2;

    /// <summary>
    /// 戦略の初期化
    /// </summary>
    public void Initialize()
    {
      // 初期化処理は不要
    }

    /// <summary>
    /// CPUの次の手を決定する
    /// </summary>
    /// <param name="turnContext">現在のターンのコンテキスト</param>
    /// <returns>CPUの次の手</returns>
    public Hand GetNextCpuHand(TurnContext turnContext)
    {
      // 乱数を生成
      var rand = new Random();
      double randomValue = rand.NextDouble();

      // 後出しするかどうかを判定
      if (randomValue < TimeoutProbability)
      {
        // 後出しする場合、ランダムに手を選択
        int index = rand.Next(HandUtil.RegularHandTypes.Count());
        var selectedHandType = HandUtil.RegularHandTypes.ElementAt(index);
        return new Hand(selectedHandType, HandUtil.GetHandName(selectedHandType), isTimeout: true);
      }

      // 再抽選
      randomValue = rand.NextDouble();

      // αの効果が発動中の場合
      if (turnContext.AlphaRemainingTurns > 0)
      {
        if (randomValue < AlphaViolationProbability)
        {
          // αを出す場合
          return Hand.Alpha;
        }
      }

      // 再抽選
      randomValue = rand.NextDouble();

      // βの効果が発動中の場合
      if (turnContext.BetaRemainingTurns > 0)
      {
        if (randomValue < BetaViolationProbability)
        {
          // βまたは封印された手を出す場合
          var sealedHandType = turnContext.SealedHandType;
          randomValue = rand.NextDouble();
          if (randomValue < 0.5 || !sealedHandType.HasValue)
          {
            // βを出す場合
            return Hand.Beta;
          }
          else
          {
            // 封印された手を出す場合
            return new Hand(sealedHandType.Value, HandUtil.GetHandName(sealedHandType.Value));
          }
        }
      }

      // 通常の手をランダムに選択
      var availableHandTypes = HandUtil.GetValidHandTypesFromContext(turnContext).ToList();
      int randomIndex = rand.Next(availableHandTypes.Count);
      var chosenHandType = availableHandTypes[randomIndex];
      return new Hand(chosenHandType, HandUtil.GetHandName(chosenHandType));
    }
  }
}