#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
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
        Type t = Type.GetType(Selection.activeObject.name);
        FieldInfo[] fieldInfos = t.GetFields();

        APConfigarator.CreateDirectoryToProject("Scripts/Editor");
        string path = Application.dataPath + "/Scripts/Editor/" + selectedScriptName + "Editor.cs";

        if (!File.Exists(path))
        {
            TextAsset asset = Resources.Load("NewAPEditorScript.cs") as TextAsset;
            string scriptSekeleton = asset.text.Replace("#SCRIPTNAME#", selectedScriptName);

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                scriptSekeleton = scriptSekeleton.Replace(
                    "#endregion ALL_PUBLIC_PROPERTIES",
                    "private SerializedProperty " + fieldInfo.Name + ";" +
                    "\n\t#endregion ALL_PUBLIC_PROPERTIES");

                scriptSekeleton = scriptSekeleton.Replace(
                    "#endregion FINDER_ALL_PUBLIC_PROPERTIES",
                    fieldInfo.Name + " = serializedObject.FindProperty(\"" + fieldInfo.Name + "\");" +
                    "\n\t\t#endregion FINDER_ALL_PUBLIC_PROPERTIES");

                scriptSekeleton = scriptSekeleton.Replace(
                    "#endregion DrawProperty(propertyName)",
                    "DrawProperty(" + fieldInfo.Name + ");" +
                    "\n\t\t\t#endregion DrawProperty(propertyName)");
            }

            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path);
            writer.WriteLine(scriptSekeleton);
            writer.Close();
            AssetDatabase.Refresh();

            Debug.Log("File has been created. PATH = " + path);
        }
        else
        {
            Debug.Log("File Exist. PATH = " + path);
            return;

            path = "Assets/Scripts/Editor/" + selectedScriptName + "Editor.cs";
            var ap = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            string scriptSekeleton = asset.text;

            string regex = @"#region ALL_PUBLIC_PROPERTIES.*?\endregion ALL_PUBLIC_PROPERTIES";
            string output = Regex.Replace(scriptSekeleton, @"!(.*)?\(", string.Empty);

            Debug.LogError(output);

            //foreach (FieldInfo fieldInfo in fieldInfos)
            //{
            //    scriptSekeleton = scriptSekeleton.Replace(
            //        "#endregion ALL_PUBLIC_PROPERTIES",
            //        "private SerializedProperty " + fieldInfo.Name + ";" +
            //        "\n\t#endregion ALL_PUBLIC_PROPERTIES");

            //    scriptSekeleton = scriptSekeleton.Replace(
            //        "#endregion FINDER_ALL_PUBLIC_PROPERTIES",
            //        fieldInfo.Name + " = serializedObject.FindProperty(\"" + fieldInfo.Name + "\");" +
            //        "\n\t\t#endregion FINDER_ALL_PUBLIC_PROPERTIES");

            //    scriptSekeleton = scriptSekeleton.Replace(
            //        "#endregion DrawProperty(propertyName)",
            //        "DrawProperty(" + fieldInfo.Name + ");" +
            //        "\n\t\t\t#endregion DrawProperty(propertyName)");
            //}

            ////Write some text to the test.txt file
            //StreamWriter writer = new StreamWriter(path);
            //writer.WriteLine(scriptSekeleton);
            //writer.Close();
            //AssetDatabase.Refresh();

            //Debug.Log("File has been created. PATH = " + path);
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
}
#endif