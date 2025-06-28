using JJJ.Core.Interfaces;
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
      // ここに依存関係のバインディングを追加
      builder.Register<IJudgeService, JudgeService>(Lifetime.Singleton);
      builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
      {
        entryPoints.Add<Test>();
      });
    }
  }
}