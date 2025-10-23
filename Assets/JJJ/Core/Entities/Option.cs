using System;

namespace JJJ.Core
{
  /// <summary>
  /// ユーザ設定のデータを表すクラス
  /// </summary>
  public class Option
  {
    /// <summary>
    /// BGMの音量
    /// </summary>
    public float BGMVolume;

    /// <summary>
    /// SEの音量
    /// </summary>
    public float SEVolume;

    /// <summary>
    /// 毎回ランキングに登録するか否か
    /// </summary>
    public bool IsAutoRankingSubmit;

    /// <summary>
    /// ランキング登録時のデフォルトの名前
    /// </summary>
    public string RankingDefaultName;

    public Option(float bgmVolume = 0.5f, float seVolume = 0.5f, bool isAutoRankingSubmit = false, string rankingDefaultName = "")
    {
      BGMVolume = Math.Clamp(bgmVolume, 0f, 1f);
      SEVolume = Math.Clamp(seVolume, 0f, 1f);
      IsAutoRankingSubmit = isAutoRankingSubmit;
      RankingDefaultName = rankingDefaultName ?? string.Empty;
    }
  }
}