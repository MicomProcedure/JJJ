using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.View;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.SceneManagement.Utilities;
using MackySoft.Navigathena.Transitions;

namespace JJJ.Infrastructure
{
  /// <summary>
  /// FadeTransitionHandleのファクトリ
  /// </summary>
  public sealed class FadeTransitionDirector : ITransitionDirector
  {
    private readonly ISceneIdentifier _sceneInfo;

    public FadeTransitionDirector(ISceneIdentifier sceneInfo)
    {
      _sceneInfo = sceneInfo;
    }

    public ITransitionHandle CreateHandle()
    {
      return new FadeTransitionHandle(_sceneInfo);
    }

    private class FadeTransitionHandle : ITransitionHandle
    {
      private readonly ISceneIdentifier _sceneInfo;
      private ISceneHandle _sceneHandle = null!;
      private FadeTransitionDirectorBehaviour _director = null!;

      public FadeTransitionHandle(ISceneIdentifier sceneInfo)
      {
        _sceneInfo = sceneInfo;
      }

      public async UniTask Start(CancellationToken cancellationToken = default)
      {
        var handle = _sceneInfo.CreateHandle();
        var scene = await handle.Load(cancellationToken: cancellationToken);
        if (!scene.TryGetComponentInScene(out _director, true))
        {
          throw new InvalidOperationException($"Scene '{scene.name}' does not have a {nameof(FadeTransitionDirectorBehaviour)} component.");
        }

        _sceneHandle = handle;

        await _director.StartTransition(cancellationToken);
      }

      public async UniTask End(CancellationToken cancellationToken = default)
      {
        await _director.EndTransition(cancellationToken);

        await _sceneHandle.Unload(cancellationToken: cancellationToken);
      }
    }
  }
}