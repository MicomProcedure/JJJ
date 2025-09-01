using UnityEngine;
using UnityEngine.UI;

namespace JJJ.View
{
  public class TimerRemainsPresenter : MonoBehaviour
  {
    [SerializeField] private readonly Image _progressCircle;

    [SerializeField] private readonly Image _timerHand;

    [SerializeField] private readonly float _initialTime = 10f;

    [SerializeField] private readonly TMPro.TextMeshProUGUI _remainTimeText;

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