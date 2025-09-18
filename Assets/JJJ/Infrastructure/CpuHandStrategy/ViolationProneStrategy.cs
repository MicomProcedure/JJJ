using System.Linq;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Utils;
namespace JJJ.Infrastructure.CpuHandStrategy
{
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
    /// 乱数生成サービス
    /// </summary>
    private readonly IRandomService _randomService;

    public ViolationProneStrategy(IRandomService randomService)
    {
      _randomService = randomService;
    }

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
      // 有効な手のリストを取得
      var availableHandTypes = HandUtil.GetValidHandTypesFromContext(turnContext).ToList();

      // 後出しするかどうかを判定
      if (_randomService.NextDouble() < TimeoutProbability)
      {
        // 後出しする場合、有効な手の中からランダムに手を選択
        int index = _randomService.Next(availableHandTypes.Count);
        var selectedHandType = availableHandTypes[index];
        return new Hand(selectedHandType, HandUtil.GetHandName(selectedHandType), isTimeout: true);
      }

      // αの効果が発動中の場合
      if (turnContext.AlphaRemainingTurns > 0)
      {
        if (_randomService.NextDouble() < AlphaViolationProbability)
        {
          // αを出す場合
          return Hand.Alpha;
        }
      }

      // βの効果が発動中の場合
      if (turnContext.BetaRemainingTurns > 0)
      {
        if (_randomService.NextDouble() < BetaViolationProbability)
        {
          // βまたは封印された手を出す場合
          var sealedHandType = turnContext.SealedHandType;
          if (_randomService.NextDouble() < 0.5 || !sealedHandType.HasValue)
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
      int randomIndex = _randomService.Next(availableHandTypes.Count);
      var chosenHandType = availableHandTypes[randomIndex];
      return new Hand(chosenHandType, HandUtil.GetHandName(chosenHandType));
    }
  }
}