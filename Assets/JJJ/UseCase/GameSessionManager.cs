using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using ZLogger;

namespace JJJ.UseCase
{
  public class GameSessionManager
  {
    private readonly IEnumerable<ICpuHandStrategy> _cpuHandStrategies;
    private readonly GameStateProvider _gameStateProvider;
    private readonly IStrategySelector _strategySelector;
    private readonly GameTurnManager _gameTurnManager;

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<GameSessionManager>();

    public GameSessionManager(
      IEnumerable<ICpuHandStrategy> cpuHandStrategies,
      GameStateProvider gameStateProvider,
      IStrategySelector strategySelector,
      GameTurnManager gameTurnManager
    )
    {
      _cpuHandStrategies = cpuHandStrategies;
      _gameStateProvider = gameStateProvider;
      _strategySelector = strategySelector;
      _gameTurnManager = gameTurnManager;
    }

    public async UniTask StartSession(CancellationToken cancellationToken = default)
    {
      _logger.ZLogDebug($"StartSession");

      var (pStrategy, oStrategy) = _strategySelector.SelectPair(_cpuHandStrategies);
      pStrategy.Initialize();
      oStrategy.Initialize();

      _gameStateProvider.PlayerCpuHandStrategy = pStrategy;
      _gameStateProvider.OpponentCpuHandStrategy = oStrategy;

      _gameStateProvider.CurrentTurnContext = new();

      bool isDraw;
      do
      {
        isDraw = await _gameTurnManager.StartTurn(cancellationToken);
      } while (isDraw);

      _logger.ZLogDebug($"Session ended");
    }
  }
}