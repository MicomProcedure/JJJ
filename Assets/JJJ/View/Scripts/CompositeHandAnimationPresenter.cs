using JJJ.Core.Interfaces;
using JJJ.Utils;
using UnityEngine;
using ZLogger;

namespace JJJ.View
{
  /// <summary>
  /// プレイヤーと相手の手のアニメーションを管理するクラス
  /// </summary>
  public class CompositeHandAnimationPresenter : MonoBehaviour, ICompositeHandAnimationPresenter
  {
    [SerializeField] private HandAnimationPresenter? _playerHandAnimationPresenter;
    [SerializeField] private HandAnimationPresenter? _opponentHandAnimationPresenter;

    public IHandAnimationPresenter? PlayerHandAnimationPresenter => _playerHandAnimationPresenter;
    public IHandAnimationPresenter? OpponentHandAnimationPresenter => _opponentHandAnimationPresenter;

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<CompositeHandAnimationPresenter>();

    public void ResetHandAll()
    {
      _logger.ZLogTrace($"Resetting both player and opponent hands.");
      PlayerHandAnimationPresenter?.ResetHand();
      OpponentHandAnimationPresenter?.ResetHand();
    }

    public void ReturnInitAll()
    {
      _logger.ZLogTrace($"Returning both player and opponent hands to initial state.");
      PlayerHandAnimationPresenter?.ReturnInit();
      OpponentHandAnimationPresenter?.ReturnInit();
    }
  }
}