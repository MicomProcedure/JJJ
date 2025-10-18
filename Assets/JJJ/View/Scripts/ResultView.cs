using TMPro;
using UnityEngine;

namespace JJJ.View
{
  public class ResultView : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _scoreText = null!;

    [SerializeField] private ResultRow _compatibility = null!;
    [SerializeField] private ResultRow _timeoutViolation = null!;
    [SerializeField] private ResultRow _doubleViolation = null!;
    [SerializeField] private TextMeshProUGUI _timeout = null!;

    [SerializeField] private ResultRow _alpha = null!;
    [SerializeField] private ResultRow _alphaRepeat = null!;
    [SerializeField] private ResultRow _betaRepeat = null!;
    [SerializeField] private ResultRow _sealedHandUsed = null!;

    public void SetScore(int score)
    {
      _scoreText.SetText(score.ToString());
    }

    public void SetResult(Vector2Int compatibilityCount,
                          Vector2Int timeoutViolationCount,
                          Vector2Int doubleViolationCount,
                          int timeoutCount,
                          Vector2Int? alphaCount,
                          Vector2Int? alphaRepeatCount,
                          Vector2Int? betaRepeatCount,
                          Vector2Int? sealedHandUsedCount)

    {
      _compatibility.SetCount(compatibilityCount.x, compatibilityCount.y);
      _timeoutViolation.SetCount(timeoutViolationCount.x, timeoutViolationCount.y);
      _doubleViolation.SetCount(doubleViolationCount.x, doubleViolationCount.y);
      _timeout.SetText(timeoutCount.ToString());

      if (alphaCount != null) _alpha.SetCount(alphaCount.Value.x, alphaCount.Value.y);
      else _alpha.gameObject.SetActive(false);

      if (alphaRepeatCount != null) _alphaRepeat.SetCount(alphaRepeatCount.Value.x, alphaRepeatCount.Value.y);
      else _alphaRepeat.gameObject.SetActive(false);

      if (betaRepeatCount != null) _betaRepeat.SetCount(betaRepeatCount.Value.x, betaRepeatCount.Value.y);
      else _betaRepeat.gameObject.SetActive(false);

      if (sealedHandUsedCount != null) _sealedHandUsed.SetCount(sealedHandUsedCount.Value.x, sealedHandUsedCount.Value.y);
      else _sealedHandUsed.gameObject.SetActive(false);
    }
  }
}