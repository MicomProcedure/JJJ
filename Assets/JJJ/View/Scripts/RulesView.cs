using JJJ.Core.Entities;
using JJJ.Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace JJJ.View
{
  public class RulesView : MonoBehaviour, IRulesView
  {
    [SerializeField] private Button _hideRuleButton = null!;
    [SerializeField] private GameObject _easyRuleRoot = null!;
    [SerializeField] private GameObject _normalRuleRoot = null!;
    [SerializeField] private GameObject _hardRuleRoot = null!;

    public void Show(GameMode gameMode)
    {
      _hideRuleButton.gameObject.SetActive(true);
      switch (gameMode)
      {
        case GameMode.Easy:
          _easyRuleRoot.SetActive(true);
          break;
        case GameMode.Normal:
          _normalRuleRoot.SetActive(true);
          break;
        case GameMode.Hard:
          _hardRuleRoot.SetActive(true);
          break;
      }
    }

    public void Hide()
    {
      _hideRuleButton.gameObject.SetActive(false);
      _easyRuleRoot.SetActive(false);
      _normalRuleRoot.SetActive(false);
      _hardRuleRoot.SetActive(false);
    }
  }
}