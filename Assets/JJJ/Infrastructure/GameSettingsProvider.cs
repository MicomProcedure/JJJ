using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using UnityEngine;

namespace JJJ.Infrastructure
{
  [CreateAssetMenu(fileName = "GameSettingsProvider", menuName = "JJJ/GameSettingsProvider")]
  public class GameSettingsProvider : ScriptableObject, IGameSettingsProvider
  {
    [field: Header("ゲーム時間制限（秒）")]
    [field: SerializeField]
    [field: Tooltip("イージーモードのゲーム時間制限（秒）")]
    public int EasyGameTimeLimit { get; private set; } = 60;
    [field: SerializeField]
    [field: Tooltip("ノーマルモードのゲーム時間制限（秒）")]
    public int NormalGameTimeLimit { get; private set; } = 45;
    [field: SerializeField]
    [field: Tooltip("ハードモードのゲーム時間制限（秒）")]
    public int HardGameTimeLimit { get; private set; } = 30;

    [field: Header("ジャッジ時間制限（秒）")]
    [field: SerializeField]
    [field: Tooltip("イージーモードのジャッジ時間制限（秒）")]
    public int EasyJudgeTimeLimit { get; private set; } = 5;
    [field: SerializeField]
    [field: Tooltip("ノーマルモードのジャッジ時間制限（秒）")]
    public int NormalJudgeTimeLimit { get; private set; } = 4;
    [field: SerializeField]
    [field: Tooltip("ハードモードのジャッジ時間制限（秒）")]
    public int HardJudgeTimeLimit { get; private set; } = 3;

    [field: Header("CPU戦略設定")]
    [field: SerializeField]
    [field: Tooltip("CyclicStrategyの違反確率")]
    public double CyclicStrategyViolationProbability { get; private set; } = 0.1;

    [field: SerializeField]
    [field: Tooltip("ViolationProneStrategyの後出し確率")]
    public double ViolationProneStrategyTimeoutProbability { get; private set; } = 0.1;

    [field: SerializeField]
    [field: Tooltip("ViolationProneStrategyのα効果中のα違反確率")]
    public double ViolationProneStrategyAlphaViolationProbability { get; private set; } = 0.1;

    [field: SerializeField]
    [field: Tooltip("ViolationProneStrategyのβ効果中のβ違反確率")]
    public double ViolationProneStrategyBetaViolationProbability { get; private set; } = 0.1;

    [field: Header("その他設定")]
    [field: SerializeField]
    [field: Tooltip("単一シーン起動のデフォルトゲームモード")]
    public GameMode DefaultGameMode { get; private set; } = GameMode.Normal;
  }
}