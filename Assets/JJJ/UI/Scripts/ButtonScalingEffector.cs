using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using JJJ.Utils;
using ZLogger;

namespace JJJ.UI
{
  /// <summary>
  /// ボタンをクリックしたときにスケールアニメーションを実行するコンポーネント
  /// </summary>
  [RequireComponent(typeof(Button))]
  public class ButtonScalingEffector : MonoBehaviour
  {
    private Tweener? _tweener = null;
    private Vector3 _originalScale;
    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<ButtonScalingEffector>();

    private void Awake()
    {
      _originalScale = transform.localScale;
    }

    private void Start()
    {
      if (!TryGetComponent<Button>(out var button))
      {
        _logger.ZLogError($"ButtonScalingEffector: No Button component found on {gameObject.name}");
        return;
      }

      button.onClick.AddListener(() =>
      {
        // 既存のアニメーションが実行中なら停止して元のスケールに戻す
        if (_tweener != null && _tweener.IsActive() && _tweener.IsPlaying())
        {
          _tweener.Kill();
          _tweener = null;
          transform.localScale = _originalScale;
        }
        
        // パンチスケールアニメーションを実行
        _tweener = transform.DOPunchScale(
          punch: Vector3.one * 0.1f,
          duration: 0.2f,
          vibrato: 1
        ).SetEase(Ease.OutExpo);
      });
    }
  }
}