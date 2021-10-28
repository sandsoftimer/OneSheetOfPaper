﻿using UnityEditor;
using UnityEngine;

class APAutoSupportClass
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        GameObject.Instantiate(Resources.Load("APTools") as GameObject).name = "APTools";
    }

}

#if UNITY_EDITOR
[InitializeOnLoad]
public class HierarchyIcons
{
    static HierarchyIcons() { EditorApplication.hierarchyWindowItemOnGUI += EvaluateIcons; }

    private static void EvaluateIcons(int instanceId, Rect selectionRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
        if (go == null) return;

        IHierarchyIcon slotCon = go.GetComponent<IHierarchyIcon>();
        if (slotCon != null) DrawIcon(slotCon.EditorIconPath, selectionRect);
    }

    private static void DrawIcon(string texName, Rect rect)
    {
        Rect r = new Rect(rect.x + rect.width - 20f, rect.y, 24f, 14f);
        GUI.DrawTexture(r, GetTex(texName));
    }

    private static Texture2D GetTex(string name)
    {
        return (Texture2D)Resources.Load(name);
    }
}
#endif

public interface IHierarchyIcon
{
    string EditorIconPath { get; }
}