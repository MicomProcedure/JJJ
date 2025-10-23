using TMPro;
using UnityEngine;

namespace JJJ.View
{
  public class RankingRow : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void SetValue(int rank, string name, int score)
    {
      _rankText.SetText($"{rank} 位");
      _nameText.SetText(name);
      _scoreText.SetText(score.ToString());
    }
  }
}