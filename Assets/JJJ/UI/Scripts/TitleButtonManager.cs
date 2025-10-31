using System;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using R3;
using VContainer.Unity;

namespace JJJ.UI
{
  public sealed class TitleButtonManager : IStartable, IDisposable
  {
    private IGameModeProvider _gameModeProvider;
    private IOptionProvider _optionProvider;
    private IRulesView _rulesView;
    private IOptionView _optionView;
    private IVisible _helpsView;
    private IVisible _rankingsView;
    private TitleButtonObservables _titleButtonObservables;
    private ISceneManager _sceneManager;

    private CompositeDisposable _disposables = new();

    public TitleButtonManager(IGameModeProvider gameModeProvider,
                              IOptionProvider optionProvider,
                              IRulesView rulesView,
                              IOptionView optionView,
                              IHelpsView helpsView,
                              IRankingsView rankingsView,
                              TitleButtonObservables titleButtonObservables,
                              ISceneManager sceneManager)
    {
      _gameModeProvider = gameModeProvider;
      _optionProvider = optionProvider;
      _rulesView = rulesView;
      _optionView = optionView;
      _helpsView = helpsView;
      _rankingsView = rankingsView;
      _titleButtonObservables = titleButtonObservables;
      _sceneManager = sceneManager;
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
        await _sceneManager.PushWithFade(SceneNavigationUtil.GameSceneIdentifier);
      })
      .AddTo(_disposables);

      Observable.Merge(
        _titleButtonObservables.EasyRuleButtonOnClick.Select(_ => GameMode.Easy),
        _titleButtonObservables.NormalRuleButtonOnClick.Select(_ => GameMode.Normal),
        _titleButtonObservables.HardRuleButtonOnClick.Select(_ => GameMode.Hard)
      )
      .Subscribe(gameMode => _rulesView.Show(gameMode))
      .AddTo(_disposables);

      _titleButtonObservables.HideRuleButtonOnClick
        .Subscribe(_ => _rulesView.Hide())
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
          _optionView.SetValue(_optionProvider.BGMVolume,
                               _optionProvider.SEVolume,
                               _optionProvider.IsAutoRankingSubmit,
                               _optionProvider.RankingDefaultName);
          _optionView.Show();
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
          _rankingsView.Show();
        })
        .AddTo(_disposables);

      _titleButtonObservables.HideOptionButtonOnClick
        .Subscribe(_ =>
        {
          _optionProvider.Set(_optionView.Option);
          _optionView.Hide();
        })
        .AddTo(_disposables);

      _titleButtonObservables.HideHelpsButtonOnClick
        .Subscribe(_ =>
        {
          _helpsView.Hide();
        })
        .AddTo(_disposables);

      _titleButtonObservables.HideRankingsButtonOnClick
        .Subscribe(_ =>
        {
          _rankingsView.Hide();
        })
        .AddTo(_disposables);
    }

    public void Dispose()
    {
      _disposables?.Dispose();
    }
  }
}