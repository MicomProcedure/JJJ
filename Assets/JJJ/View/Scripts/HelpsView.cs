using JJJ.Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace JJJ.View
{
  public class HelpsView : MonoBehaviour, IVisible
  {
    [SerializeField] private Button _hidePanelButton = null!;

    public void Show()
    {
      _hidePanelButton.gameObject.SetActive(true);
      gameObject.SetActive(true);
    }

    public void Hide()
    {
      _hidePanelButton.gameObject.SetActive(false);
      gameObject.SetActive(false);
    }
  }
}