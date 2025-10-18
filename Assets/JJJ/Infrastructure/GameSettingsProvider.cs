using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using UnityEngine;

namespace JJJ.Infrastructure
{
  [CreateAssetMenu(fileName = "GameSettingsProvider", menuName = "JJJ/GameSettingsProvider")]
  public class GameSettingsProvider : ScriptableObject, IGameSettingsProvider
  {
    [field: SerializeField]
    public int EasyGameTimeLimit { get; private set; } = 60;
    [field: SerializeField]
    public int NormalGameTimeLimit { get; private set; } = 45;
    [field: SerializeField]
    public int HardGameTimeLimit { get; private set; } = 30;

    [field: SerializeField]
    public int EasyJudgeTimeLimit { get; private set; } = 5;
    [field: SerializeField]
    public int NormalJudgeTimeLimit { get; private set; } = 4;
    [field: SerializeField]
    public int HardJudgeTimeLimit { get; private set; } = 3;

    [field: SerializeField]
    public double CyclicStrategyViolationProbability { get; private set; } = 0.1;

    [field: SerializeField]
    public double ViolationProneStrategyTimeoutProbability { get; private set; } = 0.1;

    [field: SerializeField]
    public double ViolationProneStrategyAlphaViolationProbability { get; private set; } = 0.1;

    [field: SerializeField]
    public double ViolationProneStrategyBetaViolationProbability { get; private set; } = 0.1;

    [field: SerializeField]
    public GameMode DefaultGameMode { get; private set; } = GameMode.Normal;
  }
}