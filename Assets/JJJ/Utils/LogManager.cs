using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UnityEngine;
using ZLogger;
using ZLogger.Unity;

namespace JJJ.Utils
{
  /// <summary>
  /// ログ管理クラス
  /// </summary>
  public static class LogManager
  {
    private static ILoggerFactory _loggerFactory;

    private static readonly Dictionary<string, string> _layerToColor = new Dictionary<string, string>
    {
      { "JJJ.Infrastructure", "#b4e4f8" },
      { "JJJ.UI", "#6ff527" },
      { "JJJ.UseCase", "#470b16" },
      { "JJJ.Utils", "#0075ca" },
      { "JJJ.View", "#a781ba" }
    };
    private static string _defaultColor = "#666666";

    public static ILogger<T> CreateLogger<T>()
    {
      try
      {
        return _loggerFactory.CreateLogger<T>();
      }
      catch (System.ObjectDisposedException)
      {
        return NullLogger<T>.Instance;
      }
    }

    static LogManager()
    {
      _loggerFactory = LoggerFactory.Create(logging =>
      {
        logging.SetMinimumLevel(LogLevel.Debug);
        logging.AddZLoggerUnityDebug(opt =>
        {
          opt.UsePlainTextFormatter(formatter =>
          {
            formatter.SetPrefixFormatter($"[{0}] ", (in MessageTemplate template, in LogInfo info) =>
            {
              string className = info.Category.Name.Split('.').Length > 0 ? info.Category.Name.Split('.')[^1] : info.Category.Name;
              string color = _defaultColor;
              foreach (var kvp in _layerToColor)
              {
                if (info.Category.Name.StartsWith(kvp.Key))
                {
                  color = kvp.Value;
                  break;
                }
              }
              template.Format($"<color={color}>{className}</color>");
            });
          });
        });
      });

      Application.exitCancellationToken.Register(() =>
      {
        _loggerFactory?.Dispose();
      });
    }
  }
}