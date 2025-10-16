using KanKikuchi.AudioManager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JJJ.UI
{
  /// <summary>
  /// ボタンをクリックしたときにSEを再生するコンポーネント
  /// </summary>
  public class ButtonSoundEffector : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
  {
    [SerializeField, SEPathSelector] private string _hoverSE = "";
    [SerializeField, SEPathSelector] private string _clickSE = "";

    public void OnPointerEnter(PointerEventData _)
    {
      if (!string.IsNullOrEmpty(_hoverSE))
      {
        SEManager.Instance.Play(_hoverSE);
      }
    }

    public void OnPointerClick(PointerEventData _)
    {
      if (!string.IsNullOrEmpty(_clickSE))
      {
        SEManager.Instance.Play(_clickSE);
      }
    }
  }
}