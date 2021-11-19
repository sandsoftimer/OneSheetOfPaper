#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class APEditorScriptMaker
{
    private static string selectedScriptPath = "";
    private static string selectedScriptName = "";

    [MenuItem("Assets/APTools/APEditorScript")]
    private static void APEditorScript()
    {
        GameObject go = new GameObject();
        Component component = go.AddComponent(System.Type.GetType(Selection.activeObject.name));

        Type t = component.GetType();
        FieldInfo[] fieldInfo = t.GetFields();
        GameObject.DestroyImmediate(go);

        APConfigarator.CreateDirectoryToProject("Scripts/Editor");

        string path = Application.dataPath + "/Scripts/Editor/" + selectedScriptName + "Editor.cs";

        if (!File.Exists(path))
        {
            TextAsset asset = Resources.Load("NewAPEditorScript.cs") as TextAsset;
            string scriptSekeleton = asset.text.Replace("#SCRIPTNAME#", selectedScriptName);

            foreach (FieldInfo info in fieldInfo)
            {
                scriptSekeleton = scriptSekeleton.Replace(
                    "#ALL_PUBLIC_PROPERTIES#",
                    "private SerializedProperty " + info.Name + ";" +
                    "\n\t#ALL_PUBLIC_PROPERTIES#");

                scriptSekeleton = scriptSekeleton.Replace(
                    "#ALL_PUBLIC_PROPERTIES_FINDER#",
                    info.Name + " = serializedObject.FindProperty(\"" + info.Name + "\");" +
                    "\n\t\t#ALL_PUBLIC_PROPERTIES_FINDER#");

                scriptSekeleton = scriptSekeleton.Replace(
                    "#DrawProperty(propertyName)#",
                    "DrawProperty(" + info.Name + ");" +
                    "\n\t\t\t#DrawProperty(propertyName)#");
            }
            scriptSekeleton = scriptSekeleton.Replace("#ALL_PUBLIC_PROPERTIES#", "");
            scriptSekeleton = scriptSekeleton.Replace("#ALL_PUBLIC_PROPERTIES_FINDER#", "");
            scriptSekeleton = scriptSekeleton.Replace("#DrawProperty(propertyName)#", "");


            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path);
            writer.WriteLine(scriptSekeleton);
            writer.Close();
            AssetDatabase.Refresh();

            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            Selection.activeObject = obj;

            Debug.Log("File has been created. PATH = " + path);
        }
        else
        {
            Debug.LogError("File already exist. PATH = " + path);
            EditorUtility.FocusProjectWindow();
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }
    }

    // Note that we pass the same path, and also pass "true" to the second argument for validation.
    [MenuItem("Assets/APTools/APEditorScript", true)]
    private static bool APEditorScriptValidation()
    {
        // This returns true when the selected object is an C# (the menu item will be disabled otherwise).
        selectedScriptName = Selection.activeObject.name;
        return Selection.activeObject is MonoScript;
    }
    [MenuItem("GameObject/MyCategory/Custom Game Object", false, 10)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("Custom Game Object");
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        //GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        Component component = go.AddComponent(System.Type.GetType(Selection.activeObject.name));
        Type t = component.GetType();
        Debug.Log("Type " + t);
        Debug.Log("Type information for:" + t.FullName);
        Debug.Log("\tBase class = " + t.BaseType.FullName);
        Debug.Log("\tIs Class = " + t.IsClass);
        Debug.Log("\tIs Enum = " + t.IsEnum);
        Debug.Log("\tAttributes = " + t.Attributes);
        System.Reflection.FieldInfo[] fieldInfo = t.GetFields();
        foreach (System.Reflection.FieldInfo info in fieldInfo)
            Debug.Log("Field:" + info.Name);
        System.Reflection.PropertyInfo[] propertyInfo = t.GetProperties();
        foreach (System.Reflection.PropertyInfo info in propertyInfo)
            Debug.Log("Prop:" + info.Name);
        Debug.Log("Found component " + component);
        

        // Register the creation in the undo system
        //Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        GameObject.DestroyImmediate(go);
    }
}
#endif