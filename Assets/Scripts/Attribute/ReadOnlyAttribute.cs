using UnityEngine;
using UnityEditor;

/// <summary>
/// Inspector上で編集不可にする属性
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute{ }

#if UNITY_EDITOR
/// <summary>
/// ReadOnly属性を付けた時のInspector上の描画を設定するクラス
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

#endif