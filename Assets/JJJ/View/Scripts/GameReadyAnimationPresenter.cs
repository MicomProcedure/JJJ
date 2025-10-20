using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JJJ.Core.Interfaces;
using KanKikuchi.AudioManager;
using UnityEngine;

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

      await UniTask.CompletedTask;
    }
    
    private void PlayReadySE()
    {
      if (!string.IsNullOrEmpty(_readySE)) SEManager.Instance.Play(_readySE);
    }
  }
}