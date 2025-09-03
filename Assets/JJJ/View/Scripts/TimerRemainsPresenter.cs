using UnityEngine;
using UnityEngine.UI;

namespace JJJ.View
{
  public class TimerRemainsPresenter : MonoBehaviour
  {
    [SerializeField] private Image _progressCircle;

    [SerializeField] private Image _timerHand;

    [SerializeField] private float _initialTime = 10f;

    [SerializeField] private TMPro.TextMeshProUGUI _remainTimeText;


    private void Start()
    {
      SetTimerRemains(_initialTime, _initialTime);
      _remainTimeText.text = Mathf.CeilToInt(_initialTime).ToString();
    }

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