using System.Threading;
using Cysharp.Threading.Tasks;
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

    public UniTask ResetHandAll(CancellationToken cancellationToken = default)
    {
      _logger.ZLogTrace($"Resetting both player and opponent hands.");
      return UniTask.WhenAll(
        PlayerHandAnimationPresenter?.ResetHand(cancellationToken) ?? UniTask.CompletedTask,
        OpponentHandAnimationPresenter?.ResetHand(cancellationToken) ?? UniTask.CompletedTask
      );
    }

    public UniTask ReturnInitAll(CancellationToken cancellationToken = default)
    {
      _logger.ZLogTrace($"Returning both player and opponent hands to initial state.");
      return UniTask.WhenAll(
        PlayerHandAnimationPresenter?.ReturnInit(cancellationToken) ?? UniTask.CompletedTask,
        OpponentHandAnimationPresenter?.ReturnInit(cancellationToken) ?? UniTask.CompletedTask
      );
    }

    public void SelectDominantHandAll()
    {
      _logger.ZLogTrace($"Selecting dominant hands for both player and opponent.");
      PlayerHandAnimationPresenter?.SelectDominantHand();
      OpponentHandAnimationPresenter?.SelectDominantHand();
    }
  }
}