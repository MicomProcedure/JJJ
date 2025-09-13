using UnityEngine;
using JJJ.Core.Entities;

namespace JJJ.View
{
    public class HandAnimationPresenter : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private bool _isHandReset = true;

        /// <summary>
        ///  指定した手のアニメーションを再生する
        /// </summary>
        /// <remarks>
        /// 最初の場合は、自動的に初回用の振り下ろすアニメーションが再生される
        /// </remarks>
        public void PlayHand(HandType handType)
        {
            if (_isHandReset)
            {
                _animator.SetTrigger($"Play{handType}");
            }
        }

        /// <summary>
        ///  ある手を出した状態からリセットして、次の手を出せる状態にする
        /// </summary>
        /// <remarks>
        /// これをいったん実行しないと、次の手を出せない
        /// </remarks>
        public void ResetHand()
        {
            if (!_isHandReset)
            {
                _animator.SetTrigger("DoReset");
                _isHandReset = true;
            }
        }
    }
}