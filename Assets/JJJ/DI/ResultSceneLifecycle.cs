using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.View;
using KanKikuchi.AudioManager;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;
using UnityEngine;

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
        CompatibilityCount = new(1, 2),
        TimeoutViolationCount = new(3, 4),
        DoubleViolationCount = new(5, 6),
        TimeoutCount = 7
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

    public Vector2Int CompatibilityCount { get; set; }
    public Vector2Int TimeoutViolationCount { get; set; }
    public Vector2Int DoubleViolationCount { get; set; }
    public int TimeoutCount { get; set; }

    public Vector2Int? AlphaCount { get; set; } = null;
    public Vector2Int? AlphaRepeatCount { get; set; } = null;
    public Vector2Int? BetaRepeatCount { get; set; } = null;
    public Vector2Int? SealedHandUsedCount { get; set; } = null;
  }
}