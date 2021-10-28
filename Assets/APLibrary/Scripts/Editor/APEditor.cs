using System;
using UnityEditor;
using UnityEngine;

public class APEditor : Editor
{
    protected void DrawProperty(SerializedProperty propertyName)
    {
        EditorGUILayout.PropertyField(propertyName);
    }

    protected GUIStyle GetButtonStyle(EditorButtonStyle editorButtonStyle)
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        if (editorButtonStyle != null)
        {
            GUI.backgroundColor = editorButtonStyle.buttonColor;
            style.normal.textColor = editorButtonStyle.buttonTextColor;
        }
        return style;
    }

    protected void OnButtonPressed(string buttonText, Action action, EditorButtonStyle editorButtonStyle = null)
    {
        if (GUILayout.Button(buttonText, GetButtonStyle(editorButtonStyle)))
        {
            action.Invoke();
        }
    }

    protected void DrawHorizontalLine()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    protected void DrawHorizontalLineOnGUI(Rect rect)
    {
        EditorGUI.LabelField(rect, "", GUI.skin.horizontalSlider);
    }

    protected void Space(float spaceSize = 1, bool expand = false)
    {
        EditorGUILayout.Space(spaceSize, expand);
    }

}

