using JJJ.Utils;
using UnityEngine;
using UnityEngine.UI;
using ZLogger;

namespace JJJ.UI
{
  [RequireComponent(typeof(Image))]
  public class AlphaHitAdjuster : MonoBehaviour
  {

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<AlphaHitAdjuster>();

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