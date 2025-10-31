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

    [SerializeField] private GameObject _contentsRoot;
    [SerializeField] private GameObject _rankingComponent;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TMP_InputField _rankingName;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private TextMeshProUGUI _succeedText;
    [SerializeField] private TextMeshProUGUI _failedText;
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

    public void ShowLoading()
    {
      _rankingComponent.SetActive(false);
      _loadingText.gameObject.SetActive(true);
      _succeedText.gameObject.SetActive(false);
      _failedText.gameObject.SetActive(false);
    }

    public void ShowSucceed()
    {
      _rankingComponent.SetActive(false);
      _loadingText.gameObject.SetActive(false);
      _succeedText.gameObject.SetActive(true);
      _failedText.gameObject.SetActive(false);
    }

    public void ShowFailed()
    {
      _rankingComponent.SetActive(false);
      _loadingText.gameObject.SetActive(false);
      _succeedText.gameObject.SetActive(false);
      _failedText.gameObject.SetActive(true);
    }

    public void ShowButtons()
    {
      _yesButton.gameObject.SetActive(true);
      _noButton.gameObject.SetActive(true);
    }

    public void HideButtons()
    {
      _yesButton.gameObject.SetActive(false);
      _noButton.gameObject.SetActive(false);
    }

    public void ShowRanking()
    {
      _rankingComponent.SetActive(true);
      _loadingText.gameObject.SetActive(false);
      _succeedText.gameObject.SetActive(false);
      _failedText.gameObject.SetActive(false);
    }

    public void EnableRetryMode()
    {
      _yesButton.GetComponentInChildren<TextMeshProUGUI>().SetText("再送信");
    }

    private void Start()
    {
      _rankingName.text = _optionProvider.RankingDefaultName;

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