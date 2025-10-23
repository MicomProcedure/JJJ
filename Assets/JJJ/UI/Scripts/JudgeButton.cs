using R3;
using UnityEngine;
using UnityEngine.UI;

namespace JJJ.UI
{
  /// <summary>
  /// ボタンのクリックをObservableで購読できるようにするコンポーネント
  /// </summary>
  [RequireComponent(typeof(Button))]
  public class JudgeButton : MonoBehaviour
  {
    /// <summary>
    /// クリックイベントのSubject
    /// </summary>
    private readonly Subject<Unit> _onClickSubject = new Subject<Unit>();

    /// <summary>
    /// クリックイベントのObservableを取得する
    /// </summary>
    public Observable<Unit> OnClickObservable => _onClickSubject.AsObservable();

    private void Awake()
    {
      if (TryGetComponent<Button>(out var button))
      {
        // ボタンのクリックイベントにSubjectのOnNextを登録
        button.onClick.AddListener(() => _onClickSubject.OnNext(Unit.Default));
      }
    }

  }
}