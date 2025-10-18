using R3;
using UnityEngine;
using UnityEngine.UI;

namespace JJJ.UI
{
  public class ResultButtonObservables : MonoBehaviour
  {
    [SerializeField] private Button _backgroundButton = null!;

    public Observable<Unit> BackgroundButtonOnClick => _backgroundButton.OnClickAsObservable();
  }
}