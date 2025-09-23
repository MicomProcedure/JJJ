using UnityEngine;

namespace JJJ.View
{
  /// <summary>
  /// 現在のバージョンを表示するコンポーネント
  /// </summary>
  public class CurrentVersionPresenter : MonoBehaviour
  {
    /// <summary>
    /// 現在のバージョンを表示するTextMeshProUGUI
    /// </summary>
    [SerializeField] private TMPro.TextMeshProUGUI _currentVersionText;

    private void Start()
    {
      SetCurrentVersion("ver." + Application.version);
    }

    /// <summary>
    /// 現在のバージョンを設定する
    /// </summary>
    /// <param name="currentVersion">現在のバージョン名</param>
    public void SetCurrentVersion(string currentVersion)
    {
      _currentVersionText.text = currentVersion;
    }
  }
}