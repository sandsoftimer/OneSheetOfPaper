#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public enum APPlatforms
{
    ANDROID,
    IOS
}

[CreateAssetMenu(fileName = "Project Setting", menuName = "APTools/Project Setup/APProjectSetting")]
public class APProjectSetting : ScriptableObject
{
    public APProjectDefaultSettings aPProjectDefaultSettings;
    public APProjectAndroidSettings aPProjectAndroidSettings;
    public APProjectIOSSettings aPProjectIOSSettings;

    //public BuildTargetGroup applicationIdentifier;

    public void OnEnable()
    {
        aPProjectDefaultSettings = new APProjectDefaultSettings();
        aPProjectAndroidSettings = new APProjectAndroidSettings();
        aPProjectIOSSettings = new APProjectIOSSettings();

        aPProjectDefaultSettings.companyName = PlayerSettings.companyName;
        aPProjectDefaultSettings.productName = PlayerSettings.productName;
        aPProjectDefaultSettings.icon = AssetDatabase.LoadAssetAtPath("Assets/APLibrary/Sprites/AlphaPotatoIcon.png", typeof(Texture2D)) as Texture2D;
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] {aPProjectDefaultSettings.icon});

        aPProjectDefaultSettings.bundleIdentifier = "com.alphapotato." + Application.productName.ToLower().Replace(" ", "");
        aPProjectAndroidSettings.bundleIdentifier = aPProjectDefaultSettings.bundleIdentifier;
        aPProjectIOSSettings.bundleIdentifier = aPProjectDefaultSettings.bundleIdentifier;

        aPProjectDefaultSettings.defaultInterfaceOrientation = UIOrientation.Portrait;

        aPProjectAndroidSettings.androidArchitecture = ScriptingImplementation.IL2CPP;
    }

    public void SetProjectSetting()
    {
        PlayerSettings.companyName = aPProjectDefaultSettings.companyName;
        PlayerSettings.productName = aPProjectDefaultSettings.productName;
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { aPProjectDefaultSettings.icon });
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.alphapotato.projectname");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, aPProjectAndroidSettings.androidArchitecture);

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

        AndroidArchitecture aac = AndroidArchitecture.None;
        if(aPProjectAndroidSettings.ARMv7)
            aac |= AndroidArchitecture.ARMv7;
        if (aPProjectAndroidSettings.ARM64)
            aac |= AndroidArchitecture.ARM64;
        PlayerSettings.Android.targetArchitectures = aac;
    }
}

[Serializable]
public class APProjectDefaultSettings
{
    public string companyName;
    public string productName;
    public string bundleIdentifier;
    public Texture2D icon;
    public bool unitySplashScreen;
    public UIOrientation defaultInterfaceOrientation;
}

[Serializable]
public class APProjectAndroidSettings
{
    public bool overrideBundleIdentifier;
    public bool ARMv7;
    public bool ARM64;
    public string bundleIdentifier;
    public float versionNumber;
    public int debugBuildNumber, producitonBuildNumber;
    public AndroidSdkVersions minimumSDKVersion = AndroidSdkVersions.AndroidApiLevel21;
    public AndroidSdkVersions targetSDKVersion = AndroidSdkVersions.AndroidApiLevelAuto;
    public ScriptingImplementation androidArchitecture;
}

[Serializable]
public class APProjectIOSSettings
{
    public bool overrideBundleIdentifier;
    public string bundleIdentifier;
    public float versionNumber;
    public int debugBuildNumber, producitonBuildNumber;
}
#endif