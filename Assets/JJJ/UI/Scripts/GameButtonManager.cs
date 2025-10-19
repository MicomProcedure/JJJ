using System;
using JJJ.Infrastructure;
using JJJ.Utils;
using MackySoft.Navigathena.SceneManagement;
using R3;
using VContainer.Unity;

namespace JJJ.UI
{
  public class GameButtonManager : IStartable, IDisposable
  {
    private GameButtonObservables _gameButtonObservables;

    private CompositeDisposable _disposables = new();

    public GameButtonManager(GameButtonObservables gameButtonObservables)
    {
      _gameButtonObservables = gameButtonObservables;
    }

    public void Start()
    {
      _gameButtonObservables.ExitButtonOnClick
        .Subscribe(_ =>
        {
          GlobalSceneNavigator.Instance.Push(SceneNavigationUtil.TitleSceneIdentifier, new FadeTransitionDirector(SceneNavigationUtil.FadeTransitionIdentifier));
        })
        .AddTo(_disposables);

      _gameButtonObservables.RuleButtonOnClick
        .Subscribe(_ =>
        {

        })
        .AddTo(_disposables);
    }

    public void Dispose()
    {
      _disposables?.Dispose();
    }
  }
}