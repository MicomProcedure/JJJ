using JJJ.Core.Interfaces;
using JJJ.Infrastructure;
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
      builder.Register<IGameModeProvider, GameModeProvider>(Lifetime.Singleton);

      builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
      {
        // entryPoints.Add<Test>();
      });
    }
  }
}