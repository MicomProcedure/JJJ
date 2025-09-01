using UnityEngine;

namespace JJJ.View
{
  public class CurrentJudgesPresenter : MonoBehaviour
  {
    [SerializeField] private readonly TMPro.TextMeshProUGUI _currentJudgesText;

    [SerializeField] private readonly int _initialJudges = 0;

    public void SetCurrentJudges(int currentJudges)
    {
      _currentJudgesText.text = currentJudges.ToString();
    }
  }
}