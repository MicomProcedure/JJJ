using JJJ.Core.Interfaces;
using UnityEngine;

namespace JJJ.View
{
  public class RankingsView : MonoBehaviour, IRankingsView
  {
    [SerializeField] private RankingPanel _easyRankingPanel;
    [SerializeField] private RankingPanel _normalRankingPanel;
    [SerializeField] private RankingPanel _hardRankingPanel;

    public void Show()
    {
      // ハイスコアを取得
      int highScore = -1;

      _easyRankingPanel.SetHighScore(highScore);
      _normalRankingPanel.SetHighScore(highScore);
      _hardRankingPanel.SetHighScore(highScore);

      gameObject.SetActive(true);
    }

    public void Hide()
    {
      gameObject.SetActive(false);
    }
  }
}