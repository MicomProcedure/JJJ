namespace JJJ.Core.Entities
{
  /// <summary>
  /// スコアのデータを表す構造体
  /// </summary>
  public readonly struct RankingData
  {
    public string PlayerName { get; }
    public int Score { get; }

    public RankingData(string playerName, int score)
    {
      PlayerName = playerName;
      Score = score;
    }
  }
}