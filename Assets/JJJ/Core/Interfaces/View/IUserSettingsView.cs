namespace JJJ.Core.Interfaces
{
  public interface IUserSettingsView
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

    public void SetValue(float bgmVolume,
                         float seVolume,
                         bool isAutoRankingSubmit,
                         string rankingDefaultName);

    public void Show();
    public void Hide();
  }
}