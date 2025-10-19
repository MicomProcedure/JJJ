using UnityEngine;

namespace JJJ.View
{
  /// <summary>
  /// 現在のジャッジ数を表示するコンポーネント
  /// </summary>
  public class CurrentJudgesPresenter : MonoBehaviour
  {
    /// <summary>
    /// 現在のジャッジ数を表示するTextMeshProUGUI
    /// </summary>
    [SerializeField] private TMPro.TextMeshProUGUI? _currentJudgesText;

    /// <summary>
    /// 初期のジャッジ数
    /// </summary>
    [SerializeField] private int _initialJudges = 1;

    private void Awake()
    {
      SetCurrentJudges(_initialJudges);
    }

    /// <summary>
    /// 現在のジャッジ数を設定する
    /// </summary>
    /// <param name="currentJudges">現在のジャッジ数</param>
    public void SetCurrentJudges(int currentJudges)
    {
      if (_currentJudgesText == null)
      {
        throw new System.NullReferenceException(nameof(_currentJudgesText));
      }
      _currentJudgesText.text = currentJudges.ToString();
    }
  }
}