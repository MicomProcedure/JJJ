using JJJ.Core.Interfaces;
using JJJ.Infrastructure;
using JJJ.UseCase;
using JJJ.Utils;
using MackySoft.Navigathena.SceneManagement;
using MackySoft.Navigathena.Transitions;
using VContainer;
using VContainer.Unity;

namespace JJJ.DI
{
  /// <summary>
  /// ゲーム全体のライフタイムスコープを定義するクラス
  /// </summary>
  public class GameLifetimeScope : LifetimeScope
  {
    /// <summary>
    /// ゲーム全体の依存関係を設定するメソッド
    /// </summary>
    protected override void Configure(IContainerBuilder builder)
    {
      builder.RegisterEntryPoint<OptionProvider>(Lifetime.Singleton).As<IOptionProvider>();
      builder.RegisterEntryPoint<HighScoreProvider>(Lifetime.Singleton).As<IHighScoreProvider>();

      builder.Register<IGameModeProvider, GameModeProvider>(Lifetime.Singleton);
      builder.Register(_ => SceneNavigationUtil.FadeTransitionIdentifier, Lifetime.Singleton).As<ISceneIdentifier>();
      builder.Register<ITransitionDirector, FadeTransitionDirector>(Lifetime.Singleton);
      builder.Register<ISceneManager, SceneManager>(Lifetime.Singleton);
      builder.Register<IRankingRepository, RankingRepository>(Lifetime.Singleton);
      builder.Register<RankingUseCase>(Lifetime.Singleton);
    }
  }
}