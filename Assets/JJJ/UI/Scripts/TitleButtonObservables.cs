using R3;
using UnityEngine;
using UnityEngine.UI;

namespace JJJ.UI
{
  public class TitleButtonObservables : MonoBehaviour
  {
    [SerializeField] private Button _easyButton = null!;
    [SerializeField] private Button _normalButton = null!;
    [SerializeField] private Button _hardButton = null!;
    [SerializeField] private Button _exitButton = null!;
    [SerializeField] private Button _optionButton = null!;
    [SerializeField] private Button _helpButton = null!;
    [SerializeField] private Button _rankingButton = null!;
    [SerializeField] private Button _hidePanelButton = null!;
    [SerializeField] private Button _hideUserSettingsButton = null!;

    public Observable<Unit> EasyButtonOnClick => _easyButton.OnClickAsObservable();
    public Observable<Unit> NormalButtonOnClick => _normalButton.OnClickAsObservable();
    public Observable<Unit> HardButtonOnClick => _hardButton.OnClickAsObservable();
    public Observable<Unit> ExitButtonOnClick => _exitButton.OnClickAsObservable();
    public Observable<Unit> OptionButtonOnClick => _optionButton.OnClickAsObservable();
    public Observable<Unit> HelpButtonOnClick => _helpButton.OnClickAsObservable();
    public Observable<Unit> RankingButtonOnClick => _rankingButton.OnClickAsObservable();
    public Observable<Unit> HidePanelButtonOnClick => _hidePanelButton.OnClickAsObservable();
    public Observable<Unit> HideUserSettingsButtonOnClick => _hideUserSettingsButton.OnClickAsObservable();
  }
}