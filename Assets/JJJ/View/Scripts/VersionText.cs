using TMPro;
using UnityEngine;

namespace JJJ.View
{
  [RequireComponent(typeof(TextMeshProUGUI))]
  public class VersionText : MonoBehaviour
  {
    private void Awake()
    {
      var versionText = GetComponent<TextMeshProUGUI>();
      versionText.SetText($"ver. {Application.version}");
    }
  }
}