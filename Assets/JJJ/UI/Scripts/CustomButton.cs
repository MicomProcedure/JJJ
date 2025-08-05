using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

namespace JJJ.UI
{
  [AddComponentMenu("JJJ/UI/CustomButton", 30)]
  public class CustomButton : Button
  {
    private Tweener _tweener = null;
    private Vector3 _originalScale;

    protected override void Awake()
    {
      base.Awake();
      _originalScale = transform.localScale;
    }

    private new void Start()
    {
      base.Start();

      onClick.AddListener(() =>
      {
        if (_tweener != null && _tweener.IsActive() && _tweener.IsPlaying())
        {
          _tweener.Kill();
          _tweener = null;
          transform.localScale = _originalScale;
        }

        _tweener = transform.DOPunchScale(
          punch: Vector3.one * 0.1f,
          duration: 0.2f,
          vibrato: 1
        ).SetEase(Ease.OutExpo);
      });
    }
  }
}