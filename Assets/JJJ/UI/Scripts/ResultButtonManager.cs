using System;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using KanKikuchi.AudioManager;
using ProcRanking;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace JJJ.UI
{
  public class ResultButtonManager : IStartable, IDisposable
  {
    private RankingRegisterPanel _rankingRegisterView;
    private ResultButtonObservables _resultButtonObservables;
    private GameObject _clickScreenText;
    private IGameModeProvider _gameModeProvider;
    private IOptionProvider _optionProvider;
    private ISceneManager _sceneManager;
    private CompositeDisposable _disposables = new();

    public ResultButtonManager(RankingRegisterPanel rankingRegisterView,
                               ResultButtonObservables resultButtonObservables,
                               [Key("ClickScreenText")] GameObject clickScreenText,
                               IGameModeProvider gameModeProvider,
                               IOptionProvider optionProvider,
                               ISceneManager sceneManager)
    {
      _rankingRegisterView = rankingRegisterView;
      _resultButtonObservables = resultButtonObservables;
      _clickScreenText = clickScreenText;
      _gameModeProvider = gameModeProvider;
      _optionProvider = optionProvider;
      _sceneManager = sceneManager;
    }

    public void Start()
    {
      _rankingRegisterView.OnSubmit
        .Take(1)
        .Subscribe(async result =>
        {
          if (result.Item1)
          {
            var data = new ProcRaData(ProcRaUtil.StoreName(_gameModeProvider.Current))
              .Add("name", result.Item2)
              .Add("score", _rankingRegisterView.Score);

            data.SaveAsync();
          }

          await _sceneManager.PushWithFade(SceneNavigationUtil.TitleSceneIdentifier);
        })
        .AddTo(_disposables);

      _resultButtonObservables.BackgroundButtonOnClick
        .Do(onSubscribe: () => _clickScreenText.SetActive(true))
        .DelaySubscription(TimeSpan.FromSeconds(2))
        .Take(1)
        .Subscribe(async _ =>
        {
          SEManager.Instance.Play(SEPath.SE6);

          if (_optionProvider.IsAutoRankingSubmit)
          {
            var data = new ProcRaData(ProcRaUtil.StoreName(_gameModeProvider.Current))
              .Add("name", _optionProvider.RankingDefaultName)
              .Add("score", _rankingRegisterView.Score);

            data.SaveAsync();
            await _sceneManager.PushWithFade(SceneNavigationUtil.TitleSceneIdentifier);
          }
          else
          {
            _rankingRegisterView.Show();
          }
        })
        .AddTo(_disposables);
    }

    public void Dispose()
    {
      _disposables?.Dispose();
    }
  }
}