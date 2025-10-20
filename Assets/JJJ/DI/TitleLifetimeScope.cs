using JJJ.UI;
using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using JJJ.View;
using JJJ.Core.Interfaces;

namespace JJJ.DI
{
  public sealed class TitleLifetimeScope : LifetimeScope
  {
    [SerializeField] private UserSettingsView _userSettingsView = null!;
    [SerializeField] private HelpsView _helpsView = null!;
    [SerializeField] private TitleButtonObservables _titleButtonObservables = null!;

    protected override void Configure(IContainerBuilder builder)
    {
      builder.RegisterSceneLifecycle<TitleSceneLifecycle>();

      builder.RegisterEntryPoint<TitleButtonManager>(Lifetime.Scoped);

      builder.RegisterComponent(_userSettingsView).As<IUserSettingsView>();
      builder.RegisterComponent(_helpsView).As<IVisible>();
      builder.RegisterComponent(_titleButtonObservables);
    }
  }
}