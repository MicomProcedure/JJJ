namespace JJJ.Core.Entities
{
  /// <summary>
  /// 1ターンの結果を表現
  /// </summary>
  public readonly struct TurnOutcome
  {
    public readonly JudgeResult TruthResult;
    public readonly PlayerClaim Claim;
    public readonly bool IsPlayerJudgementCorrect;

    public TurnOutcome(JudgeResult truthResult, PlayerClaim claim, bool isPlayerJudgementCorrect)
    {
      TruthResult = truthResult;
      Claim = claim;
      IsPlayerJudgementCorrect = isPlayerJudgementCorrect;
    }
  }
}
