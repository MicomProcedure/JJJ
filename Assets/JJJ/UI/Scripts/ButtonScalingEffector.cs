using DG.Tweening;
using JJJ.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZLogger;

namespace JJJ.UI
{
  /// <summary>
  /// ボタンをクリックしたときにスケールアニメーションを実行するコンポーネント
  /// </summary>
  public class ButtonScalingEffector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
  {
    private Vector3 _originalScale;
    private float _hoverScale = 1.1f;
    private float _clickScale = 0.8f;

    private Tweener? _tweener = null;
    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<ButtonScalingEffector>();

    private void Awake()
    {
      _originalScale = transform.localScale;

      if (!TryGetComponent<Button>(out var button))
      {
        _logger.ZLogError($"ButtonScalingEffector: No Button component found on {gameObject.name}");
        return;
      }
    }

    public void OnPointerEnter(PointerEventData _)
    {
      AnimateScale(_hoverScale);
    }

    public void OnPointerExit(PointerEventData _)
    {
      AnimateScale(1f);
    }

    public void OnPointerClick(PointerEventData _)
    {
      ResetAnimation();

      _tweener = transform.DOPunchScale(
        punch: (Vector3.one * _clickScale) - _originalScale,
        duration: 0.2f,
        vibrato: 1
      ).SetEase(Ease.OutExpo);
    }

    private void ResetAnimation()
    {
      if (_tweener != null && _tweener.IsActive() && _tweener.IsPlaying())
      {
        _tweener.Kill();
        _tweener = null;
        transform.localScale = _originalScale;
      }
    }

    private void AnimateScale(float targetScale)
    {
      ResetAnimation();

      _tweener = transform.DOScale(_originalScale * targetScale, 0.1f).SetEase(Ease.OutQuad);
    }
  }
}