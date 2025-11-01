using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using JJJ.View;
using JJJ.UI;
using TMPro;
using JJJ.Core.Interfaces.UI;

namespace JJJ.DI
{
  public sealed class ResultLifetimeScope : LifetimeScope
  {
    [SerializeField] private ResultView _resultView = null!;
    [SerializeField] private RankingRegisterPanel _rankingRegisterView = null!;
    [SerializeField] private TextMeshProUGUI _clickScreenText = null!;
    [SerializeField] private ResultButtonObservables _resultButtonObservables = null!;
    [SerializeField] private UIInteractivityController _uiInteractivityController = null!;

    protected override void Configure(IContainerBuilder builder)
    {
      builder.RegisterSceneLifecycle<ResultSceneLifecycle>();

      builder.RegisterEntryPoint<ResultButtonManager>(Lifetime.Scoped);

      builder.RegisterComponent(_resultView);
      builder.RegisterComponent(_rankingRegisterView);
      builder.RegisterComponent(_resultButtonObservables);
      builder.RegisterComponent(_uiInteractivityController).As<IUIInteractivityController>();

      builder.RegisterInstance(_clickScreenText.gameObject).Keyed("ClickScreenText");
    }
  }
}