using KanKikuchi.AudioManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JJJ.UI
{
  /// <summary>
  /// ボタンをクリックしたときにSEを再生するコンポーネント
  /// </summary>
  public class ButtonSoundEffector : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
  {
    [SerializeField, SEPathSelector] private string _hoverSE = "";
    [SerializeField, SEPathSelector] private string _clickSE = "";

    private Button? _button;

    private void Awake()
    {
      TryGetComponent<Button>(out _button);
    }

    public void OnPointerEnter(PointerEventData _)
    {
      if (_button != null && !_button.IsInteractable()) return;
      if (!string.IsNullOrEmpty(_hoverSE))
      {
        SEManager.Instance.Play(_hoverSE);
      }
    }

    public void OnPointerDown(PointerEventData _)
    {
      if (_button != null && !_button.IsInteractable()) return;
      if (!string.IsNullOrEmpty(_clickSE))
      {
        SEManager.Instance.Play(_clickSE);
      }
    }
  }
}