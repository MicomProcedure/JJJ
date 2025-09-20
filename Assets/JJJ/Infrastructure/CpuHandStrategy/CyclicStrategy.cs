using System.Diagnostics;
using System.Linq;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Utils;

namespace JJJ.Infrastructure.CpuHandStrategy
{
  /// <summary>
  /// CPUの手を循環的に決定する戦略
  /// </summary>
  public class CyclicStrategy : ICpuHandStrategy
  {
    private int currentSelectedIndex = -1;

    /// <summary>
    /// 反則する確率
    /// </summary>
    private double ViolationProbability = 0.1;

    /// <summary>
    /// ゲームモード
    /// </summary>
    private readonly GameMode _gameMode;

    /// <summary>
    /// 乱数生成サービス
    /// </summary>
    private readonly IRandomService _randomService;

    public CyclicStrategy(GameMode gameMode, IRandomService randomService)
    {
      _gameMode = gameMode;
      _randomService = randomService;
    }

    /// <summary>
    /// 戦略の初期化
    /// </summary>
    public void Initialize()
    {
      currentSelectedIndex = -1;
    }

    public Hand GetNextCpuHand(TurnContext turnContext)
    {
      var validHandTypes = HandUtil.GetValidHandTypesFromContext(_gameMode, turnContext);

      // 反則するかどうかを判定
      if (_randomService.NextDouble() < ViolationProbability)
      {
        // 無効な手のListを取得
        var allHandTypes = HandUtil.AllHandTypes.ToList();
        var invalidHandTypes = allHandTypes.Except(validHandTypes).ToList();
        // 無効な手を出すか後出しするかをランダムに選択
        int chosenType = _randomService.Next(0, invalidHandTypes.Count + 1);
        if (chosenType < invalidHandTypes.Count)
        {
          // 無効な手を選択
          return new Hand(invalidHandTypes[chosenType], HandUtil.GetHandName(invalidHandTypes[chosenType]));
        }
        else
        {
          // 後出しを選択
          // 手は有効な手の中からランダムに選択
          int handType = _randomService.Next(0, validHandTypes.Count());
          return new Hand(
            validHandTypes.ElementAt(handType),
            HandUtil.GetHandName(validHandTypes.ElementAt(handType)),
            isTimeout: true
          );
        }
      }

      // 反則しない場合は循環的に手を選択
      // セッションで最初の手ならランダムに選択し、そうでなければ次の手を選択
      currentSelectedIndex = currentSelectedIndex < 0 ? _randomService.Next(0, validHandTypes.Count()) : (currentSelectedIndex + 1) % validHandTypes.Count();
      if (currentSelectedIndex < 0 || currentSelectedIndex >= validHandTypes.Count())
      {
        UnityEngine.Debug.LogError($"CyclicStrategy: currentSelectedIndex out of range. currentSelectedIndex={currentSelectedIndex}, validHandTypes.Count()={validHandTypes.Count()}\nFailback to 0.");
        currentSelectedIndex = 0;
      }
      var chosenHandType = validHandTypes.ElementAt(currentSelectedIndex);
      return new Hand(chosenHandType, HandUtil.GetHandName(chosenHandType));
    }
  }
}