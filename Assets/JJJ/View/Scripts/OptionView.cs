using JJJ.Core;
using JJJ.Core.Interfaces;
using KanKikuchi.AudioManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JJJ.View
{
  public class OptionView : MonoBehaviour, IOptionView
  {
    [SerializeField] private Slider _bgmVolumeSlider = null!;
    [SerializeField] private TextMeshProUGUI _bgmValueText = null!;
    [SerializeField] private Slider _seVolumeSlider = null!;
    [SerializeField] private TextMeshProUGUI _seValueText = null!;
    [SerializeField] private Toggle _isAutoRankingSubmit = null!;
    [SerializeField] private TMP_InputField _rankingDefaultName = null!;

    public Option Option => new(
      _bgmVolumeSlider.value,
      _seVolumeSlider.value,
      _isAutoRankingSubmit.isOn,
      _rankingDefaultName.text
    );

    private void OnEnable()
    {
      _bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
      _seVolumeSlider.onValueChanged.AddListener(OnSEVolumeChanged);
    }

    private void OnDisable()
    {
      _bgmVolumeSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
      _seVolumeSlider.onValueChanged.RemoveListener(OnSEVolumeChanged);
    }

    public void SetValue(float bgmVolume,
                         float seVolume,
                         bool isAutoRankingSubmit,
                         string rankingDefaultName)
    {
      _bgmVolumeSlider.value = bgmVolume;
      SetBGMVolumeText();

      _seVolumeSlider.value = seVolume;
      SetSEVolumeText();

      _isAutoRankingSubmit.isOn = isAutoRankingSubmit;
      _rankingDefaultName.text = rankingDefaultName == "名無しさん" ? string.Empty : rankingDefaultName;
    }

    public void Show()
    {
      gameObject.SetActive(true);
    }

    public void Hide()
    {
      gameObject.SetActive(false);
    }

    private void SetBGMVolumeText()
    {
      int percent = (int)(_bgmVolumeSlider.value * 100);
      _bgmValueText.SetText($"{percent}%");
    }

    private void SetSEVolumeText()
    {
      int percent = (int)(_seVolumeSlider.value * 100);
      _seValueText.SetText($"{percent}%");
    }

    private void OnBGMVolumeChanged(float value)
    {
      BGMManager.Instance.ChangeBaseVolume(value);
      SetBGMVolumeText();
    }

    private void OnSEVolumeChanged(float value)
    {
      SEManager.Instance.ChangeBaseVolume(value);
      SetSEVolumeText();
    }
  }
}