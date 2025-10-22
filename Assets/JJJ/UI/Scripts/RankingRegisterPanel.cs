using JJJ.Core.Interfaces;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace JJJ.UI
{
  public class RankingRegisterPanel : MonoBehaviour
  {
    public int Score { get; private set; }

    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TMP_InputField _rankingName;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    public Observable<(bool, string)> OnSubmit => _onSubmit;
    private Subject<(bool, string)> _onSubmit = new();

    private IOptionProvider _optionProvider = null!;

    [Inject]
    public void Construct(IOptionProvider optionProvider)
    {
      _optionProvider = optionProvider;
    }

    public void SetScore(int score)
    {
      Score = score;
      _scoreText.SetText(Score.ToString());
    }

    public void Show()
    {
      gameObject.SetActive(true);
    }

    private void Start()
    {
      _rankingName.text = _optionProvider.RankingDefaultName == "名無しさん" ? string.Empty : _optionProvider.RankingDefaultName;

      Observable.Merge(
         _yesButton.OnClickAsObservable().Select(_ => true),
         _noButton.OnClickAsObservable().Select(_ => false)
       )
       .Subscribe(result =>
       {
         _onSubmit.OnNext((result, _rankingName.text));
       });
    }
  }
}