using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using VContainer.Unity;

namespace JJJ.DI
{
  public sealed class ResultLifetimeScope : LifetimeScope
  {
    protected override void Configure(IContainerBuilder builder)
    {
      builder.RegisterSceneLifecycle<ResultSceneLifecycle>();
    }
  }
}