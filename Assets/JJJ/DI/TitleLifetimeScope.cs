using JJJ.UI;
using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using JJJ.View;
using JJJ.Core.Interfaces;
using JJJ.Core.Interfaces.UI;

namespace JJJ.DI
{
  public sealed class TitleLifetimeScope : LifetimeScope
  {
    [SerializeField] private RulesView _rulesView = null!;
    [SerializeField] private OptionView _optionView = null!;
    [SerializeField] private HelpsView _helpsView = null!;
    [SerializeField] private RankingsView _rankingsView = null!;
    [SerializeField] private TitleButtonObservables _titleButtonObservables = null!;
    [SerializeField] private UIInteractivityController _uiInteractivityController = null!;

    protected override void Configure(IContainerBuilder builder)
    {
      builder.RegisterSceneLifecycle<TitleSceneLifecycle>();

      builder.RegisterEntryPoint<TitleButtonManager>(Lifetime.Scoped);

      builder.RegisterComponent(_rulesView).As<IRulesView>();
      builder.RegisterComponent(_optionView).As<IOptionView>();
      builder.RegisterComponent(_helpsView).As<IHelpsView>();
      builder.RegisterComponent(_rankingsView).As<IRankingsView>();
      builder.RegisterComponent(_titleButtonObservables);
      builder.RegisterComponent(_uiInteractivityController).As<IUIInteractivityController>();
    }
  }
}