using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Interfaces;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.Transitions;

namespace JJJ.Infrastructure
{
  public class SceneManager : ISceneManager
  {
    private ITransitionDirector _transitionDirector;

    public SceneManager(ITransitionDirector transitionDirector)
    {
      _transitionDirector = transitionDirector;
    }

    public async UniTask PushWithFade(ISceneIdentifier sceneIdentifier, ISceneData? sceneData = null, CancellationToken cancellationToken = default)
    {
      await GlobalSceneNavigator.Instance.Push(sceneIdentifier, _transitionDirector, sceneData, cancellationToken: cancellationToken);
    }
  }
}