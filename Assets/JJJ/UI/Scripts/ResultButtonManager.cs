using System;
using Cysharp.Threading.Tasks;
using JJJ.Core.Interfaces;
using JJJ.Core.Interfaces.UI;
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
    private CompositeDisposable _disposables = new();
    private Subject<Unit> _rankingRetrySubject = new();

    public ResultButtonManager(RankingRegisterPanel rankingRegisterView,
                               ResultButtonObservables resultButtonObservables,
                               [Key("ClickScreenText")] GameObject clickScreenText,
                               IGameModeProvider gameModeProvider,
                               IOptionProvider optionProvider,
                               ISceneManager sceneManager,
                               IUIInteractivityController uiInteractivityController)
    {
      _rankingRegisterPanel = rankingRegisterView;
      _resultButtonObservables = resultButtonObservables;
      _clickScreenText = clickScreenText;
      _gameModeProvider = gameModeProvider;
      _uiInteractivityController = uiInteractivityController;
      _optionProvider = optionProvider;
      _sceneManager = sceneManager;
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
            var task = ProcRaUtil.SaveAsync(_gameModeProvider.Current,
                            result.Item2,
                            _rankingRegisterPanel.Score);
            await task;
            if (task.Status == UniTaskStatus.Faulted)
            {
              _rankingRegisterPanel.ShowFailed();
              _rankingRegisterPanel.HideButtons();
              await UniTask.Delay(1000);
              _rankingRegisterPanel.ShowRanking();
              _rankingRetrySubject.OnNext(Unit.Default);
              _rankingRegisterPanel.EnableRetryMode();
              _rankingRegisterPanel.ShowButtons();
              // await _sceneManager.PushWithFade(SceneNavigationUtil.TitleSceneIdentifier);
            }
            else
            {
              _rankingRegisterPanel.ShowSucceed();
              _uiInteractivityController.DisableAllInteractivity();
              await UniTask.Delay(500);
              await _sceneManager.PushWithFade(SceneNavigationUtil.TitleSceneIdentifier);
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

          if (_optionProvider.IsAutoRankingSubmit)
          {
            // TODO: SDKが更新されたら送信失敗時の処理を追加する
            await ProcRaUtil.SaveAsync(_gameModeProvider.Current, _optionProvider.RankingDefaultName, _rankingRegisterPanel.Score);
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