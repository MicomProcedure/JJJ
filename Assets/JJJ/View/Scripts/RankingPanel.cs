using JJJ.Core.Entities;
using ProcRanking;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;
using JJJ.Utils;

namespace JJJ.View
{
  public class RankingPanel : MonoBehaviour
  {
    [SerializeField] private GameMode _gameMode;
    [SerializeField] private Transform _rankingRoot;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private RankingRow _rankingRowPrefab;

    public void SetHighScore(int highScore)
    {
      _highScoreText.SetText(highScore < 0 ? "-" : highScore.ToString());
    }

    private void OnEnable()
    {
      var query = new ProcRaQuery<ProcRaData>(ProcRaUtil.StoreName(_gameMode))
        .SetLimit(5)
        .SetDescSort("score");

      query.FindAsync((List<ProcRaData> foundList, ProcRaException e) =>
      {
        if (e != null)
        {
          // TODO: 通信エラーの旨を表示する
        }
        else
        {
          for (int i = 0; i < foundList.Count; ++i)
          {
            string name = Convert.ToString(foundList[i]["name"]);
            int score = Convert.ToInt32(foundList[i]["score"]);
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
    }
  }
}