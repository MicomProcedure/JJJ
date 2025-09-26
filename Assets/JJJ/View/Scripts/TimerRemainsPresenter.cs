using JJJ.Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace JJJ.View
{
  /// <summary>
  /// 残りのタイマー時間を表示するコンポーネント
  /// </summary>
  public class TimerRemainsPresenter : MonoBehaviour, ITimerRemainsPresenter
  {
    /// <summary>
    /// 残り時間を円形で表示するImage
    /// </summary>
    [SerializeField] private Image _progressCircle;

    /// <summary>
    /// タイマーの針を表示するImage
    /// </summary>
    [SerializeField] private Image _timerHand;

    /// <summary>
    /// 初期のタイマー時間（秒）
    /// </summary>
    [SerializeField] private float _initialTime = 10f;

    /// <summary>
    /// 残り時間を表示するTextMeshProUGUI
    /// </summary>
    [SerializeField] private TMPro.TextMeshProUGUI _remainTimeText;

    private void Start()
    {
      SetTimerRemains(_initialTime, _initialTime);
      _remainTimeText.text = Mathf.CeilToInt(_initialTime).ToString();
    }

    /// <summary>
    /// 残りのタイマー時間を設定する
    /// </summary>
    /// <param name="remainTime">残りの時間（秒）</param>
    /// <param name="totalTime">合計の時間（秒）</param>
    public void SetTimerRemains(float remainTime, float totalTime)
    {
      if (totalTime <= 0f)
      {
        _progressCircle.fillAmount = 0f;
        _timerHand.transform.localEulerAngles = Vector3.zero;
        _remainTimeText.text = "0";
        return;
      }

      _progressCircle.fillAmount = Mathf.Clamp01(remainTime / totalTime);
      float angle = Mathf.Clamp01(remainTime / totalTime) * 360f;
      _timerHand.transform.localEulerAngles = new Vector3(0f, 0f, -angle);
      _remainTimeText.text = Mathf.CeilToInt(remainTime).ToString();
    }
  }
}