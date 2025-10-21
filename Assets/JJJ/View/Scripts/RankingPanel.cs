using Cysharp.Threading.Tasks;
using JJJ.Core.Entities;
using TMPro;
using UnityEngine;

namespace JJJ.View
{
  public class RankingPanel : MonoBehaviour
  {
    [SerializeField] private Transform _rankingRoot;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private RankingRow _rankingRowPrefab;

    public async void SetValue(GameMode gameMode, int highScore)
    {
      _highScoreText.SetText(highScore < 0 ? "-" : highScore.ToString());

      // 難易度に応じてランキングをロード
      await UniTask.Delay(500);

      _loadingText.gameObject.SetActive(false);
    }

    public void HideValue()
    {
      _rankingRoot.gameObject.SetActive(false);
      _loadingText.gameObject.SetActive(true);
    }
  }
}