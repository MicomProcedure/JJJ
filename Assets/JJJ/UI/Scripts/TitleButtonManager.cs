using System;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.Transitions;
using R3;
using VContainer.Unity;

namespace JJJ.UI
{
  public sealed class TitleButtonManager : IStartable, IDisposable
  {
    private IGameModeProvider _gameModeProvider;
    private IVisible _helpsView;
    private TitleButtonObservables _titleButtonObservables;
    private ITransitionDirector _transitionDirector;

    private CompositeDisposable _disposables = new();

    public TitleButtonManager(IGameModeProvider gameModeProvider,
                              IVisible helpsView,
                              TitleButtonObservables titleButtonObservables,
                              ITransitionDirector transitionDirector)
    {
      _gameModeProvider = gameModeProvider;
      _helpsView = helpsView;
      _titleButtonObservables = titleButtonObservables;
      _transitionDirector = transitionDirector;
    }

    public void Start()
    {
      Observable.Merge(
        _titleButtonObservables.EasyButtonOnClick.Select(_ => GameMode.Easy),
        _titleButtonObservables.NormalButtonOnClick.Select(_ => GameMode.Normal),
        _titleButtonObservables.HardButtonOnClick.Select(_ => GameMode.Hard)
      ).Take(1)
      .Subscribe(async gameMode =>
      {
        _gameModeProvider.Set(gameMode);
        await GlobalSceneNavigator.Instance.Push(SceneNavigationUtil.GameSceneIdentifier, _transitionDirector);
      })
      .AddTo(_disposables);

      _titleButtonObservables.ExitButtonOnClick
        .Subscribe(_ =>
        {
#if UNITY_EDITOR
          UnityEditor.EditorApplication.isPlaying = false;
#else
			    UnityEngine.Application.Quit();
#endif
        })
        .AddTo(_disposables);

      _titleButtonObservables.OptionButtonOnClick
        .Subscribe(_ =>
        {

        })
        .AddTo(_disposables);

      _titleButtonObservables.HelpButtonOnClick
        .Subscribe(_ =>
        {
          _helpsView.Show();
        })
        .AddTo(_disposables);

      _titleButtonObservables.RankingButtonOnClick
        .Subscribe(_ =>
        {

        })
        .AddTo(_disposables);

      _titleButtonObservables.HidePanelButtonOnClick
        .Subscribe(_ =>
        {
          _helpsView.Hide();
        })
        .AddTo(_disposables);
    }

    public void Dispose()
    {
      _disposables?.Dispose();
    }
  }
}