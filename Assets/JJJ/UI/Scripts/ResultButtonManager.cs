using System;
using JJJ.Infrastructure;
using JJJ.Utils;
using KanKikuchi.AudioManager;
using MackySoft.Navigathena.SceneManagement;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace JJJ.UI
{
  public class ResultButtonManager : IStartable, IDisposable
  {
    private ResultButtonObservales _resultButtonObservales;
    private GameObject _clickScreenText;

    private CompositeDisposable _disposables = new();

    public ResultButtonManager(ResultButtonObservales resultButtonObservales,
                               [Key("ClickScreenText")] GameObject clickScreenText)
    {
      _resultButtonObservales = resultButtonObservales;
      _clickScreenText = clickScreenText;
    }

    public void Start()
    {
      _resultButtonObservales.BackgroundButtonOnClick
        .DelaySubscription(TimeSpan.FromSeconds(3))
        .Do(onSubscribe: () => _clickScreenText.SetActive(true))
        .Subscribe(async _ =>
        {
          SEManager.Instance.Play(SEPath.SE6);
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