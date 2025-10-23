using System;
using System.Collections.Generic;
using System.IO;
using JJJ.Utils;
using UnityEngine;
using ZLogger;

namespace JJJ.Infrastructure.Editor
{
  /// <summary>
  /// .envファイルから環境変数を読み込むユーティリティクラス
  /// </summary>
  public class EnvLoader
  {
    // キャッシュされた環境変数
    private static Dictionary<string, string> _envVariables = new Dictionary<string, string>();

    private static readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<EnvLoader>();

    /// <summary>
    /// 指定したキーの環境変数の値を取得
    /// </summary>
    /// <param name="key">環境変数のキー</param>
    /// <param name="defaultValue">キーが存在しない場合のデフォルト値</param>
    /// <returns>環境変数の値、またはデフォルト値</returns>
    public static string GetValue(string key, string defaultValue = "")
    {
      // 環境変数が未ロードの場合はロードする
      if (_envVariables == null || _envVariables.Count == 0)
      {
        LoadEnvFile();
      }

      // 指定したキーの値を返す
      return _envVariables != null && _envVariables.ContainsKey(key)
          ? _envVariables[key]
          : defaultValue;
    }

    /// <summary>
    /// .envファイルを読み込み、環境変数をキャッシュする
    /// </summary>
    private static void LoadEnvFile()
    {
      _envVariables = new Dictionary<string, string>();

      // .envファイルのパスを取得
      string envPath = Path.Combine(Application.dataPath, "../.env");

      // .envファイルが存在しない場合は警告を出して終了
      if (!File.Exists(envPath))
      {
        _logger.ZLogWarning($".envファイルが見つかりません: {envPath}");
        return;
      }

      try
      {
        // .envファイルを読み込み、キーと値を解析してキャッシュに保存
        string[] lines = File.ReadAllLines(envPath);
        foreach (string line in lines)
        {
          // コメント行と空行をスキップ
          if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            continue;

          // キーと値を分割
          int separatorIndex = line.IndexOf('=');
          if (separatorIndex > 0)
          {
            // キーと値をトリムして保存
            string key = line.Substring(0, separatorIndex).Trim();
            string value = line.Substring(separatorIndex + 1).Trim();
            _envVariables[key] = value;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.ZLogError($".envファイルの読み込みに失敗しました: {ex.Message}");
      }
    }
  }
}