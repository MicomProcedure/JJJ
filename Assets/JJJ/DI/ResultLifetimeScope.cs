using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using JJJ.View;
using JJJ.UI;
using TMPro;

namespace JJJ.DI
{
  public sealed class ResultLifetimeScope : LifetimeScope
  {
    [SerializeField] private ResultView _resultView = null!;
    [SerializeField] private TextMeshProUGUI _clickScreenText = null!;
    [SerializeField] private ResultButtonObservales _resultButtonObservales = null!;

    protected override void Configure(IContainerBuilder builder)
    {
      builder.RegisterSceneLifecycle<ResultSceneLifecycle>();

      builder.RegisterEntryPoint<ResultButtonManager>(Lifetime.Scoped);

      builder.RegisterComponent(_resultView);
      builder.RegisterComponent(_resultButtonObservales);

      builder.RegisterInstance(_clickScreenText.gameObject).Keyed("ClickScreenText");
    }
  }
}