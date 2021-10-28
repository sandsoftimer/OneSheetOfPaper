/*
 * Developer E-mail: sandsoftimer@gmail.com
 * Facebook Account: https://www.facebook.com/md.imran.hossain.902
 * 
 * Features:
 * Scene Loading Effect
 * GameData Save/Load
 * Camera Effects (Slow Mosition, Shake Camera)
 * Objects Pulling/Pushing Effect
 */

using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.alphapotato.utility
{
    [DefaultExecutionOrder(ConstantManager.APToolOrder)]
    public class APTools : MonoBehaviour, IHierarchyIcon
    {
        public static APTools Instance;

        public SceneManager sceneManager;
        public SavefileManager savefileManager;
        public CameraManager cameraManager;
        public PoolManager poolManager;
        public MathManager mathManager;
        public FunctionManager functionManager;
        public InputManager inputManager;

        #region Singleton Pattern
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion  Singleton Pattern

#if UNITY_EDITOR
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

        [MenuItem("APTools/Project Setup/Set Project Settings")]
        public static void SetupAPProjectSettings()
        {
            PlayerSettings.companyName = "Alpha Potato";
            PlayerSettings.productName = Application.productName;            
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.alphapotato.projectname");
            PlayerSettings.SetArchitecture(BuildTargetGroup.Android, (int)AndroidArchitecture.ARM64);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        }
#endif
        public string EditorIconPath { get { return "APSupportIcon"; } }
    }
}
