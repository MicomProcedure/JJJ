using UnityEngine;

public class CurrentJudgesPresenter : MonoBehaviour
{
  [SerializeField] private TMPro.TextMeshProUGUI _currentJudgesText;

  [SerializeField] private int _initialJudges = 0;

  private void Start()
  {
    SetCurrentJudges(_initialJudges);
  }

  public void SetCurrentJudges(int currentJudges)
  {
    _currentJudgesText.text = currentJudges.ToString();
  }
}