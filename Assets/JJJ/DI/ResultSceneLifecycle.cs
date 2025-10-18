using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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

    public ResultSceneLifecycle(ResultView resultView)
    {
      _resultView = resultView;
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
        _resultView.SetScore(sceneData.Score);
        _resultView.SetResult(sceneData.CompatibilityCount,
                              sceneData.TimeoutViolationCount,
                              sceneData.DoubleViolationCount,
                              sceneData.TimeoutCount,
                              sceneData.AlphaCount,
                              sceneData.AlphaRepeatCount,
                              sceneData.BetaRepeatCount,
                              sceneData.SealedHandUsedCount);
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

  public sealed class ResultSceneData : ISceneData
  {
    public int Score { get; set; }

    public (int, int) CompatibilityCount { get; set; }
    public (int, int) TimeoutViolationCount { get; set; }
    public (int, int) DoubleViolationCount { get; set; }
    public int TimeoutCount { get; set; }

    public (int, int) AlphaCount { get; set; }
    public (int, int) AlphaRepeatCount { get; set; }
    public (int, int) BetaRepeatCount { get; set; }
    public (int, int) SealedHandUsedCount { get; set; }
  }
}