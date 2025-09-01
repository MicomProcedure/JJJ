using UnityEngine;

public class CurrentVersionPresenter : MonoBehaviour
{
  [SerializeField] private TMPro.TextMeshProUGUI _currentVersionText;

  private void Start()
  {
    SetCurrentVersion("ver." + Application.version);
  }

  public void SetCurrentVersion(string currentVersion)
  {
    _currentVersionText.text = currentVersion;
  }
}