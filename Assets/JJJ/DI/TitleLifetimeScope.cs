using JJJ.UI;
using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace JJJ.DI
{
  public sealed class TitleLifetimeScope : LifetimeScope
  {
    [SerializeField] private TitleButtonObservables _titleButtonObservables = null!;

    protected override void Configure(IContainerBuilder builder)
    {
      builder.RegisterSceneLifecycle<TitleSceneLifecycle>();

      builder.RegisterEntryPoint<TitleButtonManager>(Lifetime.Scoped);

      builder.RegisterComponent(_titleButtonObservables);
    }
  }
}