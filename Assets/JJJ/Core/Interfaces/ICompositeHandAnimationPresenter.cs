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

    /// <summary>
    /// 両者の手のアニメーションをリセットする
    /// </summary>
    public void ResetHandAll();

    /// <summary>
    /// 両者の手のアニメーションを初期位置に戻す
    /// </summary>
    public void ReturnInitAll();
  }
}