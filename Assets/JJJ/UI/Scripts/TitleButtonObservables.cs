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
    [SerializeField] private Button _easyRuleButton = null!;
    [SerializeField] private Button _normalRuleButton = null!;
    [SerializeField] private Button _hardRuleButton = null!;
    [SerializeField] private Button _hideRuleButton = null!;
    [SerializeField] private Button _exitButton = null!;
    [SerializeField] private Button _optionButton = null!;
    [SerializeField] private Button _helpButton = null!;
    [SerializeField] private Button _rankingButton = null!;
    [SerializeField] private Button _hideOptionButton = null!;
    [SerializeField] private Button _hideHelpsButton = null!;
    [SerializeField] private Button _hideRankingsButton = null!;

    public Observable<Unit> EasyButtonOnClick => _easyButton.OnClickAsObservable();
    public Observable<Unit> NormalButtonOnClick => _normalButton.OnClickAsObservable();
    public Observable<Unit> HardButtonOnClick => _hardButton.OnClickAsObservable();
    public Observable<Unit> EasyRuleButtonOnClick => _easyRuleButton.OnClickAsObservable();
    public Observable<Unit> NormalRuleButtonOnClick => _normalRuleButton.OnClickAsObservable();
    public Observable<Unit> HardRuleButtonOnClick => _hardRuleButton.OnClickAsObservable();
    public Observable<Unit> HideRuleButtonOnClick => _hideRuleButton.OnClickAsObservable();
    public Observable<Unit> ExitButtonOnClick => _exitButton.OnClickAsObservable();
    public Observable<Unit> OptionButtonOnClick => _optionButton.OnClickAsObservable();
    public Observable<Unit> HelpButtonOnClick => _helpButton.OnClickAsObservable();
    public Observable<Unit> RankingButtonOnClick => _rankingButton.OnClickAsObservable();
    public Observable<Unit> HideOptionButtonOnClick => _hideOptionButton.OnClickAsObservable();
    public Observable<Unit> HideHelpsButtonOnClick => _hideHelpsButton.OnClickAsObservable();
    public Observable<Unit> HideRankingsButtonOnClick => _hideRankingsButton.OnClickAsObservable();
  }
}