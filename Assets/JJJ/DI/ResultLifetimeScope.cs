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
    [SerializeField] private RankingRegisterPanel _rankingRegisterView = null!;
    [SerializeField] private TextMeshProUGUI _clickScreenText = null!;
    [SerializeField] private ResultButtonObservables _resultButtonObservables = null!;

    protected override void Configure(IContainerBuilder builder)
    {
      builder.RegisterSceneLifecycle<ResultSceneLifecycle>();

      builder.RegisterEntryPoint<ResultButtonManager>(Lifetime.Scoped);

      builder.RegisterComponent(_resultView);
      builder.RegisterComponent(_rankingRegisterView);
      builder.RegisterComponent(_resultButtonObservables);

      builder.RegisterInstance(_clickScreenText.gameObject).Keyed("ClickScreenText");
    }
  }
}