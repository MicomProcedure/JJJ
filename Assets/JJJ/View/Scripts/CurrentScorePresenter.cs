using UnityEngine;

namespace JJJ.View
{
  /// <summary>
  /// 現在のスコアを表示するコンポーネント
  /// </summary>
  public class CurrentScorePresenter : MonoBehaviour
  {
    /// <summary>
    /// 現在のスコアを表示するTextMeshProUGUI
    /// </summary>
    [SerializeField] private TMPro.TextMeshProUGUI _currentScoreText;

    /// <summary>
    /// 初期のスコア
    /// </summary>
    [SerializeField] private int _initialScore = 0;

    private void Start()
    {
      SetCurrentScore(_initialScore);
    }

    /// <summary>
    /// 現在のスコアを設定する
    /// </summary>
    /// <param name="currentScore">現在のスコア</param>
    public void SetCurrentScore(int currentScore)
    {
      _currentScoreText.text = currentScore.ToString();
    }
  }
}