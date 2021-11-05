using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace PotatoSDK
{
    public class SymbolControlledModuleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical("box");
            string symbol = OnCoreModuleGUI(); 
            GUILayout.EndVertical();
            SDKEditorUtilityMan.ManageScriptingDefineSymbolAndCheckIfEnabled(symbol);
        }
        public virtual string OnCoreModuleGUI()
        {
            base.OnInspectorGUI();
            return "";
        }
    }

    public static class SDKEditorUtilityMan
    {
        [MenuItem("PotatoSDK/Find Config Prefab",priority =1)]
        public static void SelectThePotato()
        {
            Object target = Resources.Load("PotatoSDK");// AssetDatabase.LoadMainAssetAtPath("PotatoSDK/Resources/PotatoSDK.prefab");
            Debug.Log(target.name);
            Selection.activeObject = target;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
        [MenuItem("PotatoSDK/Open Config Prefab", priority = 2)]
        public static void OpenThePotato()
        {
            Object target = Resources.Load("PotatoSDK");// AssetDatabase.LoadMainAssetAtPath("PotatoSDK/Resources/PotatoSDK.prefab");
            Debug.Log(target.name);
            Selection.activeObject = target;
            EditorGUIUtility.PingObject(Selection.activeObject);
            AssetDatabase.OpenAsset(Selection.activeInstanceID);
        }
        //[MenuItem("PotatoSDK/Select/Facebook Wrapper", priority = 10)]
        //public static void SelectFB()
        //{
        //    Object target = Resources.Load("PotatoSDK");// AssetDatabase.LoadMainAssetAtPath("PotatoSDK/Resources/PotatoSDK.prefab");
        //    Debug.Log(target.name);
        //    Selection.activeObject = target;
        //    AssetDatabase.OpenAsset(Selection.activeInstanceID);
        //    Selection.activeObject = Object.FindObjectOfType<FaceBookMan>();
        //    EditorGUIUtility.PingObject(Selection.activeObject);

        //}



        #region Symbol Management
        public class SymbolDefinitionCache
        {
            
            public BuildTargetGroup buildTargetGroup;
            public List<string> currentSymList = new List<string>();
            
        }
        public static SymbolDefinitionCache symbolCache;

        static GUIStyle horizontalLine;
        public static bool ManageScriptingDefineSymbolAndCheckIfEnabled(string symbol)
        {
            if (horizontalLine == null)
            {
                horizontalLine = new GUIStyle();
                horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
                horizontalLine.margin = new RectOffset(0, 0, 5, 5);
                horizontalLine.fixedHeight = 1;
            }

            //EditorGUILayout.Space();
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);

            if (symbolCache == null|| buildTargetGroup != symbolCache.buildTargetGroup)
            {
                symbolCache = new SymbolDefinitionCache();
                symbolCache.buildTargetGroup = buildTargetGroup;

                string[] currentSyms;
                PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup, out currentSyms);
                symbolCache.currentSymList.AddRange(currentSyms);
            }

            bool isModuleEnabled = symbolCache.currentSymList.Contains(symbol);
            
            if(isModuleEnabled)GUILayout.Box(GUIContent.none, horizontalLine);

            GUILayout.BeginVertical("box");
            GUILayout.Space(3);
            if (isModuleEnabled)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Wrapper module is active");
                GUILayout.Space(4);
                if (GUILayout.Button("Hibernate", GUILayout.Height(25)))
                {
                    if(EditorUtility.DisplayDialog("Set module to hibernate?","Module hibernation disables the wrapper code and avoid compile errors while the proper SDK unitypackage is not imported yet.", "Confirm", "Not Now"))
                    {
                        Debug.Log("Need to recompile scripts. Give Unity a moment...");
                        symbolCache.currentSymList.Remove(symbol);
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbolCache.currentSymList.ToArray());
                        symbolCache = null;
                    }
;
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Wrapper module is hibernating...");
                GUILayout.Space(4);
                if (GUILayout.Button("Activate", GUILayout.Height(25), GUILayout.MinWidth(75)))
                {
                    if (EditorUtility.DisplayDialog("Activate wrapper module?", "You should activate the wrapper only after importing the relevant SDK unitypackage", "Confirm", "Not Now"))
                    {
                        Debug.Log("Need to recompile scripts. Give Unity a moment...");
                        symbolCache.currentSymList.Add(symbol);
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbolCache.currentSymList.ToArray());
                        symbolCache = null;
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            return isModuleEnabled;
        }
        #endregion
        #region Dirty Object Management
        public static void SetObjectDirty(Object o)
        {
            if (Application.isPlaying)
                return;

            if (o is GameObject)
            {
                SetObjectDirty((GameObject)o);
            }
            else if (o is Component)
            {
                SetObjectDirty((Component)o);
            }
            else
            {
                EditorUtility.SetDirty(o);
            }
        }

        public static void SetObjectDirty(GameObject go)
        {
            if (Application.isPlaying)
                return;

            HandlePrefabInstance(go);
            EditorUtility.SetDirty(go);

            //This stopped happening in EditorUtility.SetDirty after multi-scene editing was introduced.
            EditorSceneManager.MarkSceneDirty(go.scene);
        }

        public static void SetObjectDirty(Component comp)
        {
            if (Application.isPlaying)
                return;

            HandlePrefabInstance(comp.gameObject);
            EditorUtility.SetDirty(comp);

            //This stopped happening in EditorUtility.SetDirty after multi-scene editing was introduced.
            EditorSceneManager.MarkSceneDirty(comp.gameObject.scene);
        }

        // Some prefab overrides are not handled by Undo.RecordObject or EditorUtility.SetDirty.
        // eg. adding an item to an array/list on a prefab instance updates that the instance
        // has a different array count than the prefab, but not any data about the added thing
        private static void HandlePrefabInstance(GameObject gameObject)
        {
            var prefabType = PrefabUtility.GetPrefabType(gameObject);
            if (prefabType == PrefabType.PrefabInstance)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
            }
        }
        #endregion
    }



}

