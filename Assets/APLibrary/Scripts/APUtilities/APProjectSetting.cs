using System;
using UnityEditor;
using UnityEngine;

public enum APPlatforms
{
    ANDROID,
    IOS
}

[CreateAssetMenu(fileName = "Project Setting", menuName = "APTools/Project Setup/APProjectSettings")]
public class APProjectSetting : ScriptableObject
{
    public APProjectDefaultSettings aPProjectDefaultSettings;
    public APProjectAndroidSettings aPProjectAndroidSettings;
    public APProjectIOSSettings aPProjectIOSSettings;

    //public BuildTargetGroup applicationIdentifier;
    public UIOrientation defaultInterfaceOrientation;

    public void OnEnable()
    {
        aPProjectDefaultSettings = new APProjectDefaultSettings();
        aPProjectAndroidSettings = new APProjectAndroidSettings();
        aPProjectIOSSettings = new APProjectIOSSettings();

        aPProjectDefaultSettings.companyName = PlayerSettings.companyName;
        aPProjectDefaultSettings.productName = Application.productName;

        aPProjectAndroidSettings.applicationIdentifier = "com.alphapotato." + Application.productName.ToLower().Replace(" ", "");
        aPProjectIOSSettings.applicationIdentifier = "com.alphapotato." + Application.productName.ToLower().Replace(" ", "");

        //applicationIdentifier = BuildTargetGroup.Android;
        defaultInterfaceOrientation = UIOrientation.Portrait;        
    }

}

public class SetProjectSettings{

    public SetProjectSettings(APProjectSetting aPProjectSetting)
    {
        PlayerSettings.companyName = aPProjectSetting.aPProjectDefaultSettings.companyName;
        PlayerSettings.productName = Application.productName;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.alphapotato.projectname");
        PlayerSettings.SetArchitecture(BuildTargetGroup.Android, (int)AndroidArchitecture.ARM64);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

    }
}

[Serializable]
public class APProjectDefaultSettings
{
    public string companyName;
    public string productName;
    public string applicationIdentifier;

}

[Serializable]
public class APProjectAndroidSettings
{
    public string applicationIdentifier;
}

[Serializable]
public class APProjectIOSSettings
{
    public string applicationIdentifier;

}
