// Add by kamekiti-mars

using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

namespace KanKikuchi.AudioManager
{
  [CustomPropertyDrawer(typeof(SEPathSelectorAttribute))]
  public class SEPathSelectorDrawer : PropertyDrawer
  {
    private static string[] _names;
    private static string[] _values;

    private static void Init()
    {
      if (_names != null) return;

      var fields = typeof(SEPath)
          .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
          .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

      if (!fields.Any())
      {
        _names = new[] { "\n" };
        _values = new[] { string.Empty };
      }
      else
      {
        _names = new[] { "\n" }
            .Concat(fields.Select(f => f.Name))
            .ToArray();

        _values = new[] { string.Empty }
            .Concat(fields.Select(f => f.GetValue(null)?.ToString() ?? string.Empty))
            .ToArray();
      }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      Init();

      int index = Array.IndexOf(_values, property.stringValue);
      if (index < 0) index = 0;

      int newIndex = EditorGUI.Popup(position, label.text, index, _names);

      property.stringValue = _values[newIndex];
    }
  }
}