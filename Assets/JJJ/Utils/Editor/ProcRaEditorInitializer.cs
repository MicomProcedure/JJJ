#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ProcRanking;
using UnityEditor.SceneManagement;
using JJJ.Utils;
using ZLogger;

namespace JJJ.Infrastructure.Editor
{
  /// <summary>
  /// エディタのPlayMode開始時にProcRaのアクセストークンを環境変数から設定し、PlayMode終了時に元のトークンに戻すための初期化クラス
  /// </summary>
  [InitializeOnLoad]
  public class ProcRaEditorInitializer
  {
    // 環境変数のキー
    private const string EnvKey = "PROCRA_ACCESS_TOKEN";
    // インスタンス化フラグのキー
    private const string InstantiatedFlagKey = "JJJ.ProcRaEditorInitializer.Instantiated";
    // Prefabのパス
    private const string PrefabAssetPath = "Packages/com.micomprocedure.procranking.unity/Prefabs/ProcRaSettings.prefab";
    // Dirty状態保存用キー
    private const string SceneWasDirtyKey = "JJJ.ProcRaEditorInitializer.SceneWasDirty";

    private static readonly Microsoft.Extensions.Logging.ILogger _logger = LogManager.CreateLogger<ProcRaEditorInitializer>();

    /// <summary>
    /// 静的コンストラクタでPlayModeの状態変化イベントに登録
    /// </summary>
    static ProcRaEditorInitializer()
    {
      EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    /// <summary>
    /// PlayModeの状態変化イベントハンドラ
    /// </summary>
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
      if (state == PlayModeStateChange.ExitingEditMode)
      {
        // PlayMode開始前に環境変数からアクセストークンを設定
        SetAccessTokenFromEnv();
      }
      else if (state == PlayModeStateChange.EnteredEditMode)
      {
        // PlayMode終了後に元のトークンに戻す
        EditorApplication.delayCall += RestoreOriginalToken;
      }
    }

    /// <summary>
    /// 環境変数からアクセストークンを取得してProcRaSettingsに設定
    /// </summary>
    private static void SetAccessTokenFromEnv()
    {
      // アクセストークンを.envファイルから取得
      string accessToken = EnvLoader.GetValue(EnvKey, "");

      // トークンが空の場合は警告を出して終了
      if (string.IsNullOrEmpty(accessToken))
      {
        _logger.ZLogWarning($".envファイルから'{EnvKey}'が見つかりませんでした。");
        return;
      }

      // PlayMode開始前に現在のシーンのDirty状態を保存
      var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
      SessionState.SetInt(SceneWasDirtyKey, activeScene.isDirty ? 1 : 0);

      // PlayMode開始前にオブジェクトを検索
      var targetSettings = Object.FindFirstObjectByType<ProcRaSettings>();

      if (targetSettings == null)
      {
        // シーンに無ければプレハブから生成を試みる
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabAssetPath);
        // Prefabが見つからなかった場合は警告を出して終了
        if (prefab == null)
        {
          _logger.ZLogWarning($"シーンにProcRaSettingsが見つからず、Prefabが'{PrefabAssetPath}'に存在しません。");
          return;
        }

        // Prefabからインスタンスを生成
        var instanceObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        // Prefabのインスタンス生成に失敗した場合はエラーを出して終了
        if (instanceObj == null)
        {
          _logger.ZLogError($"ProcRaSettings prefabのインスタンス生成に失敗しました。");
          return;
        }

        // ProcRaSettingsコンポーネントを取得
        targetSettings = instanceObj.GetComponent<ProcRaSettings>();
        // 生成したPrefabにProcRaSettingsコンポーネントが見つからなかった場合はエラーを出して終了
        if (targetSettings == null)
        {
          _logger.ZLogError($"ProcRaSettings コンポーネントがインスタンス化したPrefabに見つかりません。インスタンスを削除します。");
          Object.DestroyImmediate(instanceObj);
          return;
        }

        // 後で削除するためのフラグを保存
        SessionState.SetInt(InstantiatedFlagKey, 1);
        _logger.ZLogInformation($"ProcRaSettings prefabをランタイムでインスタンス化してトークンを注入しました。");
      }

      // アクセストークンを設定
      var so = new SerializedObject(targetSettings);
      var prop = so.FindProperty("accessToken");

      // プロパティが見つからなかった場合はエラーを出して終了
      if (prop == null)
      {
        _logger.ZLogError($"ProcRaSettingsに'accessToken'プロパティが見つかりませんでした。");
        return;
      }

      // トークンを設定
      prop.stringValue = accessToken;
      so.ApplyModifiedProperties();
      // EditorUtility.SetDirtyは不要（変更適用でDirtyになるため）

      _logger.ZLogInformation($".envファイルからアクセストークンを設定しました。");
    }

    /// <summary>
    /// PlayMode終了後に元のトークンに戻す
    /// </summary>
    private static void RestoreOriginalToken()
    {
      // PlayMode終了後に再度オブジェクトを検索
      var targetSettings = Object.FindFirstObjectByType<ProcRaSettings>();

      // オブジェクトが見つからなかった場合の処理
      if (targetSettings == null)
      {
        _logger.ZLogWarning($"ProcRaSettingsが見つかりません。トークンをクリアできません。");
        CleanUp();
        return;
      }

      // アクセストークンをクリア
      var so = new SerializedObject(targetSettings);
      var prop = so.FindProperty("accessToken");

      // プロパティが見つからなかった場合はエラーを出して終了
      if (prop == null)
      {
        _logger.ZLogError($"'accessToken'プロパティがProcRaSettingsに見つかりませんでした。");
        CleanUp();
        return;
      }

      // トークンをクリア
      prop.stringValue = "";
      so.ApplyModifiedProperties();
      EditorUtility.SetDirty(targetSettings);

      _logger.ZLogInformation($"PlayMode終了後にトークンをクリアしました。");

      // 生成したPrefabインスタンスがあれば削除
      if (SessionState.GetInt(InstantiatedFlagKey, 0) == 1)
      {
        try
        {
          // 生成したPrefabインスタンスを削除
          // 生成したPrefabインスタンスを削除
          Object.DestroyImmediate(targetSettings.gameObject);
          _logger.ZLogInformation($"生成したProcRaSettings prefabを削除しました。");
        }
        catch (System.Exception ex)
        {
          // 生成したPrefabインスタンスの削除に失敗した場合の処理
          _logger.ZLogWarning($"ProcRaSettingsのインスタンス削除に失敗しました: {ex.Message}");
        }
      }

      // シーンのDirty状態を復元
      var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
      bool wasDirty = SessionState.GetInt(SceneWasDirtyKey, 0) == 1;

      if (wasDirty)
      {
        // もともとDirtyだった場合、Dirtyを維持（保存しない）
        EditorSceneManager.MarkSceneDirty(scene);
        _logger.ZLogInformation($"元のDirty状態(true)を復元しました（保存なし）。");
      }
      else
      {
        // もともとCleanだった場合、変更を破棄してCleanへ戻す
        if (!string.IsNullOrEmpty(scene.path))
        {
          EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
          _logger.ZLogInformation($"元のDirty状態(false)を復元するため、シーンをリロードしました。");
        }
        else
        {
          // 保存されていないシーンはリロード不可
          _logger.ZLogWarning($"未保存シーンのためDirty状態をリロードで復元できません。手動で調整してください。");
        }
      }

      CleanUp();
    }

    private static void CleanUp()
    {
      SessionState.EraseInt(InstantiatedFlagKey);
      SessionState.EraseInt(SceneWasDirtyKey);
    }
  }
}
#endif