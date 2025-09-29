using UnityEngine;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using ZLogger;

namespace JJJ.View
{
  /// <summary>
  /// 手のアニメーションを制御するコンポーネント
  /// </summary>
  public class HandAnimationPresenter : MonoBehaviour, IHandAnimationPresenter
  {
    /// <summary>
    /// 手のアニメーションを制御するAnimator
    /// </summary>
    [SerializeField] private Animator? _animator;

    /// <summary>
    /// 手がリセットされているかどうか
    /// </summary>
    private bool _isHandReset = true;

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<HandAnimationPresenter>();

    private void Start()
    {
      if (_animator == null)
      {
        _logger.ZLogError($"Animator is not assigned in HandAnimationPresenter.");
      }
    }

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
        // TODO: Alpha/Betaのアニメーションが実装されたらここを削除する
        if (handType == HandType.Alpha || handType == HandType.Beta)
        {
          _logger.ZLogWarning($"Alpha/Beta are not implemented yet. Playing Rock instead.");
          _animator?.SetTrigger("PlayRock");
        }
        else
        {
          _logger.ZLogDebug($"Playing {handType} hand animation.");
          _animator?.SetTrigger($"Play{handType}");
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
        _logger.ZLogDebug($"Resetting hand animation.");
        _animator?.SetTrigger("DoReset");
        _isHandReset = true;
      }
    }

    /// <summary>
    /// セッションの終わり(勝敗がついたとき)にこれを実行して手を初期位置に戻す
    /// </summary>
    public void ReturnInit()
    {
      _logger.ZLogDebug($"Returning hand to initial position.");
      _animator?.SetTrigger("ReturnInit");
      _isHandReset = true;
    }
  }
}