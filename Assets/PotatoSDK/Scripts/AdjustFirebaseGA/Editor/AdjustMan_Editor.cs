using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PotatoSDK
{
    [CustomEditor(typeof(AdjustMan))]
    public class AdjustMan_Editor : SymbolControlledModuleEditor
    {
        public override string OnCoreModuleGUI()
        {
#if POTATO_ADJUST
            base.OnCoreModuleGUI();

            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
            GUILayout.BeginHorizontal();
            GUILayout.Label(AdjustMan.isTestMode ? "*Test Mode Active*" : "Production Mode Active");
            if (AdjustMan.isTestMode)
            {
                if (GUILayout.Button("Test=>Production"))
                {
                    string oldbundleid = EditorPrefs.GetString("STORED_BUNDLE_ID", "");
                    string body = string.Format("Potato SDK will attempt to restore your bundleId to {0} However, you should manually refresh your Player setting window to check", oldbundleid);
                    if (EditorUtility.DisplayDialog("Returning to production mode", body, "Proceed", "Cancel"))
                    {
                        Debug.LogFormat("TestMode Terminated. Restoring bundle id to {0}", oldbundleid);
                        Debug.Log("You should consider delete and resolving your android libraries after setting Package Name");
                        PlayerSettings.SetApplicationIdentifier(buildTargetGroup, oldbundleid);
                        //SetProjDirty();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        EditorApplication.ExecuteMenuItem("File/Save Project");
                    }

                }
            }
            else
            {
                if (GUILayout.Button("Production=>Test", GUILayout.Height(25), GUILayout.MinWidth(75)))
                {
                    if (EditorUtility.DisplayDialog("Enabling Test Mode", "Potato SDK will change your bundle ID to com.portbliss.icycake for testing", "Proceed", "Cancel"))
                    {
                        EditorPrefs.SetString("STORED_BUNDLE_ID", PlayerSettings.applicationIdentifier);
                        PlayerSettings.SetApplicationIdentifier(buildTargetGroup, "com.portbliss.icycake");
                        //SetProjDirty();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        EditorApplication.ExecuteMenuItem("File/Save Project");
                    }

                }
            }
            GUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();

#endif

            return "POTATO_ADJUST";
        }
        //void SetProjDirty()
        //{
        //    UnityEditor.CloudProjectSettings
        //    Object obj =  Unsupported.GetSerializedAssetInterfaceSingleton("ProjectSettings");
        //    Debug.Log(obj);
        //    PlayerSettings ps = obj as PlayerSettings;
        //    //PlayerSettings.i
            
        //    Debug.Log(ps);
        //    //SDKEditorUtilityMan.SetObjectDirty(obj);
        //}

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();

//#if POTATO_ADJUST
//            AdjustMan adjman = target as AdjustMan;// Selection.activeContext as AdjustMan;

//            if (adjman)
//            {
//                GUILayout.Label(adjman.isTestMode ? "*Test Mode Active*" : "Production Mode Active");
//                if (adjman.isTestMode)
//                {
//                    if (GUILayout.Button("Test=>Production"))
//                    {
//                        string oldbundleid = PlayerPrefs.GetString("STORED_BUNDLE_ID", "");
//                        Debug.LogError("TestMode Terminated. Please Check Android Package Name manually");
//                        Debug.LogError("You should consider delete and resolving your android libraries after setting Package Name");
//                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, oldbundleid);
//                        adjman.SwitchToProduction();
//                        SDKEditorUtilityMan.SetObjectDirty(adjman);
//                        AssetDatabase.SaveAssets();
//                        AssetDatabase.Refresh();
//                        EditorApplication.ExecuteMenuItem("File/Save Project");
//                    }
//                }
//                else
//                {
//                    if (GUILayout.Button("Production=>Test"))
//                    {
//                        PlayerPrefs.SetString("STORED_BUNDLE_ID", PlayerSettings.applicationIdentifier);
//                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.portbliss.icycake");
//                        adjman.SwitchToTest();
//                        SDKEditorUtilityMan.SetObjectDirty(adjman);
//                        AssetDatabase.SaveAssets();
//                        AssetDatabase.Refresh();
//                        EditorApplication.ExecuteMenuItem("File/Save Project");
//                    }
//                }
//            }
//            serializedObject.ApplyModifiedProperties();

//#endif

//            SDKEditorUtilityMan.ManageScriptingDefineSymbolAndCheckIfEnabled("POTATO_ADJUST");
//        }

    }
}

