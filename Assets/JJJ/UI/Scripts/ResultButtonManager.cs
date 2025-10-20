using System;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using KanKikuchi.AudioManager;
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
    private ISceneManager _sceneManager;
    private CompositeDisposable _disposables = new();

    public ResultButtonManager(ResultButtonObservables resultButtonObservables,
                               [Key("ClickScreenText")] GameObject clickScreenText,
                               ISceneManager sceneManager)
    {
      _resultButtonObservables = resultButtonObservables;
      _clickScreenText = clickScreenText;
      _sceneManager = sceneManager;
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
          await _sceneManager.PushWithFade(SceneNavigationUtil.TitleSceneIdentifier);
        })
        .AddTo(_disposables);
    }

    public void Dispose()
    {
      _disposables?.Dispose();
    }
  }
}