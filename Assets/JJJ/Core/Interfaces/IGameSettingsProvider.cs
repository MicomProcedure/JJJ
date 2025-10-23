using JJJ.Core.Entities;

namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// ゲーム設定を提供するインターフェース
  /// </summary>
  public interface IGameSettingsProvider
  {
    /// <summary>
    /// イージーモードのゲーム時間制限（秒）
    /// </summary>
    public int EasyGameTimeLimit { get; }
    /// <summary>
    /// ノーマルモードのゲーム時間制限（秒）
    /// </summary>
    public int NormalGameTimeLimit { get; }
    /// <summary>
    /// ハードモードのゲーム時間制限（秒）
    /// </summary>
    public int HardGameTimeLimit { get; }
    /// <summary>
    /// イージーモードのジャッジ時間制限（秒）
    /// </summary>
    public int EasyJudgeTimeLimit { get; }
    /// <summary>
    /// ノーマルモードのジャッジ時間制限（秒）
    /// </summary>
    public int NormalJudgeTimeLimit { get; }
    /// <summary>
    /// ハードモードのジャッジ時間制限（秒）
    /// </summary>
    public int HardJudgeTimeLimit { get; }

    /// <summary>
    /// CyclicStrategyの反則確率
    /// </summary>
    public double CyclicStrategyViolationProbability { get; }
    /// <summary>
    /// ViolationProneStrategyの後出し反則確率
    /// </summary>
    public double ViolationProneStrategyTimeoutProbability { get; }
    /// <summary>
    /// ViolationProneStrategyのα効果中のα反則確率
    /// </summary>
    public double ViolationProneStrategyAlphaViolationProbability { get; }
    /// <summary>
    /// ViolationProneStrategyのβ効果中のβ反則確率
    /// </summary>
    public double ViolationProneStrategyBetaViolationProbability { get; }

    /// <summary>
    /// 単一シーン起動のデフォルトゲームモード
    /// </summary>
    public GameMode DefaultGameMode { get; }
  }
}