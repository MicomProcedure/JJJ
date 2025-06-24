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
    public string Name { get; private set; }

    /// <summary>
    /// Handクラスのコンストラクタ
    /// </summary>
    /// <param name="type">手の種類</param>
    /// <param name="name">手の名前</param>
    public Hand(HandType type, string name)
    {
      Type = type;
      Name = name;
    }


    // 手を定義する静的プロパティ
    public static readonly Hand Rock = new Hand(HandType.Rock, "グー");
    public static readonly Hand Paper = new Hand(HandType.Paper, "パー");
    public static readonly Hand Scissors = new Hand(HandType.Scissors, "チョキ");
    public static readonly Hand Alpha = new Hand(HandType.Alpha, "α");
    public static readonly Hand Beta = new Hand(HandType.Beta, "β");
    public static readonly Hand One = new Hand(HandType.One, "1");
    public static readonly Hand Two = new Hand(HandType.Two, "2");
    public static readonly Hand Three = new Hand(HandType.Three, "3");
    public static readonly Hand Four = new Hand(HandType.Four, "4");
  }
}