using System;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Infrastructure;

namespace JJJ.DI
{
  /// <summary>
  /// GameMode 等のコンテキストに応じて IRuleSet を生成 / 選択するファクトリの実装
  /// </summary>
  public class RuleSetFactory : IRuleSetFactory
  {
    private readonly IGameModeProvider _gameModeProvider;
    private readonly EasyRuleSet _easyRuleSet;
    private readonly NormalRuleSet _normalRuleSet;
    private readonly HardRuleSet _hardRuleSet;

    private IRuleSet _cachedRuleSet;

    public RuleSetFactory(IGameModeProvider gameModeProvider,
                          EasyRuleSet easyRuleSet,
                          NormalRuleSet normalRuleSet,
                          HardRuleSet hardRuleSet)
    {
      _gameModeProvider = gameModeProvider;
      _easyRuleSet = easyRuleSet;
      _normalRuleSet = normalRuleSet;
      _hardRuleSet = hardRuleSet;
    }

    public IRuleSet Create()
    {
      if (_cachedRuleSet != null)
      {
        return _cachedRuleSet;
      }

      var gameMode = _gameModeProvider.Current;

      _cachedRuleSet = gameMode switch
      {
        GameMode.Easy => _easyRuleSet,
        GameMode.Normal => _normalRuleSet,
        GameMode.Hard => _hardRuleSet,
        _ => throw new ArgumentOutOfRangeException($"Unsupported GameMode: {gameMode}")
      };
      return _cachedRuleSet;
    }
  }
}