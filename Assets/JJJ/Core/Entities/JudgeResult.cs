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
    public bool IsValid => Type != JudgeResultType.Violation && Type != JudgeResultType.OpponentViolation && Type != JudgeResultType.DoubleViolation;

    /// <summary>
    /// αの効果によって勝利したプレイヤー
    /// </summary>
    public PersonType? WinByAlpha { get; private set; } = null;

    /// <summary>
    /// プレイヤーの反則時の理由
    /// </summary>
    /// <remarks>
    /// <para>左側の手をプレイヤーの手、右側の手を対戦相手の手とする</para>
    /// <para>反則がない場合は ViolationType.None になる</para>
    /// </remarks>
    public ViolationType PlayerViolationType { get; private set; } = ViolationType.None;

    /// <summary>
    /// 相手の反則時の理由
    /// </summary>
    /// <remarks>
    /// <para>左側の手をプレイヤーの手、右側の手を対戦相手の手とする</para>
    /// <para>反則がない場合は ViolationType.None になる</para>
    /// </remarks>
    public ViolationType OpponentViolationType { get; private set; } = ViolationType.None;

    /// <summary>
    /// JudgeResult クラスのコンストラクタ
    /// </summary>
    /// <param name="type">じゃんけんの結果の種類</param>
    /// <param name="playerHand">プレイヤーの手</param>
    /// <param name="opponentHand">相手の手</param>
    /// <param name="playerViolationType">じゃんけんの結果の理由</param>
    public JudgeResult(JudgeResultType type, Hand playerHand, Hand opponentHand, ViolationType playerViolationType = ViolationType.None, ViolationType opponentViolationType = ViolationType.None, PersonType? winByAlpha = null)
    {
      Type = type;
      PlayerHand = playerHand;
      OpponentHand = opponentHand;
      PlayerViolationType = playerViolationType;
      OpponentViolationType = opponentViolationType;
      WinByAlpha = winByAlpha;
    }
  }
}