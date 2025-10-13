using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
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
    private readonly GameMode _gameMode = GameMode.Normal;
    private readonly IGameModeProvider _gameModeProvider;
    private readonly IRuleSetFactory _ruleSetFactory;
    private readonly ILogger _logger = LogManager.CreateLogger<JudgeSceneLifecycle>();

    public JudgeSceneLifecycle(IGameModeProvider gameModeProvider,
                              IRuleSetFactory ruleSetFactory)
    {
      _gameModeProvider = gameModeProvider;
      _ruleSetFactory = ruleSetFactory;
    }

#if UNITY_EDITOR
    protected override UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken)
    {
      _logger.ZLogTrace($"OnEditorFirstPreInitialize called");
      writer.Write(new JudgeSceneData
      {
        GameMode = _gameMode
      });
      _logger.ZLogInformation($"Wrote JudgeSceneData with GameMode: {_gameMode}");
      return UniTask.CompletedTask;
    }
#endif

    protected override UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
      _logger.ZLogTrace($"OnInitialize called");
      // 初期化処理
      if (reader.TryRead<JudgeSceneData>(out var sceneData))
      {
        _gameModeProvider.Set(sceneData.GameMode);
        _logger.ZLogInformation($"GameMode set to {_gameModeProvider.Current}");
      }
      else
      {
        _gameModeProvider.Set(GameMode.Normal);
        _logger.ZLogError($"Failed to read JudgeSceneData. Defaulting GameMode to {_gameModeProvider.Current}");
      }

      // ルールセットの初期化を強制的に行う
      _ = _ruleSetFactory.Create();
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

  public class JudgeSceneData : ISceneData
  {
    public GameMode GameMode { get; set; } = GameMode.Normal;
  }
}