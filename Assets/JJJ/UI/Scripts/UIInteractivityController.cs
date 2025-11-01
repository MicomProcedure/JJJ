using System.Collections.Generic;
using JJJ.Core.Interfaces.UI;
using JJJ.Utils;
using UnityEngine;
using ZLogger;

namespace JJJ.UI
{
  [DisallowMultipleComponent]
  public class UIInteractivityController : MonoBehaviour,IUIInteractivityController
  {
    [SerializeField] private List<CanvasGroup> _roots = new();

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<UIInteractivityController>();

    public void DisableAllInteractivity()
    {
      if (_roots.Count == 0)
      {
        _logger.ZLogWarning($"UIInteractivityController: No CanvasGroup assigned to _roots.");
        return;
      }
      foreach (var cg in _roots)
      {
        cg.interactable = false;
      }
    }

    public void EnableAllInteractivity()
    {
      if (_roots.Count == 0)
      {
        _logger.ZLogWarning($"UIInteractivityController: No CanvasGroup assigned to _roots.");
        return;
      }
      foreach (var cg in _roots)
      {
        cg.interactable = true;
      }
    }
  }
}