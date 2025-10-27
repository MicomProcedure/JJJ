using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using ZLogger;

namespace JJJ.UseCase
{
  /// <summary>
  /// ゲームのセッションを管理するクラス
  /// </summary>
  public class GameSessionManager
  {
    private readonly IEnumerable<ICpuHandStrategy> _cpuHandStrategies;
    private readonly GameStateProvider _gameStateProvider;
    private readonly IStrategySelector _strategySelector;
    private readonly GameTurnManager _gameTurnManager;
    private readonly ICompositeHandAnimationPresenter _compositeHandAnimationPresenter;

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<GameSessionManager>();

    public GameSessionManager(
      IEnumerable<ICpuHandStrategy> cpuHandStrategies,
      GameStateProvider gameStateProvider,
      IStrategySelector strategySelector,
      GameTurnManager gameTurnManager,
      ICompositeHandAnimationPresenter compositeHandAnimationPresenter
    )
    {
      _cpuHandStrategies = cpuHandStrategies;
      _gameStateProvider = gameStateProvider;
      _strategySelector = strategySelector;
      _gameTurnManager = gameTurnManager;
      _compositeHandAnimationPresenter = compositeHandAnimationPresenter;
    }

    /// <summary>
    /// セッションを開始する
    /// </summary>
    public async UniTask StartSession(CancellationToken cancellationToken = default)
    {
      _logger.ZLogDebug($"StartSession");

      // CPUの戦略を選択して初期化
      var (pStrategy, oStrategy) = _strategySelector.SelectPair(_cpuHandStrategies);
      pStrategy.Initialize();
      oStrategy.Initialize();

      _gameStateProvider.PlayerCpuHandStrategy = pStrategy;
      _gameStateProvider.OpponentCpuHandStrategy = oStrategy;

      _compositeHandAnimationPresenter.SelectDominantHandAll();

      // ターン開始前の初期化
      _gameStateProvider.CurrentTurnContext = new();

      // ターン開始
      // 引き分けでない限りターンを繰り返す
      bool isDraw;
      do
      {
        isDraw = await _gameTurnManager.StartTurn(cancellationToken);
      } while (isDraw);

      _logger.ZLogDebug($"Session ended");
    }
  }
}