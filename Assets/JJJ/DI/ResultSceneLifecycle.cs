using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.UI;
using JJJ.View;
using KanKikuchi.AudioManager;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;

namespace JJJ.DI
{
  public sealed class ResultSceneLifecycle : SceneLifecycleBase
  {
    private string _sceneBGM = BGMPath.BGM3;

    private ResultView _resultView;
    private RankingRegisterPanel _rankingRegisterPanel;
    private IHighScoreProvider _highScoreProvider;
    private IGameModeProvider _gameModeProvider;

    public ResultSceneLifecycle(ResultView resultView,
                                RankingRegisterPanel rankingRegisterPanel,
                                IHighScoreProvider highScoreProvider,
                                IGameModeProvider gameModeProvider)
    {
      _resultView = resultView;
      _rankingRegisterPanel = rankingRegisterPanel;
      _highScoreProvider = highScoreProvider;
      _gameModeProvider = gameModeProvider;
    }

#if UNITY_EDITOR
    protected override UniTask OnEditorFirstPreInitialize(ISceneDataWriter writer, CancellationToken cancellationToken)
    {
      writer.Write(new ResultSceneData
      {
        Score = 1500,
        CompatibilityCount = (1, 2),
        TimeoutViolationCount = (3, 4),
        DoubleViolationCount = (5, 6),
        TimeoutCount = 7,
        AlphaCount = (8, 9),
        AlphaRepeatCount = (10, 11),
        BetaRepeatCount = (12, 13),
        SealedHandUsedCount = (14, 15)
      });
      return UniTask.CompletedTask;
    }
#endif

    protected override UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
      if (reader.TryRead<ResultSceneData>(out var sceneData))
      {
        _highScoreProvider.Set(_gameModeProvider.Current, sceneData.Score);
        _resultView.SetResult(sceneData);
        _rankingRegisterPanel.SetScore(sceneData.Score);
      }
      return UniTask.CompletedTask;
    }

    protected override UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
    {
      if (!string.IsNullOrEmpty(_sceneBGM))
      {
        BGMManager.Instance.Play(_sceneBGM);
      }

      return UniTask.CompletedTask;
    }

    protected override async UniTask OnExit(ISceneDataWriter writer, CancellationToken cancellationToken)
    {
      BGMManager.Instance.FadeOut(duration: 0.5f);
      await UniTask.Delay(500);
    }

    protected override UniTask OnFinalize(ISceneDataWriter writer, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
      return UniTask.CompletedTask;
    }
  }
}