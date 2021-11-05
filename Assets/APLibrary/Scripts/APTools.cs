/*
 * Developer Name: Md. Imran Hossain
 * E-mail: sandsoftimer@gmail.com
 * FB: https://www.facebook.com/md.imran.hossain.902
 * in: https://www.linkedin.com/in/md-imran-hossain-69768826/
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

namespace Com.AlphaPotato.Utility
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
            Input.backButtonLeavesApp = true;
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

        public static void SetupAPProjectSettings()
        {
            PlayerSettings.companyName = "Alpha Potato";
            PlayerSettings.productName = Application.productName;            
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.alphapotato.projectname");
            PlayerSettings.SetArchitecture(BuildTargetGroup.Android, (int)AndroidArchitecture.ARM64);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        }

        [MenuItem("APTools/Project Setup/APProjectSettings")]
        public static void CreateProjectSettings()
        {
            //if (Selection.activeObject is MonoScript)
            //{
            //    MonoScript ms = (MonoScript)Selection.activeObject;
            //    ScriptableObject so = ScriptableObject.CreateInstance(ms.name);

            //    string path = System.IO.Directory.GetParent(AssetDatabase.GetAssetPath(ms.GetInstanceID())) + "/" + ms.name + ".asset";
            //    Selection.activeObject = CreateIfDoesntExists(path, so);
            //}

            ScriptableObject so = ScriptableObject.CreateInstance(typeof(APProjectSetting));

            string path = ConstantManager.APPROJECTSETTING_DESTINATION_PATH;
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
#endif
        public string EditorIconPath { get { return "APSupportIcon"; } }
    }
}
