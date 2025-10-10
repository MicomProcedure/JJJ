using UnityEngine;
using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using JJJ.Utils;
using ZLogger;
using Cysharp.Threading.Tasks;
using System.Threading;

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

    private string _currentState = string.Empty;

    private static readonly int InitHash = Animator.StringToHash("Init");
    private static readonly int ResetHash = Animator.StringToHash("Reset");

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
    public async UniTask PlayHand(HandType handType, CancellationToken cancellationToken = default)
    {
      if (_animator == null)
      {
        _logger.ZLogError($"Animator is not assigned in HandAnimationPresenter.");
        await UniTask.CompletedTask;
        return;
      }
      if (_isHandReset)
      {
        // TODO: Alpha/Betaのアニメーションが実装されたらここを削除する
        if (handType == HandType.Alpha || handType == HandType.Beta)
        {
          _logger.ZLogWarning($"Alpha/Beta are not implemented yet. Playing Rock instead.");
          _animator.SetTrigger("PlayRock");
          _currentState = "PlayRock";
        }
        else
        {
          _logger.ZLogDebug($"Playing {handType} hand animation.");
          _animator.SetTrigger($"Play{handType}");
          _currentState = $"Play{handType}";
        }
        await UniTask.WaitUntil(() =>
        {
          if (_animator == null) return false;
          var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
          return stateInfo.normalizedTime >= 1.0f;
        }, cancellationToken: CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.GetCancellationTokenOnDestroy()).Token);
      }
      else
      {
        _logger.ZLogWarning($"Hand is not reset. Cannot play hand animation.");
      }
      _logger.ZLogDebug($"Hand animation for {handType} completed.");
      _isHandReset = false;
      await UniTask.CompletedTask;
    }

    /// <summary>
    /// ある手を出した状態からリセットして、次の手を出せる状態にする
    /// </summary>
    /// <remarks>
    /// これをいったん実行しないと、次の手を出せない
    /// </remarks>
    public UniTask ResetHand(CancellationToken cancellationToken = default)
    {
      if (_animator == null)
      {
        _logger.ZLogError($"Animator is not assigned in HandAnimationPresenter.");
        return UniTask.CompletedTask;
      }
      if (!_isHandReset)
      {
        _logger.ZLogDebug($"Resetting hand animation.");
        _animator.ResetTrigger(_currentState);
        _animator.SetTrigger("DoReset");
        _isHandReset = true;
        return UniTask.WaitUntil(() =>
        {
          var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
          return stateInfo.shortNameHash == ResetHash && stateInfo.normalizedTime >= 1.0f;
        }, cancellationToken: CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.GetCancellationTokenOnDestroy()).Token);
      }
      else
      {
        _logger.ZLogWarning($"Hand is already reset. No need to reset again.");
        return UniTask.CompletedTask;
      }
    }

    /// <summary>
    /// セッションの終わり(勝敗がついたとき)にこれを実行して手を初期位置に戻す
    /// </summary>
    public UniTask ReturnInit(CancellationToken cancellationToken = default)
    {
      _logger.ZLogDebug($"Returning hand to initial position.");
      if (_animator == null)
      {
        _logger.ZLogError($"Animator is not assigned in HandAnimationPresenter.");
        return UniTask.CompletedTask;
      }
      _animator.ResetTrigger(_currentState);
      _animator.SetTrigger("ReturnInit");
      _isHandReset = true;
      return UniTask.WaitUntil(() =>
      {
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.shortNameHash == InitHash && stateInfo.normalizedTime >= 1.0f;
      }, cancellationToken: CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.GetCancellationTokenOnDestroy()).Token);
    }
  }
}