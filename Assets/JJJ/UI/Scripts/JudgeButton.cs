using R3;
using UnityEngine;
using UnityEngine.UI;

namespace JJJ.UI
{
  [RequireComponent(typeof(Button))]
  public class JudgeButton : MonoBehaviour
  {
    private readonly Subject<Unit> _onClickSubject = new Subject<Unit>();
    public Observable<Unit> OnClickObservable => _onClickSubject.AsObservable();

    private void Awake()
    {
      if (TryGetComponent<Button>(out var button))
      {
        button.onClick.AddListener(() => _onClickSubject.OnNext(Unit.Default));
      }
    }

  }
}