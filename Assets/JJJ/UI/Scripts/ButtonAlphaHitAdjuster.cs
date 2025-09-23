using UnityEngine;
using UnityEngine.UI;

namespace JJJ.UI
{
  /// <summary>
  /// ボタンのAlphaHitTestMinimumThresholdを調整するコンポーネント
  /// </summary>
  [RequireComponent(typeof(Image))]
  public class ButtonAlphaHitAdjuster : MonoBehaviour
  {
    private void Awake()
    {
      if (!TryGetComponent<Image>(out var img))
      {
        Debug.LogWarning("ButtonAlphaHitAdjuster: No Image component found on " + gameObject.name);
        return;
      }

      img.alphaHitTestMinimumThreshold = 0.1f;
    }
  }
}