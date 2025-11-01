using System;
using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Core.Interfaces.UI;
using JJJ.UseCase;
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
    private RankingRegisterPanel _rankingRegisterPanel;
    private ResultButtonObservables _resultButtonObservables;
    private GameObject _clickScreenText;
    private IGameModeProvider _gameModeProvider;
    private IOptionProvider _optionProvider;
    private ISceneManager _sceneManager;
    private IUIInteractivityController _uiInteractivityController;
    private RankingUseCase _rankingUseCase;

    private CompositeDisposable _disposables = new();
    private Subject<Unit> _rankingRetrySubject = new();

    public ResultButtonManager(RankingRegisterPanel rankingRegisterView,
                               ResultButtonObservables resultButtonObservables,
                               [Key("ClickScreenText")] GameObject clickScreenText,
                               IGameModeProvider gameModeProvider,
                               IOptionProvider optionProvider,
                               ISceneManager sceneManager,
                               IUIInteractivityController uiInteractivityController,
                               RankingUseCase rankingUseCase)
    {
      _rankingRegisterPanel = rankingRegisterView;
      _resultButtonObservables = resultButtonObservables;
      _clickScreenText = clickScreenText;
      _gameModeProvider = gameModeProvider;
      _uiInteractivityController = uiInteractivityController;
      _optionProvider = optionProvider;
      _sceneManager = sceneManager;
      _rankingUseCase = rankingUseCase;
    }

    public void Start()
    {
      _rankingRegisterPanel.OnSubmit
        .ThrottleFirst(_rankingRetrySubject)
        .Subscribe(async result =>
        {
          if (result.Item1)
          {
            _rankingRegisterPanel.ShowLoading();
            try
            {
              await _rankingUseCase.SaveScoreAsync(_gameModeProvider.Current, new RankingData(result.Item2, _rankingRegisterPanel.Score));
              _rankingRegisterPanel.ShowSucceed();
              _uiInteractivityController.DisableAllInteractivity();
              await UniTask.Delay(500);
              await _sceneManager.PushWithFade(SceneNavigationUtil.TitleSceneIdentifier);
            }
            catch (Exception)
            {
              _rankingRegisterPanel.ShowFailed();
              _rankingRegisterPanel.HideButtons();
              await UniTask.Delay(1000);
              _rankingRegisterPanel.ShowRanking();
              _rankingRetrySubject.OnNext(Unit.Default);
              _rankingRegisterPanel.EnableRetryMode();
              _rankingRegisterPanel.ShowButtons();
            }
          }
          else
          {
            _uiInteractivityController.DisableAllInteractivity();
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
          _clickScreenText.SetActive(false);

          if (_optionProvider.IsAutoRankingSubmit)
          {
            try
            {
              await _rankingUseCase.SaveScoreAsync(_gameModeProvider.Current, new RankingData(_optionProvider.RankingDefaultName, _rankingRegisterPanel.Score));
            }
            catch (Exception)
            {
              // TODO: SDKが更新されたら送信失敗時の処理を追加する
              throw;
            }
            _uiInteractivityController.DisableAllInteractivity();
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