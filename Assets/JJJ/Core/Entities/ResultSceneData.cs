using MackySoft.Navigathena.SceneManagement;

namespace JJJ.Core.Entities
{
  /// <summary>
  /// リザルトシーンのデータ
  /// </summary>
  public sealed class ResultSceneData : ISceneData
  {
    /// <summary>
    /// スコア
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// 相性のジャッジ正誤数
    /// </summary>
    public (int, int) CompatibilityCount { get; set; }
    /// <summary>
    /// 後出しによる反則のジャッジ正誤数
    /// </summary>
    public (int, int) TimeoutViolationCount { get; set; }
    /// <summary>
    /// 両者とも反則のジャッジ正誤数
    /// </summary>
    public (int, int) DoubleViolationCount { get; set; }
    /// <summary>
    /// ジャッジ時間切れの回数
    /// </summary>
    public int TimeoutCount { get; set; }

    /// <summary>
    /// αのジャッジ正誤数
    /// </summary>
    public (int, int) AlphaCount { get; set; }
    /// <summary>
    /// αの連続使用による反則のジャッジ正誤数
    /// </summary>
    public (int, int) AlphaRepeatCount { get; set; }
    /// <summary>
    /// βの連続使用による反則のジャッジ正誤数
    /// </summary>
    public (int, int) BetaRepeatCount { get; set; }
    /// <summary>
    /// 封印された手の使用による反則のジャッジ正誤数
    /// </summary>
    public (int, int) SealedHandUsedCount { get; set; }
  }
}