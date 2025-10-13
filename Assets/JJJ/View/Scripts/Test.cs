using JJJ.View;
using JJJ.Core.Entities;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private HandAnimationPresenter _handAnimationPresenter;

    public void Rcok()
    {
        _handAnimationPresenter.PlayHand(HandType.Rock);
    }

    public void Scissors()
    {
        _handAnimationPresenter.PlayHand(HandType.Scissors);
    }

    public void Paper()
    {
        _handAnimationPresenter.PlayHand(HandType.Paper);
    }

    public void One()
    {
        _handAnimationPresenter.PlayHand(HandType.One);
    }

    public void Two()
    {
        _handAnimationPresenter.PlayHand(HandType.Two);
    }

    public void Three()
    {
        _handAnimationPresenter.PlayHand(HandType.Three);
    }

    public void Four()
    {
        _handAnimationPresenter.PlayHand(HandType.Four);
    }

    public void Alpha()
    {
        _handAnimationPresenter.PlayHand(HandType.Alpha);
    }

    public void Beta()
    {
        _handAnimationPresenter.PlayHand(HandType.Beta);
    }

    public void Reset()
    {
        _handAnimationPresenter.ResetHand();
    }

    public void Init()
    {
        _handAnimationPresenter.ReturnInit();
    }
}
