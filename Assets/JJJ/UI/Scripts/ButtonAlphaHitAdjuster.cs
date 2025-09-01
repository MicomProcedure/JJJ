using UnityEngine;
using UnityEngine.UI;

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