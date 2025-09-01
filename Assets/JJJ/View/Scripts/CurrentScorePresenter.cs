using UnityEngine;

namespace JJJ.View
{
  public class CurrentScorePresenter : MonoBehaviour
  {
    [SerializeField] private readonly TMPro.TextMeshProUGUI _currentScoreText;

    public void SetCurrentScore(int currentScore)
    {
      _currentScoreText.text = currentScore.ToString();
    }
  }
}