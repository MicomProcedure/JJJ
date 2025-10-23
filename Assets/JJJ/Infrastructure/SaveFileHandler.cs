using System.IO;
using System.Text;
using UnityEngine;

namespace JJJ.Infrastructure
{
  public static class SaveFileHandler
  {
    public static void Save<T>(T data)
    {
      string json = JsonUtility.ToJson(data);

      string path = Path.Combine(Application.persistentDataPath, $"{typeof(T).Name}.sav");

      File.WriteAllText(path, json, Encoding.UTF8);
    }

    public static bool TryLoad<T>(out T? data)
    {
      string path = Path.Combine(Application.persistentDataPath, $"{typeof(T).Name}.sav");

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