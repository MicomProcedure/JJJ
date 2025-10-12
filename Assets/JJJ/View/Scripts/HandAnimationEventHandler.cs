using UnityEngine;

namespace JJJ.View
{
    public class HandAnimationEventHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _balance;
        [SerializeField] private GameObject _ofuda;

        public void OnPlayBalance()
        {
            _balance.SetActive(true);
        }

        public void OnPlayOfuda()
        {
            _ofuda.SetActive(true);
        }

        public void OnReset()
        {
            _balance.SetActive(false);
            _ofuda.SetActive(false);
        }
    }
}
