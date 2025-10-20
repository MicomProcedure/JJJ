using System.IO;
using System.Text;
using UnityEngine;

namespace JJJ.Infrastructure
{
  public static class SaveFileHandler
  {
    public static void Save(UserSettingsProvider data)
    {
      Save(data, "UserSettings");
    }

    private static void Save<T>(T data, string fileName)
    {
      string json = JsonUtility.ToJson(data);

      string path = Path.Combine(Application.persistentDataPath, $"{fileName}.sav");

      File.WriteAllText(path, json, Encoding.UTF8);
    }

    public static bool TryLoad(out UserSettingsProvider? userSettingsProvider)
    {
      userSettingsProvider = null;
      return TryLoad(out userSettingsProvider, "UserSettings");
    }

    private static bool TryLoad<T>(out T? data, string fileName)
    {
      string path = Path.Combine(Application.persistentDataPath, $"{fileName}.sav");

      if (!File.Exists(path))
      {
        data = default;
        return false;
      }

      string json = File.ReadAllText(path, Encoding.UTF8);
      data = JsonUtility.FromJson<T>(json);
      return true;
    }
  }
}