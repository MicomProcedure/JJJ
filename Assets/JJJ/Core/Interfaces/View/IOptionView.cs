namespace JJJ.Core.Interfaces
{
  public interface IOptionView : IVisible
  {
    public Option Option { get; }

    public void SetValue(float bgmVolume,
                         float seVolume,
                         bool isAutoRankingSubmit,
                         string rankingDefaultName);
  }
}