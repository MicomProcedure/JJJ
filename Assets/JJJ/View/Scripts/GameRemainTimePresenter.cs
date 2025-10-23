using UnityEngine;

namespace JJJ.View
{
  /// <summary>
  /// 残りのゲーム時間を表示するコンポーネント
  /// </summary>
  public class GameRemainTimePresenter : MonoBehaviour
  {
    /// <summary>
    /// 残りのゲーム時間を表示するTextMeshProUGUI
    /// </summary>
    [SerializeField] private TMPro.TextMeshProUGUI? _remainGameTimeText;

    /// <summary>
    /// 初期のゲーム時間
    /// </summary>
    [SerializeField] private int _initialTime = 0;

    private void Awake()
    {
      SetRemainGameTime(_initialTime);
    }

    /// <summary>
    /// 残りのゲーム時間を設定する
    /// </summary>
    /// <param name="remainTime">残りのゲーム時間</param>
    public void SetRemainGameTime(int remainTime)
    {
      if (_remainGameTimeText == null)
      {
        throw new System.NullReferenceException(nameof(_remainGameTimeText));
      }
      _remainGameTimeText.text = remainTime.ToString();
    }
  }
}