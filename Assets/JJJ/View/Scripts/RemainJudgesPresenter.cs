using UnityEngine;

namespace JJJ.View
{
  public class RemainJudgeTimePresenter : MonoBehaviour
  {
    [SerializeField] private TMPro.TextMeshProUGUI _remainJudgeTimeText;

    [SerializeField] private int _initialTime = 0;

    private void Start()
    {
      SetRemainJudgeTime(_initialTime);
    }

    public void SetRemainJudgeTime(int remainTime)
    {
      _remainJudgeTimeText.text = remainTime.ToString();
    }
  }
}