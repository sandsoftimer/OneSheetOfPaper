using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Project Setting", menuName = "APTools/Project Setup/APProjectSettings")]
public class APProjectSetting : ScriptableObject
{
    public string companyName = "Alpha Potato";

#if UNITY_2021
public static bool showIOSSettings = false;
#else
    public static bool showIOSSettings = false;
#endif
    private static bool showWP8Settings = false;
    private static bool showAndroidSettings = false;

    //public string productName = Application.productName;            
    //UIOrientation defaultInterfaceOrientation = UIOrientation.Portrait;
}
