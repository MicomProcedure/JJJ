using UnityEngine;
using ZLogger;
using JJJ.Utils;
using Cysharp.Threading.Tasks;

namespace JJJ.View
{
    public class HandAnimationEventHandler : MonoBehaviour
    {
        [SerializeField] private HandAnimationPresenter _handAnimationPresenter;
        [SerializeField] private GameObject _balance = null!;
        [SerializeField] private GameObject _ofuda = null!;

        private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<HandAnimationEventHandler>();

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

        public void OnIsTimeout()
        {
            if (_handAnimationPresenter == null)
            {
                _logger.ZLogError($"HandAnimationPresenter is not assigned in HandAnimationEventHandler.");
                return;
            }
            
            _handAnimationPresenter.DoTimeout().Forget();
        }
    }
}
