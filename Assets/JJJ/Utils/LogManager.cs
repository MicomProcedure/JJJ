using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger.Unity;

namespace JJJ.Utils
{
  /// <summary>
  /// ログ管理クラス
  /// </summary>
  public static class LogManager
  {
    private static ILoggerFactory _loggerFactory;

    public static ILogger<T> CreateLogger<T>() => _loggerFactory.CreateLogger<T>();
    public static readonly Microsoft.Extensions.Logging.ILogger Global;

    static LogManager()
    {
      _loggerFactory = LoggerFactory.Create(logging =>
      {
        logging.SetMinimumLevel(LogLevel.Trace);
        logging.AddZLoggerUnityDebug();
      });
      Global = _loggerFactory.CreateLogger("Logger");

      Application.exitCancellationToken.Register(() =>
      {
        _loggerFactory?.Dispose();
      });
    }
  }
}