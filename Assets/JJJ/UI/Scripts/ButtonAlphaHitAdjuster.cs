using UnityEngine;
using UnityEngine.UI;
using JJJ.Utils;
using ZLogger;

namespace JJJ.UI
{
  /// <summary>
  /// ボタンのAlphaHitTestMinimumThresholdを調整するコンポーネント
  /// </summary>
  [RequireComponent(typeof(Image))]
  public class ButtonAlphaHitAdjuster : MonoBehaviour
  {
    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<ButtonAlphaHitAdjuster>();

    private void Awake()
    {
      if (!TryGetComponent<Image>(out var img))
      {
        _logger.ZLogError($"ButtonAlphaHitAdjuster: No Image component found on {gameObject.name}");
        return;
      }

      img.alphaHitTestMinimumThreshold = 0.1f;
    }
  }
}