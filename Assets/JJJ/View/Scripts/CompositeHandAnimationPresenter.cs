using JJJ.Core.Interfaces;
using UnityEngine;

namespace JJJ.View
{
  public class CompositeHandAnimationPresenter : MonoBehaviour, ICompositeHandAnimationPresenter
  {
    [SerializeField] private HandAnimationPresenter _playerHandAnimationPresenter;
    [SerializeField] private HandAnimationPresenter _opponentHandAnimationPresenter;

    public IHandAnimationPresenter PlayerHandAnimationPresenter => _playerHandAnimationPresenter;
    public IHandAnimationPresenter OpponentHandAnimationPresenter => _opponentHandAnimationPresenter;
  }
}