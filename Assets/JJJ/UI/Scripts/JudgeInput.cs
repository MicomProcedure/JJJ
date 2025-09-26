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
    private JudgeButton _playerWinButton;

    [SerializeField]
    private JudgeButton _opponentWinButton;

    [SerializeField]
    private JudgeButton _drawButton;
    
    public Observable<Unit> PlayerWinObservable => _playerWinButton.OnClickObservable;
    public Observable<Unit> OpponentWinObservable => _opponentWinButton.OnClickObservable;
    public Observable<Unit> DrawObservable => _drawButton.OnClickObservable;
  }
}