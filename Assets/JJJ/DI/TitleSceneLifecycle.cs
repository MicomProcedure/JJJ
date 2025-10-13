using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MackySoft.Navigathena;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.VContainer;

namespace JJJ.DI
{
  public sealed class TitleSceneLifecycle : SceneLifecycleBase
  {
    protected override UniTask OnInitialize(ISceneDataReader reader, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
      return UniTask.CompletedTask;
    }

    protected override UniTask OnEnter(ISceneDataReader reader, CancellationToken cancellationToken)
    {
      return base.OnEnter(reader, cancellationToken);
    }

    protected override UniTask OnExit(ISceneDataWriter writer, CancellationToken cancellationToken)
    {
      return base.OnExit(writer, cancellationToken);
    }

    protected override UniTask OnFinalize(ISceneDataWriter writer, IProgress<IProgressDataStore> progress, CancellationToken cancellationToken)
    {
      return base.OnFinalize(writer, progress, cancellationToken);
    }
  }
}