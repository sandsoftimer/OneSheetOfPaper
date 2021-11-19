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

        public string EditorIconPath { get { return "APSupportIcon"; } }
    }
}
