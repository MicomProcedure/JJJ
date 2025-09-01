using UnityEngine;

namespace JJJ.View
{
  public class RemainJudgeTimePresenter : MonoBehaviour
  {
    [SerializeField] private readonly TMPro.TextMeshProUGUI _remainJudgeTimeText;

    [SerializeField] private readonly int _initialTime = 0;

    public void SetRemainJudgeTime(int remainTime)
    {
      _remainJudgeTimeText.text = remainTime.ToString();
    }
  }
}