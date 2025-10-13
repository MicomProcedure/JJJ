using UnityEngine;
using JJJ.Core.Entities;

namespace JJJ.View
{
  /// <summary>
  /// 手のアニメーションを制御するコンポーネント
  /// </summary>
  public class HandAnimationPresenter : MonoBehaviour
  {
    /// <summary>
    /// この手がプレイヤーのものかどうか
    /// </summary>
    [SerializeField] private bool _isPlayerHand = true;

    /// <summary>
    /// この手が右手かどうか
    /// </summary>
    [SerializeField] private bool _isRightHand = true;

    /// <summary>
    /// 手のアニメーションを制御するAnimator
    /// </summary>
    [SerializeField] private Animator _handAnimator;

    /// <summary>
    /// 手がリセットされているかどうか
    /// </summary>
    private bool _isHandReset = true;

    /// <summary>
    /// 指定した手のアニメーションを再生する
    /// </summary>
    /// <remarks>
    /// 最初の場合は、自動的に初回用の振り下ろすアニメーションが再生される
    /// </remarks>
    public void PlayHand(HandType handType)
    {
      if (_isHandReset)
      {
        if (handType == HandType.Beta)
        {
          if (_isPlayerHand ^ _isRightHand)
          {
            _handAnimator.SetTrigger("PlayBetaL");
          }
          else
          {
            _handAnimator.SetTrigger("PlayBetaR");
          }
        }
        else
        {
          _handAnimator.SetTrigger($"Play{handType}");
        }
        _isHandReset = false;
      }
    }

    /// <summary>
    /// ある手を出した状態からリセットして、次の手を出せる状態にする
    /// </summary>
    /// <remarks>
    /// これをいったん実行しないと、次の手を出せない
    /// </remarks>
    public void ResetHand()
    {
      if (!_isHandReset)
      {
        _handAnimator.SetTrigger("DoReset");
        _isHandReset = true;
      }
    }

    /// <summary>
    /// セッションの終わり(勝敗がついたとき)にこれを実行して手を初期位置に戻す
    /// </summary>
    public void ReturnInit()
    {
      _handAnimator.SetTrigger("ReturnInit");
      _isHandReset = true;
    }
  }
}