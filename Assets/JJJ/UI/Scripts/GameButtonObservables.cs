using R3;
using UnityEngine;
using UnityEngine.UI;

namespace JJJ.UI
{
  public class GameButtonObservables : MonoBehaviour
  {
    [SerializeField] private Button _exitButton = null!;
    [SerializeField] private Button _ruleButton = null!;
    [SerializeField] private Button _hideRuleButton = null!;

    public Observable<Unit> ExitButtonOnClick => _exitButton.OnClickAsObservable();
    public Observable<Unit> RuleButtonOnClick => _ruleButton.OnClickAsObservable();
    public Observable<Unit> HideRuleButtonOnClick => _hideRuleButton.OnClickAsObservable();
  }
}