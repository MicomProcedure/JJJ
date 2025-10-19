using JJJ.Core.Interfaces;
using JJJ.Infrastructure;
using JJJ.Infrastructure.CpuHandStrategy;
using JJJ.UI.Scripts;
using JJJ.View;
using JJJ.UseCase;
using JJJ.UseCase.Strategy;
using JJJ.UseCase.Turn;
using MackySoft.Navigathena.SceneManagement.VContainer;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using JJJ.UI;

namespace JJJ.DI
{
  /// <summary>
  /// ジャッジ関連のライフタイムスコープを定義するクラス
  /// </summary>
  public class JudgeLifetimeScope : LifetimeScope
  {
    [SerializeField] private JudgeInput? _judgeInput;
    [SerializeField] private CompositeHandAnimationPresenter? _compositeHandAnimationPresenter;
    [SerializeField] private TimerRemainsPresenter? _timerRemainsPresenter;
    [SerializeField] private CurrentScorePresenter? _currentScorePresenter;
    [SerializeField] private CurrentJudgesPresenter? _currentJudgesPresenter;
    [SerializeField] private RemainJudgeTimePresenter? _remainJudgeTimePresenter;
    [SerializeField] private GameSettingsProvider? _gameSettingsProvider;
    [SerializeField] private RulesView? _rulesView;
    [SerializeField] private GameButtonObservables? _gameButtonObservables;

    protected override void Configure(IContainerBuilder builder)
    {
      builder.RegisterSceneLifecycle<JudgeSceneLifecycle>();

      builder.Register<EasyRuleSet>(Lifetime.Scoped);
      builder.Register<NormalRuleSet>(Lifetime.Scoped);
      builder.Register<HardRuleSet>(Lifetime.Scoped);
      builder.Register<IRuleSetFactory, RuleSetFactory>(Lifetime.Scoped);
      builder.Register(resolver =>
      {
        var factory = resolver.Resolve<IRuleSetFactory>();
        return factory.Create();
      }, Lifetime.Scoped);

      builder.Register<IJudgeService, JudgeService>(Lifetime.Scoped);

      builder.RegisterEntryPoint<JudgeService>(Lifetime.Scoped);
      builder.RegisterEntryPoint<GameButtonManager>(Lifetime.Scoped);

      builder.Register<ICpuHandStrategy, CyclicStrategy>(Lifetime.Scoped);
      builder.Register<ICpuHandStrategy, ViolationProneStrategy>(Lifetime.Scoped);
      builder.Register<ITimerService, TimerService>(Lifetime.Scoped);
      builder.Register<IStrategySelector, RandomStrategySelector>(Lifetime.Scoped);
      builder.Register<ITurnExecutor, ReactiveTurnExecutor>(Lifetime.Scoped);
      builder.Register<IRandomService, RandomService>(Lifetime.Scoped);
      builder.Register<IScoreCalculator, ScoreCalculator>(Lifetime.Scoped);
      builder.RegisterComponent(_judgeInput).As<IJudgeInput>();
      builder.RegisterComponent(_compositeHandAnimationPresenter).As<ICompositeHandAnimationPresenter>();
      builder.RegisterComponent(_timerRemainsPresenter).As<ITimerRemainsPresenter>();
      builder.RegisterComponent(_currentScorePresenter).As<CurrentScorePresenter>();
      builder.RegisterComponent(_currentJudgesPresenter).As<CurrentJudgesPresenter>();
      builder.RegisterComponent(_remainJudgeTimePresenter).As<RemainJudgeTimePresenter>();
      builder.RegisterInstance(_gameSettingsProvider).As<IGameSettingsProvider>();
      builder.RegisterComponent(_rulesView).As<IRulesView>();
      builder.RegisterComponent(_gameButtonObservables).As<GameButtonObservables>();
    }
  }
}