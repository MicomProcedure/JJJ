namespace JJJ.Core.Entities
{
  /// <summary>
  /// 手の種類を表すクラス
  /// </summary>
  public class Hand
  {
    /// <summary>
    /// 手の種類
    /// </summary>
    public HandType Type { get; private set; }

    /// <summary>
    /// 手の名前
    /// </summary>
    public string Name => Type switch
    {
      HandType.Rock => "グー",
      HandType.Paper => "パー",
      HandType.Scissors => "チョキ",
      HandType.Alpha => "α",
      HandType.Beta => "β",
      HandType.One => "1",
      HandType.Two => "2",
      HandType.Three => "3",
      HandType.Four => "4",
      _ => throw new System.ArgumentOutOfRangeException()
    };

    /// <summary>
    /// 後出しかどうか
    /// </summary>
    public bool IsTimeout { get; private set; }

    /// <summary>
    /// Handクラスのコンストラクタ
    /// </summary>
    /// <param name="type">手の種類</param>
    public Hand(HandType type, bool isTimeout = false)
    {
      Type = type;
      IsTimeout = isTimeout;
    }


    // 手を定義する静的プロパティ
    public static readonly Hand Rock = new Hand(HandType.Rock);
    public static readonly Hand Paper = new Hand(HandType.Paper);
    public static readonly Hand Scissors = new Hand(HandType.Scissors);
    public static readonly Hand Alpha = new Hand(HandType.Alpha);
    public static readonly Hand Beta = new Hand(HandType.Beta);
    public static readonly Hand One = new Hand(HandType.One);
    public static readonly Hand Two = new Hand(HandType.Two);
    public static readonly Hand Three = new Hand(HandType.Three);
    public static readonly Hand Four = new Hand(HandType.Four);
  }
}