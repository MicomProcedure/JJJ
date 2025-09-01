using UnityEngine;

namespace JJJ.View
{
  public class RemainJudgeTimePresenter : MonoBehaviour
  {
    [SerializeField] private readonly TMPro.TextMeshProUGUI _remainJudgeTimeText;

    public void SetRemainJudgeTime(int remainTime)
    {
      _remainJudgeTimeText.text = remainTime.ToString();
    }
  }
}