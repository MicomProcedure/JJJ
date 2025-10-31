using JJJ.Core.Entities;
using ProcRanking;
using TMPro;
using UnityEngine;
using System;
using JJJ.Utils;
using Cysharp.Threading.Tasks;

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

    public void SetHighScore(int highScore)
    {
      _highScoreText.SetText(highScore < 0 ? "-" : highScore.ToString());
    }

    private void OnEnable()
    {
      var task = ProcRaUtil.LoadTopNAsync<ProcRaData>(_gameMode, 5);
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
            string name = Convert.ToString(list[i]["name"]);
            int score = Convert.ToInt32(list[i]["score"]);
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