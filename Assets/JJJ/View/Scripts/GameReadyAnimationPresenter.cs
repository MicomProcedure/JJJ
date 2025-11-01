using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Interfaces;
using JJJ.Core.Interfaces.UI;
using KanKikuchi.AudioManager;
using UnityEngine;
using VContainer;

namespace JJJ.View.Scripts
{
  public class GameReadyAnimationPresenter : MonoBehaviour, IGameReadyAnimationPresenter
  {
    [SerializeField]
    private GameObject _ready3 = null!;
    [SerializeField]
    private GameObject _ready2 = null!;
    [SerializeField]
    private GameObject _ready1 = null!;
    [SerializeField]
    private GameObject _readyGo = null!;

    [SerializeField, SEPathSelector]
    private string _readySE = "";
    [SerializeField]
    private double _animationInterval = 0.5;

    private IUIInteractivityController _uiInteractivityController = null!;

    [Inject]
    public void Construct(IUIInteractivityController uiInteractivityController)
    {
      _uiInteractivityController = uiInteractivityController;
    }

    private void Awake()
    {
      _ready3.SetActive(false);
      _ready2.SetActive(false);
      _ready1.SetActive(false);
      _readyGo.SetActive(false);
    }

    public async UniTask PlayGameReadyAnimation(CancellationToken cancellationToken = default)
    {
      _ready3.SetActive(true);
      PlayReadySE();
      await UniTask.Delay(TimeSpan.FromSeconds(_animationInterval), cancellationToken: cancellationToken);
      _ready3.SetActive(false);

      _ready2.SetActive(true);
      PlayReadySE();
      await UniTask.Delay(TimeSpan.FromSeconds(_animationInterval), cancellationToken: cancellationToken);
      _ready2.SetActive(false);

      _ready1.SetActive(true);
      PlayReadySE();
      await UniTask.Delay(TimeSpan.FromSeconds(_animationInterval), cancellationToken: cancellationToken);
      _ready1.SetActive(false);

      _readyGo.SetActive(true);
      PlayReadySE();
      await UniTask.Delay(TimeSpan.FromSeconds(_animationInterval), cancellationToken: cancellationToken);
      _readyGo.SetActive(false);

      _uiInteractivityController.EnableAllInteractivity();

      await UniTask.CompletedTask;
    }

    private void PlayReadySE()
    {
      if (!string.IsNullOrEmpty(_readySE)) SEManager.Instance.Play(_readySE);
    }
  }
}