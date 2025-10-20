namespace JJJ.Core.Interfaces
{
  public interface IOptionView
  {
    public Option Option { get; }

    public void SetValue(float bgmVolume,
                         float seVolume,
                         bool isAutoRankingSubmit,
                         string rankingDefaultName);

    public void Show();
    public void Hide();
  }
}