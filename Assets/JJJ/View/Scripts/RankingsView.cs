using JJJ.Core.Entities;
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

      _easyRankingPanel.SetValue(GameMode.Easy, highScore);
      _normalRankingPanel.SetValue(GameMode.Normal, highScore);
      _hardRankingPanel.SetValue(GameMode.Hard, highScore);

      gameObject.SetActive(true);
    }

    public void Hide()
    {
      gameObject.SetActive(false);

      _easyRankingPanel.HideValue();
      _normalRankingPanel.HideValue();
      _hardRankingPanel.HideValue();
    }
  }
}