using System;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using R3;
using VContainer.Unity;

namespace JJJ.UI
{
  public class GameButtonManager : IStartable, IDisposable
  {
    private GameButtonObservables _gameButtonObservables;
    private IGameModeProvider _gameModeProvider;
    private IRulesView _rulesView;
    private ISceneManager _sceneManager;

    private CompositeDisposable _disposables = new();

    public GameButtonManager(GameButtonObservables gameButtonObservables,
                             IGameModeProvider gameModeProvider,
                             IRulesView rulesView,
                             ISceneManager sceneManager)
    {
      _gameButtonObservables = gameButtonObservables;
      _gameModeProvider = gameModeProvider;
      _rulesView = rulesView;
      _sceneManager = sceneManager;
    }

    public void Start()
    {
      _gameButtonObservables.ExitButtonOnClick
        .Take(1)
        .Subscribe(async _ =>
        {
          await _sceneManager.PushWithFade(SceneNavigationUtil.TitleSceneIdentifier);
        })
        .AddTo(_disposables);

      _gameButtonObservables.RuleButtonOnClick
        .Subscribe(_ =>
        {
          _rulesView.Show(_gameModeProvider.Current);
        })
        .AddTo(_disposables);

      _gameButtonObservables.HideRuleButtonOnClick
        .Subscribe(_ =>
        {
          _rulesView.Hide();
        })
        .AddTo(_disposables);
    }

    public void Dispose()
    {
      _disposables?.Dispose();
    }
  }
}