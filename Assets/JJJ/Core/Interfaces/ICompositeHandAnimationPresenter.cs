namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// 手のアニメーションをまとめて扱うインターフェース
  /// </summary>
  public interface ICompositeHandAnimationPresenter
  {
    /// <summary>
    /// プレイヤーの手のアニメーションプレゼンター
    /// </summary>
    public IHandAnimationPresenter PlayerHandAnimationPresenter { get; }

    /// <summary>
    /// 対戦相手の手のアニメーションプレゼンター
    /// </summary>
    public IHandAnimationPresenter OpponentHandAnimationPresenter { get; }
  }
}