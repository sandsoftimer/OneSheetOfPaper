#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class APToolsMenuItems
{
    [MenuItem("APTools/Delete Gameplay Data")]
    public static void DeleteBinaryData()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("APTools/Generate Conflict")]
    public static void GenerateConflict()
    {
        string path = "ConflictForGreaterGood.txt";
        string data = string.Format("GitConflictGeneration: {0}\n{1} - Device Name: {2}\n{3}\n{4}", System.DateTime.Now, System.DateTime.Now.Ticks, SystemInfo.deviceName, SystemInfo.deviceModel, SystemInfo.deviceUniqueIdentifier);
        StreamWriter sw = File.CreateText(path);
        sw.WriteLine(data);
        sw.Close();
        Debug.LogErrorFormat(data);
    }

    public static void SetupAPProjectSettings()
    {
        PlayerSettings.companyName = "Alpha Potato";
        PlayerSettings.productName = Application.productName;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.alphapotato.projectname");
        PlayerSettings.SetArchitecture(BuildTargetGroup.Android, (int)AndroidArchitecture.ARM64);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
    }

    [MenuItem("APTools/DataHolders/APProjectSettings")]
    public static void CreateProjectSettings()
    {
        //string path = System.IO.Directory.GetParent(AssetDatabase.GetAssetPath(ms.GetInstanceID())) + "/" + ms.name + ".asset";
        CreateScriptableObject(ConstantManager.APPROJECT_SETTING_DESTINATION_PATH, typeof(APProjectSetting));
    }

    [MenuItem("APTools/DataHolders/Level Prefab Data")]
    public static void CreateLevelPrefabDataContainer()
    {
        CreateScriptableObject(ConstantManager.LEVEL_PREFAB_DESTINATION_PATH, typeof(LevelPrefabData));
    }

    public static void CreateScriptableObject(string path, Type obj)
    {
        ScriptableObject so = ScriptableObject.CreateInstance(obj);
        Selection.activeObject = CreateIfDoesntExists(path, so);
    }

    public static Object CreateIfDoesntExists(string path, Object o)
    {
        var ap = AssetDatabase.LoadAssetAtPath(path, o.GetType());
        if (ap == null)
        {
            AssetDatabase.CreateAsset(o, path);
            ap = AssetDatabase.LoadAssetAtPath(path, o.GetType());
            AssetDatabase.Refresh();
            return ap;
        }
        return ap;
    }


}
#endif
