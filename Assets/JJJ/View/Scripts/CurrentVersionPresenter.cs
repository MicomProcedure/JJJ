using UnityEngine;

namespace JJJ.View
{
  public class CurrentVersionPresenter : MonoBehaviour
  {
    [SerializeField] private readonly TMPro.TextMeshProUGUI _currentVersionText;

    public void SetCurrentVersion(string currentVersion)
    {
      _currentVersionText.text = currentVersion;
    }
  }
}