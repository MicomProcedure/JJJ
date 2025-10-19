using System;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Infrastructure;
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
    private TitleButtonObservables _titleButtonObservables;
    private ITransitionDirector _transitionDirector;

    private CompositeDisposable _disposables = new();

    public TitleButtonManager(IGameModeProvider gameModeProvider,
                              TitleButtonObservables titleButtonObservables,
                              ITransitionDirector transitionDirector)
    {
      _gameModeProvider = gameModeProvider;
      _titleButtonObservables = titleButtonObservables;
      _transitionDirector = transitionDirector;
    }

    public void Start()
    {
      _titleButtonObservables.EasyButtonOnClick
        .Subscribe(async _ =>
        {
          _gameModeProvider.Set(GameMode.Easy);
          await GlobalSceneNavigator.Instance.Push(SceneNavigationUtil.GameSceneIdentifier, _transitionDirector);
        })
        .AddTo(_disposables);

      _titleButtonObservables.NormalButtonOnClick
        .Subscribe(async _ =>
        {
          _gameModeProvider.Set(GameMode.Normal);
          await GlobalSceneNavigator.Instance.Push(SceneNavigationUtil.GameSceneIdentifier, _transitionDirector);
        })
        .AddTo(_disposables);

      _titleButtonObservables.HardButtonOnClick
        .Subscribe(async _ =>
        {
          _gameModeProvider.Set(GameMode.Hard);
          await GlobalSceneNavigator.Instance.Push(SceneNavigationUtil.GameSceneIdentifier, new FadeTransitionDirector(SceneNavigationUtil.FadeTransitionIdentifier));
        })
        .AddTo(_disposables);

      _titleButtonObservables.ExitButtonOnClick
        .Subscribe(_ =>
        {
#if UNITY_EDITOR
          UnityEditor.EditorApplication.isPlaying = false;
#else
			    Application.Quit();
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

        })
        .AddTo(_disposables);

      _titleButtonObservables.RankingButtonOnClick
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