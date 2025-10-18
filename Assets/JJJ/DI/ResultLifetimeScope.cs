using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using JJJ.View;

namespace JJJ.DI
{
  public sealed class ResultLifetimeScope : LifetimeScope
  {
    [SerializeField] private ResultView _resultView;

    protected override void Configure(IContainerBuilder builder)
    {
      builder.RegisterSceneLifecycle<ResultSceneLifecycle>();

      builder.RegisterComponent(_resultView);
    }
  }
}