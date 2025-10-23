using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JJJ.Core.Interfaces;
using UnityEngine;

namespace JJJ.View
{
  /// <summary>
  /// フェードによるシーン遷移演出を表示するコンポーネント
  /// </summary>
  public sealed class FadeTransitionDirectorBehaviour : MonoBehaviour, ITransitionEffect
  {
    private float _fadeDuration = 0.5f;
    private Ease _fadeEase = Ease.OutCubic;
    [SerializeField] private CanvasGroup _cg = null!;

    public UniTask StartTransition(CancellationToken cancellationToken = default)
    {
      return _cg.DOFade(1f, _fadeDuration)
        .SetEase(_fadeEase)
        .ToUniTask(cancellationToken: cancellationToken);
    }

    public UniTask EndTransition(CancellationToken cancellationToken = default)
    {
      return _cg.DOFade(0f, _fadeDuration)
        .SetEase(_fadeEase)
        .ToUniTask(cancellationToken: cancellationToken);
    }
  }
}