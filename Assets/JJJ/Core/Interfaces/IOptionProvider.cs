namespace JJJ.Core.Interfaces
{
  /// <summary>
  /// ユーザ設定を提供するインターフェース
  /// </summary>
  public interface IOptionProvider
  {
    /// <summary>
    /// BGMの音量
    /// </summary>
    public float BGMVolume { get; }
    /// <summary>
    /// SEの音量
    /// </summary>
    public float SEVolume { get; }
    /// <summary>
    /// 毎回ランキングに登録するか否か
    /// </summary>
    public bool IsAutoRankingSubmit { get; }
    /// <summary>
    /// ランキング登録時のデフォルトの名前
    /// </summary>
    public string RankingDefaultName { get; }

    public void Set(float bgmVolume,
                    float seVolume,
                    bool isAutoRankingSubmit,
                    string rankingDefaultName);
  }
}