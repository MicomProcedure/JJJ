using UnityEngine;
using UnityEngine.EventSystems;

namespace JJJ.UI
{
  public class BringToFrontOnClick : MonoBehaviour, IPointerClickHandler
  {
    public void OnPointerClick(PointerEventData eventData)
    {
      transform.SetAsLastSibling();
    }
  }
}