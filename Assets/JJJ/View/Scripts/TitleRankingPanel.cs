using JJJ.Core.Entities;
using JJJ.UseCase;
using JJJ.Utils;
using TMPro;
using UnityEngine;
using VContainer;
using ZLogger;

namespace JJJ.View
{
  public class TitleRankingPanel : MonoBehaviour
  {
    [SerializeField] private GameMode _gameMode;
    [SerializeField] private Transform _rankingRoot;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private TextMeshProUGUI _failedText;
    [SerializeField] private RankingRow _rankingRowPrefab;

    private RankingUseCase _rankingUseCase = null!;

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<TitleRankingPanel>();

    [Inject]
    public void Construct(RankingUseCase rankingUseCase)
    {
      _rankingUseCase = rankingUseCase;
    }

    public void SetHighScore(int highScore)
    {
      _highScoreText.SetText(highScore < 0 ? "-" : highScore.ToString());
    }

    private async void OnEnable()
    {
      try
      {
        var list = await _rankingUseCase.GetTopNScoresAsync(_gameMode, 5);
        for (int i = 0; i < list.Count; ++i)
        {
          string name = list[i].PlayerName;
          int score = list[i].Score;
          var obj = Instantiate(_rankingRowPrefab, _rankingRoot);
          obj.SetValue(i + 1, name, score);
        }
        _loadingText.gameObject.SetActive(false);
        _rankingRoot.gameObject.SetActive(true);
      }
      catch (System.Exception e)
      {
        _logger.ZLogError($"Failed to load ranking data for {_gameMode} mode.\n{e}");
        _loadingText.gameObject.SetActive(false);
        _failedText.gameObject.SetActive(true);
      }
    }

    private void OnDisable()
    {
      foreach (Transform child in _rankingRoot)
      {
        Destroy(child.gameObject);
      }
      _rankingRoot.gameObject.SetActive(false);
      _loadingText.gameObject.SetActive(true);
      _failedText.gameObject.SetActive(false);
    }
  }
}