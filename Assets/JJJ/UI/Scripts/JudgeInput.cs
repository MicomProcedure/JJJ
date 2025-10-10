using R3;
using JJJ.Core.Interfaces;
using UnityEngine;

namespace JJJ.UI.Scripts
{
  /// <summary>
  /// プレイヤーの勝利・敗北・引き分け入力を提供するコンポーネント
  /// </summary>
  public class JudgeInput : MonoBehaviour, IJudgeInput
  {
    [SerializeField]
    private JudgeButton? _playerWinButton;

    [SerializeField]
    private JudgeButton? _opponentWinButton;

    [SerializeField]
    private JudgeButton? _drawButton;

    public Observable<Unit>? PlayerWinObservable => _playerWinButton != null ? _playerWinButton.OnClickObservable : null;
    public Observable<Unit>? OpponentWinObservable => _opponentWinButton != null ? _opponentWinButton.OnClickObservable : null;
    public Observable<Unit>? DrawObservable => _drawButton != null ? _drawButton.OnClickObservable : null;

    public void SetInputEnabled(bool enabled)
    {
      if (_playerWinButton == null) throw new System.NullReferenceException(nameof(_playerWinButton));
      if (_opponentWinButton == null) throw new System.NullReferenceException(nameof(_opponentWinButton));
      if (_drawButton == null) throw new System.NullReferenceException(nameof(_drawButton));

      _playerWinButton.GetComponent<UnityEngine.UI.Button>().interactable = enabled;
      _opponentWinButton.GetComponent<UnityEngine.UI.Button>().interactable = enabled;
      _drawButton.GetComponent<UnityEngine.UI.Button>().interactable = enabled;
    }
  }
}