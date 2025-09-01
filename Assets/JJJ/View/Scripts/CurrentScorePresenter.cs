using UnityEngine;

public class CurrentScorePresenter : MonoBehaviour
{
  [SerializeField] private TMPro.TextMeshProUGUI _currentScoreText;

  [SerializeField] private int _initialScore = 0;

  private void Start()
  {
    SetCurrentScore(_initialScore);
  }

  public void SetCurrentScore(int currentScore)
  {
    _currentScoreText.text = currentScore.ToString();
  }
}