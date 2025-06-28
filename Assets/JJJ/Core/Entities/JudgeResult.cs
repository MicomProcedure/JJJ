namespace JJJ.Core.Entities
{
  /// <summary>
  /// じゃんけんの結果を表すクラス
  /// </summary>
  public class JudgeResult
  {
    /// <summary>
    /// じゃんけんの結果の種類
    /// </summary>
    public JudgeResultType Type { get; private set; }

    /// <summary>
    /// プレイヤーの手
    /// </summary>
    /// <remarks>
    /// 左側の手をプレイヤーの手、右側の手を対戦相手の手とする
    /// </remarks>
    public Hand PlayerHand { get; private set; }

    /// <summary>
    /// 相手の手
    /// </summary>
    /// <remarks>
    /// 左側の手をプレイヤーの手、右側の手を対戦相手の手とする
    /// </remarks>
    public Hand OpponentHand { get; private set; }

    /// <summary>
    /// じゃんけんの結果が反則かどうか
    /// </summary>
    public bool IsValid => Type != JudgeResultType.Violation && Type != JudgeResultType.DoubleViolation;

    /// <summary>
    /// 反則時の理由
    /// </summary>
    /// <remarks>
    /// 反則がない場合は ViolationType.None になる
    /// </remarks>
    public ViolationType ViolationType { get; private set; } = ViolationType.None;

    /// <summary>
    /// JudgeResult クラスのコンストラクタ
    /// </summary>
    /// <param name="type">じゃんけんの結果の種類</param>
    /// <param name="playerHand">プレイヤーの手</param>
    /// <param name="opponentHand">相手の手</param>
    /// <param name="reason">じゃんけんの結果の理由</param>
    public JudgeResult(JudgeResultType type, Hand playerHand, Hand opponentHand, ViolationType violationType = ViolationType.None)
    {
      Type = type;
      ViolationType = violationType;
      PlayerHand = playerHand;
      OpponentHand = opponentHand;
    }
  }
}