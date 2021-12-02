#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Com.AlphaPotato.Utility;

[CustomEditor(typeof(APProjectSetting))]
public class APProjectSettingEditor : APEditor
{
    private APProjectSetting scriptReference;

    // All SerializedProperties
    #region ALL_PUBLIC_PROPERTIES
    private SerializedProperty aPProjectDefaultSettings;
	private SerializedProperty aPProjectAndroidSettings;
	private SerializedProperty aPProjectIOSSettings;
	#endregion ALL_PUBLIC_PROPERTIES

    public void OnEnable()
    {
        scriptReference = (APProjectSetting)target;
        #region FINDER_ALL_PUBLIC_PROPERTIES_FINDER
        aPProjectDefaultSettings = serializedObject.FindProperty("aPProjectDefaultSettings");
		aPProjectAndroidSettings = serializedObject.FindProperty("aPProjectAndroidSettings");
		aPProjectIOSSettings = serializedObject.FindProperty("aPProjectIOSSettings");
		#endregion FINDER_ALL_PUBLIC_PROPERTIES
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        #region DrawProperty(propertyName)

        var previousGUIState = GUI.enabled;
        GUI.enabled = false;
        DrawProperty(aPProjectDefaultSettings.FindPropertyRelative("productName"));
        GUI.enabled = previousGUIState;

        DrawProperty(aPProjectDefaultSettings.FindPropertyRelative("companyName"));
        DrawProperty(aPProjectDefaultSettings.FindPropertyRelative("bundleIdentifier"));
        DrawProperty(aPProjectDefaultSettings.FindPropertyRelative("icon"));
        DrawProperty(aPProjectDefaultSettings.FindPropertyRelative("unitySplashScreen"));
        DrawProperty(aPProjectDefaultSettings.FindPropertyRelative("defaultInterfaceOrientation"));

        var previousGUIBackground = GUI.backgroundColor;
        GUI.backgroundColor = Color.black;
        Space();
        DrawHorizontalLine();
        GUI.backgroundColor = previousGUIBackground;

        BuildTargetGroup selectedBuildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
        if (selectedBuildTargetGroup == BuildTargetGroup.Standalone)
        {
            EditorGUILayout.LabelField("Standalone specific things");
        }

        if (selectedBuildTargetGroup == BuildTargetGroup.Android)
        {
            Space(10);
            DrawProperty(aPProjectAndroidSettings.FindPropertyRelative("overrideBundleIdentifier"));

            previousGUIState = GUI.enabled;
            GUI.enabled = aPProjectAndroidSettings.FindPropertyRelative("overrideBundleIdentifier").boolValue;
            DrawProperty(aPProjectAndroidSettings.FindPropertyRelative("bundleIdentifier"));
            GUI.enabled = previousGUIState;

            Space(10);
            EditorGUILayout.BeginVertical(BackgroundStyle(new Color(0.22f, 0.22f, 0.22f), Color.white));
            DrawProperty(aPProjectAndroidSettings.FindPropertyRelative("versionNumber"));
            DrawProperty(aPProjectAndroidSettings.FindPropertyRelative("debugBuildNumber"));
            DrawProperty(aPProjectAndroidSettings.FindPropertyRelative("producitonBuildNumber"));
            EditorGUILayout.EndVertical();

            Space(10);
            DrawProperty(aPProjectAndroidSettings.FindPropertyRelative("minimumSDKVersion"));
            DrawProperty(aPProjectAndroidSettings.FindPropertyRelative("targetSDKVersion"));
            DrawProperty(aPProjectAndroidSettings.FindPropertyRelative("androidArchitecture"));
            DrawProperty(aPProjectAndroidSettings.FindPropertyRelative("ARMv7"));
            DrawProperty(aPProjectAndroidSettings.FindPropertyRelative("ARM64"));
        }

        if (selectedBuildTargetGroup == BuildTargetGroup.iOS)
        {
        }
        EditorGUILayout.EndBuildTargetSelectionGrouping();

        #endregion DrawProperty(propertyName)

        previousGUIBackground = GUI.backgroundColor;
        GUI.backgroundColor = Color.green;
        Space();
        DrawHorizontalLine();
        GUI.backgroundColor = previousGUIBackground;

        OnButtonPressed("Debug Build",
            () =>
            {
                scriptReference.SetProjectSetting();
            },
            new EditorButtonStyle
            {
                buttonColor = Color.white,
                buttonTextColor = Color.white
            });

        OnButtonPressed("Production Build",
            () =>
            {

            },
            new EditorButtonStyle
            {
                buttonColor = Color.cyan,
                buttonTextColor = Color.white
            });

        serializedObject.ApplyModifiedProperties();
    }
}
#endif