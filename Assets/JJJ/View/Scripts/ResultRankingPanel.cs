using JJJ.Core.Interfaces;
using JJJ.UseCase;
using TMPro;
using UnityEngine;
using VContainer;

namespace JJJ.View
{
  public class ResultRankingPanel : MonoBehaviour
  {
    [SerializeField] private Transform _rankingRoot;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private TextMeshProUGUI _failedText;
    [SerializeField] private RankingRow _rankingRowPrefab;

    private IGameModeProvider _gameModeProvider = null!;
    private RankingUseCase _rankingUseCase = null!;

    [Inject]
    public void Construct(IGameModeProvider gameModeProvider, RankingUseCase rankingUseCase)
    {
      _gameModeProvider = gameModeProvider;
      _rankingUseCase = rankingUseCase;
    }

    private async void OnEnable()
    {
      _loadingText.gameObject.SetActive(true);
      _failedText.gameObject.SetActive(false);
      try
      {
        var list = await _rankingUseCase.GetTopNScoresAsync(_gameModeProvider.Current, 3);
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
      catch (System.Exception)
      {
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