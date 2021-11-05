using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace PotatoSDK
{
    [CustomEditor(typeof(Potato))]
    public class Potato_Editor : Editor
    {
        const string LOADING_SCREEN_PATH = "Assets/PotatoSDK/Resources/SplashScreen.unity";
        public override void OnInspectorGUI()
        {
            Potato p = target as Potato;
            //base.OnInspectorGUI();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("disablePotatoLogs"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("skipUsingSplashScene"));
            if (!p.skipUsingSplashScene)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skipAutoLoadNextScene"));
                if (!p.skipAutoLoadNextScene)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("autoLoadSceneIndex"));
                    if (p.autoLoadSceneIndex<=0)
                    {
                        EditorGUILayout.HelpBox("Setting values less than 1 will result in loading scene on build index 1 instead!", MessageType.Warning);
                    }
                }

                EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;

                if (original.Length < 1 || !string.Equals(original[0].path, LOADING_SCREEN_PATH))
                {
                    if (GUILayout.Button("Add Splash Screen"))
                    {
                        AddLoadingScreenNow(original);
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();



        }
        private void OnEnable()
        {
            Prompt();
        }
        private void OnDisable()
        {
            Prompt();
        }
        void Prompt()
        {
            EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;

            Potato p = target as Potato;
            if (p.skipUsingSplashScene)
            {
                if (original.Length >= 1 && string.Equals(original[0].path, LOADING_SCREEN_PATH))
                {
                    if (EditorUtility.DisplayDialog("Should Potato SDK remove it's Splash Scene!", "You have checked that you want to skip Potato Splash Screen, but it is still added. Should we remove it? ", "Remove", "Keep"))
                    {
                        RemoveLoadingScreenNow(original);
                    }
                }
            }
            else
            {
                if (original.Length < 1 || !string.Equals(original[0].path, LOADING_SCREEN_PATH))
                {
                    if (EditorUtility.DisplayDialog("PotatoSDK wants to add it's Splash Scene!", "Potato SDK wants to add an initial Splash Scene to ensure safe initialization. Its recommended that you allow this!", "Allow", "Ignore"))
                    {
                        AddLoadingScreenNow(original);
                    }

                }
            }

        }
        void RemoveLoadingScreenNow(EditorBuildSettingsScene[] original)
        {
            var newSettings = new EditorBuildSettingsScene[original.Length - 1];

            for (int i = 0; i < newSettings.Length; i++)
            {

                newSettings[i] = original[i + 1];

            }
            EditorBuildSettings.scenes = newSettings;
        }
        void AddLoadingScreenNow(EditorBuildSettingsScene[] original)
        {
            var newSettings = new EditorBuildSettingsScene[original.Length + 1];

            for (int i = 0; i < newSettings.Length; i++)
            {
                if (i == 0)
                {
                    newSettings[0] = new EditorBuildSettingsScene(LOADING_SCREEN_PATH, true);
                }
                else
                {
                    newSettings[i] = original[i-1];
                }

            }
            EditorBuildSettings.scenes = newSettings;
        }
    }


}