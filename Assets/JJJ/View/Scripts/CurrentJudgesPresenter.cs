using UnityEngine;

namespace JJJ.View
{
  public class CurrentJudgesPresenter : MonoBehaviour
  {
    [SerializeField] private readonly TMPro.TextMeshProUGUI _currentJudgesText;

    public void SetCurrentJudges(int currentJudges)
    {
      _currentJudgesText.text = currentJudges.ToString();
    }
  }
}