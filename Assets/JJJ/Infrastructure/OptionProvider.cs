using UnityEngine;
using JJJ.Core.Interfaces;
using KanKikuchi.AudioManager;
using VContainer.Unity;
using JJJ.Core;

namespace JJJ.Infrastructure
{
  public class OptionProvider : IOptionProvider, IStartable
  {
    private float _bgmVolume = 1f;
    public float BGMVolume
    {
      get => _bgmVolume;
      private set => _bgmVolume = Mathf.Clamp01(value);
    }

    private float _seVolume = 1f;
    public float SEVolume
    {
      get => _seVolume;
      private set => _seVolume = Mathf.Clamp01(value);
    }

    public bool IsAutoRankingSubmit { get; private set; } = false;

    private string _rankingDefaultName = string.Empty;
    public string RankingDefaultName
    {
      get => string.IsNullOrEmpty(_rankingDefaultName) ? "名無しさん" : _rankingDefaultName;
      private set => _rankingDefaultName = value;
    }

    public void Start()
    {
      if (SaveFileHandler.TryLoad<Option>(out var data) && data != null)
      {
        Set(data);
      }
      else
      {
        var option = new Option();
        SaveFileHandler.Save(option);
      }
    }

    public void Set(Option option)
    {
      BGMVolume = option.BGMVolume;
      SEVolume = option.SEVolume;
      IsAutoRankingSubmit = option.IsAutoRankingSubmit;
      RankingDefaultName = option.RankingDefaultName;

      ApplyVolume();

      SaveFileHandler.Save(option);
    }

    private void ApplyVolume()
    {
      BGMManager.Instance.ChangeBaseVolume(BGMVolume);
      SEManager.Instance.ChangeBaseVolume(SEVolume);
    }
  }
}