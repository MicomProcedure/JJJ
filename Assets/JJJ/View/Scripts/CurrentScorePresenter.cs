using DG.Tweening;
using UnityEngine;

namespace JJJ.View
{
  /// <summary>
  /// 現在のスコアを表示するコンポーネント
  /// </summary>
  public class CurrentScorePresenter : MonoBehaviour
  {
    /// <summary>
    /// 現在のスコアを表示するTextMeshProUGUI
    /// </summary>
    [SerializeField] private TMPro.TextMeshProUGUI? _currentScoreText;

    /// <summary>
    /// スコアの増減を表示するTextMeshProUGUI
    /// </summary>
    [SerializeField] private TMPro.TextMeshProUGUI? _scoreDiffText;

    /// <summary>
    /// 初期のスコア
    /// </summary>
    [SerializeField] private int _initialScore = 0;

    private Vector3 _initialPosition = Vector3.zero;
    private Tweener? _tweener;

    private void Awake()
    {
      if (_currentScoreText == null)
      {
        throw new System.NullReferenceException(nameof(_currentScoreText));
      }
      if (_scoreDiffText == null)
      {
        throw new System.NullReferenceException(nameof(_scoreDiffText));
      }
      SetCurrentScore(_initialScore);
      _initialPosition = _scoreDiffText.transform.localPosition;
    }

    /// <summary>
    /// 現在のスコアを設定する
    /// </summary>
    /// <param name="currentScore">現在のスコア</param>
    public void SetCurrentScore(int currentScore)
    {
      if (_currentScoreText == null)
      {
        throw new System.NullReferenceException(nameof(_currentScoreText));
      }
      _currentScoreText.text = currentScore.ToString();
    }

    public void SetScoreDiff(int scoreDiff)
    {
      if (_scoreDiffText == null)
      {
        throw new System.NullReferenceException(nameof(_scoreDiffText));
      }
      if (scoreDiff > 0)
      {
        _scoreDiffText.text = $"+{scoreDiff}";
        _scoreDiffText.color = Color.green;
      }
      else if (scoreDiff < 0)
      {
        _scoreDiffText.text = scoreDiff.ToString();
        _scoreDiffText.color = Color.red;
      }
      else
      {
        _scoreDiffText.text = "0";
        _scoreDiffText.color = Color.white;
      }

      // スコアの増減表示を一時的に表示し、数秒後に非表示にする
      // 下から上にDOTweenでアニメーションさせる, Y方向に30ピクセル移動
      if (_tweener != null && _tweener.IsActive() && _tweener.IsPlaying())
      {
        _tweener.Kill();
        _tweener = null;
      }
      _scoreDiffText.gameObject.SetActive(true);
      _scoreDiffText.transform.localPosition = _initialPosition;

      _tweener = _scoreDiffText.transform.DOLocalMoveY(_initialPosition.y + 30f, 1.0f).SetEase(Ease.OutCubic).OnComplete(() =>
      {
        _scoreDiffText.gameObject.SetActive(false);
      });
    }

  }
}