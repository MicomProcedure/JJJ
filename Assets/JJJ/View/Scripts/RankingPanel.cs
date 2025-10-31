using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
using JJJ.UseCase;
using TMPro;
using UnityEngine;
using VContainer;

namespace JJJ.View
{
  public class RankingPanel : MonoBehaviour
  {
    [SerializeField] private GameMode _gameMode;
    [SerializeField] private Transform _rankingRoot;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private TextMeshProUGUI _failedText;
    [SerializeField] private RankingRow _rankingRowPrefab;

    private RankingUseCase _rankingUseCase = null!;

    [Inject]
    public void Construct(RankingUseCase rankingUseCase)
    {
      _rankingUseCase = rankingUseCase;
    }

    public void SetHighScore(int highScore)
    {
      _highScoreText.SetText(highScore < 0 ? "-" : highScore.ToString());
    }

    private void OnEnable()
    {
      var task = _rankingUseCase.GetTopNScoresAsync(_gameMode, 5);
      task.ContinueWith(list =>
      {
        if (task.Status == UniTaskStatus.Faulted)
        {
          _loadingText.gameObject.SetActive(false);
          _failedText.gameObject.SetActive(true);
        }
        else
        {
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
      });
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