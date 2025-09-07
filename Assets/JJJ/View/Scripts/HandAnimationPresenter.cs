using UnityEngine;

namespace JJJ.View
{
    public class HandAnimationPresenter : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private void Start()
        {
            PlayHand();
        }

        public void PlayHand()
        {
            _animator.SetTrigger($"PlayFour");
        }
    }
}