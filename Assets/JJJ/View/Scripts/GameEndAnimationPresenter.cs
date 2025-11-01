namespace JJJ.View.Scripts
{
  using System;
  using System.Threading;
  using Cysharp.Threading.Tasks;
  using DG.Tweening;
  using JJJ.Core.Interfaces;
  using JJJ.Core.Interfaces.UI;
  using KanKikuchi.AudioManager;
  using UnityEngine;
  using VContainer;

  /// <summary>
  /// ゲーム終了時のアニメーションを担当するプレゼンターの実装
  /// </summary>
  public class GameEndAnimationPresenter : MonoBehaviour, IGameEndAnimationPresenter
  {
    [SerializeField]
    private CanvasGroup _gameEndCg = null!;

    [SerializeField]
    private double _fadeInDuration = 2.0;
    [SerializeField, SEPathSelector]
    private string _endSE = "";

    private IUIInteractivityController _uiInteractivityController = null!;

    [Inject]
    public void Construct(IUIInteractivityController uiInteractivityController)
    {
      _uiInteractivityController = uiInteractivityController;
    }

    private void Awake()
    {
      _gameEndCg.gameObject.SetActive(false);
    }

    /// <summary>
    /// ゲーム終了時のアニメーションを再生する
    /// </summary>
    /// <returns>アニメーション再生完了を待機するためのUniTask</returns>
    public async UniTask PlayGameEndAnimation(CancellationToken cancellationToken = default)
    {
      _uiInteractivityController.DisableAllInteractivity();
      _gameEndCg.gameObject.SetActive(true);
      BGMManager.Instance.Stop();
      if (!string.IsNullOrEmpty(_endSE)) SEManager.Instance.Play(_endSE);

      var sequence = DOTween.Sequence();
      await sequence.Append(_gameEndCg.DOFade(1.0f, (float)_fadeInDuration))
                    .Join(_gameEndCg.transform.DOScale(1.0f, (float)_fadeInDuration).SetEase(Ease.OutExpo).From(0f))
                    .WithCancellation(cancellationToken);
      await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
    }
  }
}