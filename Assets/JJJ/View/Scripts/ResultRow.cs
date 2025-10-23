using TMPro;
using UnityEngine;

namespace JJJ.View
{
  public class ResultRow : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _correctCountText = null!;
    [SerializeField] private TextMeshProUGUI _incorrectCountText = null!;

    public void SetCount(int correctCount, int incorrectCount)
    {
      _correctCountText.SetText(correctCount.ToString());
      _incorrectCountText.SetText(incorrectCount.ToString());
    }
  }
}