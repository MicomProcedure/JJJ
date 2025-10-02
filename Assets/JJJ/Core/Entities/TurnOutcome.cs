namespace JJJ.Core.Entities
{
  /// <summary>
  /// 1ターンの結果を表現
  /// </summary>
  public readonly struct TurnOutcome
  {
    /// <summary>
    /// このターンの正解の判定
    /// </summary>
    public readonly JudgeResult TruthResult;

    /// <summary>
    /// プレイヤーの主張
    /// </summary>
    public readonly PlayerClaim Claim;
    
    /// <summary>
    /// プレイヤーの判定が正しいかどうか
    /// </summary>
    public readonly bool IsPlayerJudgementCorrect;
    
    /// <summary>
    /// プレイヤーが判定を下すまでにかかった時間（秒）
    /// </summary>
    public readonly double JudgeTime;

    public TurnOutcome(JudgeResult truthResult, PlayerClaim claim, bool isPlayerJudgementCorrect, double judgeTime)
    {
      TruthResult = truthResult;
      Claim = claim;
      IsPlayerJudgementCorrect = isPlayerJudgementCorrect;
      JudgeTime = judgeTime;
    }
  }
}
