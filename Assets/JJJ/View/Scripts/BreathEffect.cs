using UnityEngine;
using DG.Tweening;

namespace JJJ.View
{
  /// <summary>
  /// GameObjectをふわふわ拡縮させるエフェクト
  /// </summary>
  public class BreathEffect : MonoBehaviour
  {
    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _scaleRange = 0.05f;

    private void OnEnable()
    {
      transform
        .DOScale(Vector3.one * (1f + _scaleRange), _duration)
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine);
    }

    private void OnDisable()
    {
      DOTween.Kill(transform);
    }
  }
}
