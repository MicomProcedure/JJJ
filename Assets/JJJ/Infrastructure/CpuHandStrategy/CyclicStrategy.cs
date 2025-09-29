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
    /// <summary>
    /// 現在選択されている手のインデックス
    /// </summary>
    private int _currentSelectedIndex = -1;

    /// <summary>
    /// 反則する確率
    /// </summary>
    private double _violationProbability = 0.1;

    /// <summary>
    /// ゲームモード
    /// </summary>
    private readonly GameMode _gameMode;

    /// <summary>
    /// 乱数生成サービス
    /// </summary>
    private readonly IRandomService _randomService;

    public CyclicStrategy(IGameModeProvider gameModeProvider, IRandomService randomService)
    {
      _gameMode = gameModeProvider.Current;
      _randomService = randomService;
    }

    /// <summary>
    /// 戦略の初期化
    /// </summary>
    public void Initialize()
    {
      _currentSelectedIndex = -1;
    }

    public Hand GetNextCpuHand(TurnContext turnContext)
    {
      var validHandTypes = HandUtil.GetValidHandTypesFromContext(_gameMode, turnContext);

      // 反則するかどうかを判定
      if (_randomService.NextDouble() < _violationProbability)
      {
        // 無効な手のListを取得
        var allHandTypes = HandUtil.GetAvailableHandTypesFromGameMode(_gameMode);
        var invalidHandTypes = allHandTypes.Except(validHandTypes).ToList();
        // 無効な手を出すか後出しするかをランダムに選択
        int chosenType = _randomService.Next(0, invalidHandTypes.Count + 1);
        if (chosenType < invalidHandTypes.Count)
        {
          // 無効な手を選択
          return new Hand(invalidHandTypes[chosenType]);
        }
        else
        {
          // 後出しを選択
          // 手は有効な手の中からランダムに選択
          int handType = _randomService.Next(0, validHandTypes.Count());
          return new Hand(
            validHandTypes.ElementAt(handType),
            isTimeout: true
          );
        }
      }

      // 反則しない場合は循環的に手を選択
      // セッションで最初の手ならランダムに選択し、そうでなければ次の手を選択
      _currentSelectedIndex = _currentSelectedIndex < 0 ? _randomService.Next(0, validHandTypes.Count()) : (_currentSelectedIndex + 1) % validHandTypes.Count();
      if (_currentSelectedIndex < 0 || _currentSelectedIndex >= validHandTypes.Count())
      {
        UnityEngine.Debug.LogError($"CyclicStrategy: currentSelectedIndex out of range. currentSelectedIndex={_currentSelectedIndex}, validHandTypes.Count()={validHandTypes.Count()}\nFailback to 0.");
        _currentSelectedIndex = 0;
      }
      var chosenHandType = validHandTypes.ElementAt(_currentSelectedIndex);
      return new Hand(chosenHandType);
    }
  }
}