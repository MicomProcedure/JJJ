using System;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.Transitions;
using R3;
using VContainer.Unity;

namespace JJJ.UI
{
  public class GameButtonManager : IStartable, IDisposable
  {
    private GameButtonObservables _gameButtonObservables;
    private IGameModeProvider _gameModeProvider;
    private IRulesView _rulesView;
    private ITransitionDirector _transitionDirector;

    private CompositeDisposable _disposables = new();

    public GameButtonManager(GameButtonObservables gameButtonObservables,
                             IGameModeProvider gameModeProvider,
                             IRulesView rulesView,
                             ITransitionDirector transitionDirector)
    {
      _gameButtonObservables = gameButtonObservables;
      _gameModeProvider = gameModeProvider;
      _rulesView = rulesView;
      _transitionDirector = transitionDirector;
    }

    public void Start()
    {
      _gameButtonObservables.ExitButtonOnClick
        .Subscribe(_ =>
        {
          GlobalSceneNavigator.Instance.Push(SceneNavigationUtil.TitleSceneIdentifier, _transitionDirector);
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