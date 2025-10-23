using System;
using Cysharp.Threading.Tasks;
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
    private RankingRegisterPanel _rankingRegisterPanel;
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
      _rankingRegisterPanel = rankingRegisterView;
      _resultButtonObservables = resultButtonObservables;
      _clickScreenText = clickScreenText;
      _gameModeProvider = gameModeProvider;
      _optionProvider = optionProvider;
      _sceneManager = sceneManager;
    }

    public void Start()
    {
      _rankingRegisterPanel.OnSubmit
        .Take(1)
        .Subscribe(async result =>
        {
          if (result.Item1)
          {
            var data = new ProcRaData(ProcRaUtil.StoreName(_gameModeProvider.Current))
              .Add("name", result.Item2)
              .Add("score", _rankingRegisterPanel.Score);

            data.SaveAsync(async (ProcRaException e) =>
            {
              if (e != null)
              {
                _rankingRegisterPanel.ShowFailed();
                await UniTask.Delay(500);
                await _sceneManager.PushWithFade(SceneNavigationUtil.TitleSceneIdentifier);
              }
              else
              {
                _rankingRegisterPanel.ShowSucceed();
                await UniTask.Delay(500);
                await _sceneManager.PushWithFade(SceneNavigationUtil.TitleSceneIdentifier);
              }
            });
          }
          else
          {
            await _sceneManager.PushWithFade(SceneNavigationUtil.TitleSceneIdentifier);
          }

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
              .Add("score", _rankingRegisterPanel.Score);

            data.SaveAsync();
            await _sceneManager.PushWithFade(SceneNavigationUtil.TitleSceneIdentifier);
          }
          else
          {
            _rankingRegisterPanel.Show();
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