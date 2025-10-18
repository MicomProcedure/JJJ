using System;
using JJJ.Infrastructure;
using JJJ.Utils;
using MackySoft.Navigathena.SceneManagement;
using R3;
using VContainer.Unity;

namespace JJJ.UI
{
  public class ResultButtonManager : IStartable, IDisposable
  {
    private ResultButtonObservales _resultButtonObservales;

    private CompositeDisposable _disposables = new();

    public ResultButtonManager(ResultButtonObservales resultButtonObservales)
    {
      _resultButtonObservales = resultButtonObservales;
    }

    public void Start()
    {
      _resultButtonObservales.BackgroundButtonOnClick
        .DelaySubscription(TimeSpan.FromSeconds(1))
        .Subscribe(async _ =>
        {
          await GlobalSceneNavigator.Instance.Push(SceneNavigationUtil.TitleSceneIdentifier, new FadeTransitionDirector(SceneNavigationUtil.FadeTransitionIdentifier));
        })
        .AddTo(_disposables);
    }

    public void Dispose()
    {
      _disposables?.Dispose();
    }
  }
}