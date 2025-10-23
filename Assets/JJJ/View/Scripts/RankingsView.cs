using JJJ.Core.Interfaces;
using UnityEngine;
using VContainer;

namespace JJJ.View
{
  public class RankingsView : MonoBehaviour, IRankingsView
  {
    [SerializeField] private RankingPanel _easyRankingPanel;
    [SerializeField] private RankingPanel _normalRankingPanel;
    [SerializeField] private RankingPanel _hardRankingPanel;

    private IHighScoreProvider _highScoreProvider = null!;

    [Inject]
    public void Construct(IHighScoreProvider highScoreProvider)
    {
      _highScoreProvider = highScoreProvider;
    }

    public void Show()
    {
      _easyRankingPanel.SetHighScore(_highScoreProvider.HighScoreEasy);
      _normalRankingPanel.SetHighScore(_highScoreProvider.HighScoreNormal);
      _hardRankingPanel.SetHighScore(_highScoreProvider.HighScoreHard);
      gameObject.SetActive(true);
    }

    public void Hide()
    {
      gameObject.SetActive(false);
    }
  }
}