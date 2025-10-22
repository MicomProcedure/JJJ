using JJJ.Core.Interfaces;
using UnityEngine;

namespace JJJ.View
{
  public class HelpsView : MonoBehaviour, IHelpsView
  {

    public void Show()
    {
      gameObject.SetActive(true);
    }

    public void Hide()
    {
      gameObject.SetActive(false);
    }
  }
}