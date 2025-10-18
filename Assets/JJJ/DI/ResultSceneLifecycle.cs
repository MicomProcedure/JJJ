using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using KanKikuchi.AudioManager;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;

namespace JJJ.DI
{
  public sealed class ResultSceneLifecycle : SceneLifecycleBase
  {
    private string _sceneBGM = BGMPath.BGM3;

    protected override UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
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