using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using TMPro;
using UnityEngine;
using VContainer;

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

    private IGameModeProvider _gameModeProvider = null!;

    [Inject]
    public void Construct(IGameModeProvider gameModeProvider)
    {
      _gameModeProvider = gameModeProvider;
    }

    public void SetResult(ResultSceneData resultSceneData)
    {
      int score = resultSceneData.Score;
      var compatibilityCount = resultSceneData.CompatibilityCount;
      var timeoutViolationCount = resultSceneData.TimeoutViolationCount;
      var doubleViolationCount = resultSceneData.DoubleViolationCount;
      int timeoutCount = resultSceneData.TimeoutCount;
      var alphaCount = resultSceneData.AlphaCount;
      var alphaRepeatCount = resultSceneData.AlphaRepeatCount;
      var betaRepeatCount = resultSceneData.BetaRepeatCount;
      var sealedHandUsedCount = resultSceneData.SealedHandUsedCount;

      _scoreText.SetText(score.ToString());
      _compatibility.SetCount(compatibilityCount.Item1, compatibilityCount.Item2);
      _timeoutViolation.SetCount(timeoutViolationCount.Item1, timeoutViolationCount.Item2);
      _doubleViolation.SetCount(doubleViolationCount.Item1, doubleViolationCount.Item2);
      _timeout.SetText(timeoutCount.ToString());

      if (_gameModeProvider.Current == GameMode.Hard)
      {
        _alpha.SetCount(alphaCount.Item1, alphaCount.Item2);
        _alphaRepeat.SetCount(alphaRepeatCount.Item1, alphaRepeatCount.Item2);
        _betaRepeat.SetCount(betaRepeatCount.Item1, betaRepeatCount.Item2);
        _sealedHandUsed.SetCount(sealedHandUsedCount.Item1, sealedHandUsedCount.Item2);
      }
      else
      {
        _alpha.gameObject.SetActive(false);
        _alphaRepeat.gameObject.SetActive(false);
        _betaRepeat.gameObject.SetActive(false);
        _sealedHandUsed.gameObject.SetActive(false);
      }
    }
  }
}