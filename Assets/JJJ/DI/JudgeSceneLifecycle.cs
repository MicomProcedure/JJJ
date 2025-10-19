using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace JJJ.DI
{
  public class JudgeSceneLifecycle : SceneLifecycleBase
  {
    private readonly IGameModeProvider _gameModeProvider;
    private readonly IGameSettingsProvider _gameSettingsProvider;
    private readonly IRuleSetFactory _ruleSetFactory;
    private readonly IJudgeService _judgeService;
    private readonly ILogger _logger = LogManager.CreateLogger<JudgeSceneLifecycle>();

    public JudgeSceneLifecycle(IGameModeProvider gameModeProvider,
                               IGameSettingsProvider gameSettingsProvider,
                               IJudgeService judgeService,
                               IRuleSetFactory ruleSetFactory)
    {
      _gameModeProvider = gameModeProvider;
      _gameSettingsProvider = gameSettingsProvider;
      _judgeService = judgeService;
      _ruleSetFactory = ruleSetFactory;
    }

#if UNITY_EDITOR
    protected override UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken)
    {
      _logger.ZLogTrace($"OnEditorFirstPreInitialize called");
      var gameMode = _gameSettingsProvider.DefaultGameMode;
      _gameModeProvider.Set(gameMode);
      _logger.ZLogInformation($"Set GameMode: {gameMode}");
      return UniTask.CompletedTask;
    }
#endif

    protected override UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
      _logger.ZLogTrace($"OnInitialize called");

      // ルールセットの初期化を強制的に行う
      _ = _ruleSetFactory.Create();
      _judgeService.ApplyGameSettings();
      return UniTask.CompletedTask;
    }

    protected override UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
    {
      // シーンに入るときの処理
      return UniTask.CompletedTask;
    }

    protected override UniTask OnExit(ISceneDataWriter writer, CancellationToken cancellationToken)
    {
      // シーンから出るときの処理
      return UniTask.CompletedTask;
    }

    protected override UniTask OnFinalize(ISceneDataWriter writer, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
      // 終了処理
      return UniTask.CompletedTask;
    }
  }
}