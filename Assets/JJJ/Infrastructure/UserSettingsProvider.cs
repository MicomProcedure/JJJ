using UnityEngine;
using JJJ.Core.Interfaces;
using KanKikuchi.AudioManager;
using VContainer.Unity;

namespace JJJ.Infrastructure
{
  public class UserSettingsProvider : IUserSettingsProvider, IStartable
  {
    [SerializeField] private float _bgmVolume = 1f;
    public float BGMVolume
    {
      get => _bgmVolume;
      private set => _bgmVolume = Mathf.Clamp01(value);
    }

    [SerializeField] private float _seVolume = 1f;
    public float SEVolume
    {
      get => _seVolume;
      private set => _seVolume = Mathf.Clamp01(value);
    }

    [field: SerializeField]
    public bool IsAutoRankingSubmit { get; private set; } = false;

    [SerializeField] private string _rankingDefaultName = string.Empty;
    public string RankingDefaultName
    {
      get => string.IsNullOrEmpty(_rankingDefaultName) ? "名無しさん" : _rankingDefaultName;
      private set => _rankingDefaultName = value;
    }

    public void Start()
    {
      if (SaveFileHandler.TryLoad(out var data) && data != null)
      {
        Set(data.BGMVolume,
            data.SEVolume,
            data.IsAutoRankingSubmit,
            data.RankingDefaultName);
      }
      else
      {
        SaveFileHandler.Save(this);
      }
    }

    public void Set(float bgmVolume,
                    float seVolume,
                    bool isAutoRankingSubmit,
                    string rankingDefaultName)
    {
      BGMVolume = bgmVolume;
      SEVolume = seVolume;
      IsAutoRankingSubmit = isAutoRankingSubmit;
      RankingDefaultName = rankingDefaultName;

      ApplyVolume();

      SaveFileHandler.Save(this);
    }

    private void ApplyVolume()
    {
      BGMManager.Instance.ChangeBaseVolume(BGMVolume);
      SEManager.Instance.ChangeBaseVolume(SEVolume);
    }
  }
}