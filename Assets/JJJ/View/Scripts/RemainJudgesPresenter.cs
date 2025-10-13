using UnityEngine;

namespace JJJ.View
{
  /// <summary>
  /// 残りのジャッジ時間を表示するコンポーネント
  /// </summary>
  public class RemainJudgeTimePresenter : MonoBehaviour
  {
    /// <summary>
    /// 残りのジャッジ時間を表示するTextMeshProUGUI
    /// </summary>
    [SerializeField] private TMPro.TextMeshProUGUI? _remainJudgeTimeText;

    /// <summary>
    /// 初期のジャッジ時間
    /// </summary>
    [SerializeField] private int _initialTime = 0;

    private void Start()
    {
      SetRemainJudgeTime(_initialTime);
    }

    /// <summary>
    /// 残りのジャッジ時間を設定する
    /// </summary>
    /// <param name="remainTime">残りのジャッジ時間</param>
    public void SetRemainJudgeTime(int remainTime)
    {
      if (_remainJudgeTimeText == null)
      {
        throw new System.NullReferenceException(nameof(_remainJudgeTimeText));
      }
      _remainJudgeTimeText.text = remainTime.ToString();
    }
  }
}