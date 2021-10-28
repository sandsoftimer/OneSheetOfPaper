#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public static class APEditorScriptMaker
{
    private static string selectedScriptPath = "";
    private static string selectedScriptName = "";

    [MenuItem("Assets/APTools/APEditorScript")]
    private static void APEditorScript()
    {
        APConfigarator.CreateDirectoryToProject("Scripts/Editor");

        string path = Application.dataPath + "/Scripts/Editor/" + selectedScriptName + "Editor.cs";

        if (!File.Exists(path))
        {
            TextAsset asset = Resources.Load("NewAPEditorScript.cs") as TextAsset;
            string scriptSekeleton = asset.text.Replace("#SCRIPTNAME#", selectedScriptName);
            
            //string[] allPublicProperties = ClassPropertyReader.GetAllPublicPropertyNames(System.Type.GetType(selectedScriptName));

            //Debug.LogError("Property length:" + allPublicProperties.Length);
            //for (int i = 0; i < allPublicProperties.Length; i++)
            //{
            //    Debug.LogError(allPublicProperties[i]);
            //    scriptSekeleton = scriptSekeleton.Replace("#ALL_PUBLIC_PROPERTIES#", allPublicProperties[i] + "/n#ALL_PUBLIC_PROPERTIES#");
            //}
            scriptSekeleton = scriptSekeleton.Replace("#ALL_PUBLIC_PROPERTIES#", "");


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
        selectedScriptPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (selectedScriptPath.Length >= 3) {
            string extension = "";
            for (int i = selectedScriptPath.Length - 3; i < selectedScriptPath.Length; i++)
            {
                extension += selectedScriptPath[i];
            }
            return extension.Equals(".cs");
        }
        return false;
    }

}
#endif