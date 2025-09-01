using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

namespace JJJ.UI
{
  [RequireComponent(typeof(Button))]
  public class ButtonScalingEffector : MonoBehaviour
  {
    private Tweener _tweener = null;
    private Vector3 _originalScale;
  }
}