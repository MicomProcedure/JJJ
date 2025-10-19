using System;
using JJJ.Utils;
using KanKikuchi.AudioManager;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.Transitions;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace JJJ.UI
{
  public class ResultButtonManager : IStartable, IDisposable
  {
    private ResultButtonObservables _resultButtonObservables;
    private GameObject _clickScreenText;
    private ITransitionDirector _transitionDirector;
    private CompositeDisposable _disposables = new();

    public ResultButtonManager(ResultButtonObservables resultButtonObservables,
                               [Key("ClickScreenText")] GameObject clickScreenText,
                               ITransitionDirector transitionDirector)
    {
      _resultButtonObservables = resultButtonObservables;
      _clickScreenText = clickScreenText;
      _transitionDirector = transitionDirector;
    }

    public void Start()
    {
      _resultButtonObservables.BackgroundButtonOnClick
        .Do(onSubscribe: () => _clickScreenText.SetActive(true))
        .DelaySubscription(TimeSpan.FromSeconds(2))
        .Take(1)
        .Subscribe(async _ =>
        {
          SEManager.Instance.Play(SEPath.SE6);
          await GlobalSceneNavigator.Instance.Push(SceneNavigationUtil.TitleSceneIdentifier, _transitionDirector);
        })
        .AddTo(_disposables);
    }

    public void Dispose()
    {
      _disposables?.Dispose();
    }
  }
}