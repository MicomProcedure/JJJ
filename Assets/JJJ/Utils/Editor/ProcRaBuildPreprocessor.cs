#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using ProcRanking;
using UnityEngine;
using JJJ.Utils;
using ZLogger;
using UnityEditor.SceneManagement;

namespace JJJ.Infrastructure.Editor
{
  /// <summary>
  /// ビルド前にProcRaのアクセストークンを環境変数から設定し、ビルド後に元のトークンに戻すためのビルドプリプロセッサ
  /// </summary>
  public class ProcRaBuildPreprocessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
  {
    public int callbackOrder => 0;

    // 環境変数のキー
    private static string _envKey = "PROCRA_ACCESS_TOKEN";
    private static ProcRaSettings _targetSettings = null!;
    private static string _scenePath = "";
    private static bool _instantiatedFromPrefab = false;
    private const string PrefabAssetPath = "Packages/com.micomprocedure.procranking.unity/Prefabs/ProcRaSettings.prefab";

    private readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<ProcRaBuildPreprocessor>();

    // ビルド前処理
    public void OnPreprocessBuild(BuildReport report)
    {
      // 現在のシーンパスを保存
      var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
      _scenePath = scene.path;

      if (scene.isDirty)
      {
        throw new BuildFailedException("シーンが保存されていません。ビルド前にシーンを保存してください。");
      }

      // ProcRaSettingsをシーンから探す
      _targetSettings = Object.FindFirstObjectByType<ProcRaSettings>();
      // シーンに存在しなければPrefabから生成を試みる
      if (_targetSettings == null)
      {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabAssetPath);
        // Prefabが見つからなかった場合は警告を出して終了
        if (prefab == null)
        {
          _logger.ZLogWarning($"ProcRaSettingsがシーンに存在せず、Prefab '{PrefabAssetPath}' も見つかりませんでした。");
          return;
        }

        // Prefabからインスタンスを生成
        var instanceObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (instanceObj == null)
        {
          // Prefabのインスタンス生成に失敗した場合
          _logger.ZLogError($"ProcRaSettingsのPrefab生成に失敗しました。");
          return;
        }

        // ProcRaSettingsコンポーネントを取得
        _targetSettings = instanceObj.GetComponent<ProcRaSettings>();
        if (_targetSettings == null)
        {
          // 生成したPrefabにProcRaSettingsコンポーネントが見つからなかった場合
          _logger.ZLogError($"生成したPrefabにProcRaSettingsコンポーネントが見つかりません。インスタンスを削除します。");
          Object.DestroyImmediate(instanceObj);
          return;
        }

        // フラグを立てる
        _instantiatedFromPrefab = true;
        _logger.ZLogInformation($"ProcRaSettings Prefab をビルド前にインスタンス化しました。");
      }

      // 環境変数からアクセストークンを取得して設定
      string accessToken = EnvLoader.GetValue(_envKey, "");
      if (string.IsNullOrEmpty(accessToken))
      {
        // 環境変数が見つからなかった場合は警告を出して終了
        _logger.ZLogWarning($"環境変数'{_envKey}'が設定されていません。ProcRaのアクセストークンをビルド前に設定できません。");
        return;
      }

      // アクセストークンを設定
      var so = new SerializedObject(_targetSettings);
      var prop = so.FindProperty("accessToken");

      // プロパティが見つからなかった場合はエラーを出して終了
      if (prop == null)
      {
        _logger.ZLogError($"ProcRaSettingsの'accessToken'プロパティが見つかりません。");
        return;
      }

      // アクセストークンを設定
      prop.stringValue = accessToken;
      // 変更を適用
      so.ApplyModifiedProperties();
      EditorUtility.SetDirty(_targetSettings);
      EditorSceneManager.SaveOpenScenes();

      _logger.ZLogInformation($"ProcRaのアクセストークンをビルド前に環境変数から設定しました。");
    }

    // ビルド後処理
    public void OnPostprocessBuild(BuildReport report)
    {
      EditorApplication.delayCall += RestoreToken;
    }

    // ビルド後にアクセストークンを元に戻す
    public void RestoreToken()
    {
      // ProcRaSettingsが未設定の場合、シーンを開いて探す
      if (_targetSettings == null)
      {
        if (!string.IsNullOrEmpty(_scenePath))
        {
          // シーンを開いてProcRaSettingsを探す
          var scene = EditorSceneManager.OpenScene(_scenePath);
          _targetSettings = Object.FindFirstObjectByType<ProcRaSettings>();
        }

        // それでも見つからなければ警告を出して終了
        if (_targetSettings == null)
        {
          _logger.ZLogWarning($"ProcRaSettingsがシーンに存在しません。ビルド後にアクセストークンを元に戻せません。");
          CleanUp();
          return;
        }
      }

      // ProcRaSettingsが見つかったのでアクセストークンをクリア
      var so = new SerializedObject(_targetSettings);
      var prop = so.FindProperty("accessToken");

      // プロパティが見つからなかった場合はエラーを出して終了
      if (prop == null)
      {
        _logger.ZLogError($"ProcRaSettingsの'accessToken'プロパティが見つかりません。");
        CleanUp();
        return;
      }

      // トークンをクリア
      prop.stringValue = "";
      so.ApplyModifiedProperties();
      EditorUtility.SetDirty(_targetSettings);

      _logger.ZLogInformation($"ProcRaのアクセストークンをビルド後に元に戻しました。");

      // Prefabからインスタンス化していた場合は削除
      if (_instantiatedFromPrefab && _targetSettings != null)
      {
        try
        {
          // 生成したオブジェクトを削除
          Object.DestroyImmediate(_targetSettings.gameObject);
          _logger.ZLogInformation($"ビルド用に生成した ProcRaSettings Prefab を削除しました。");
        }
        catch (System.Exception ex)
        {
          // 生成したオブジェクトを削除できなかった場合は警告を出す
          _logger.ZLogWarning($"生成した ProcRaSettings を削除できませんでした: {ex.Message}");
        }
      }

      // シーンを保存する
      EditorSceneManager.SaveOpenScenes();

      CleanUp();
    }

    public void CleanUp()
    {
      _targetSettings = null!;
      _scenePath = "";
      _instantiatedFromPrefab = false;
    }
  }
}

#endif