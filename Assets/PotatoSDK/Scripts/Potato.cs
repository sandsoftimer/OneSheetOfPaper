using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if AP_LIONSTUDIO_SDK_INSTALLED
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Debugging;
#endif
namespace PotatoSDK
{
    public class Potato : MonoBehaviour
    {
        public bool disablePotatoLogs;
        public bool skipUsingSplashScene;
        public bool skipAutoLoadNextScene;
        public int autoLoadSceneIndex=1;
        const string LogColorCode = "00ffff";
        public static bool IsReady { get; private set; }
        public static float progress { get; private set; }
        public static bool initialized { get; private set; }

        public static bool consentDialogApplicableDetected { get; private set; }
        public static bool ios14_5plusDetected { get; private set; }



        public static bool hasConsent { get; private set; }
        static System.Action<bool> onInitializationCallback;

        static GameObject potatoGo;
               

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitialLoad()
        {
            initialized = false;
#if !UNITY_IOS

            hasConsent = true;
#else
            hasConsent = false;
#endif
#if POTATO_MAX
            
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
#if UNITY_IOS
                if (MaxSdkUtils.CompareVersions(UnityEngine.iOS.Device.systemVersion, "14.5") != MaxSdkUtils.VersionComparisonResult.Lesser)
                {
                    // Note that App transparency tracking authorization can be checked via `sdkConfiguration.AppTrackingStatus` for Unity Editor and iOS targets
                    // 1. Set Facebook ATE flag here, THEN
                    hasConsent = (sdkConfiguration.AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized);
                    ios14_5plusDetected = true;

                }
                else
                {
                    hasConsent = true;
                }
#else
                hasConsent = true;
#endif

                consentDialogApplicableDetected = (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies);
                Debug.Log("<color=#00ffff>Max Initialized</color>");
                OtherInit();
            };
            MaxSdk.SetVerboseLogging(false);
            MaxSdk.SetSdkKey("Lzi5VR_J50y55PM5ctwAwALT5d9g1CKMhT1TF0naOa4fSUn98Vd6rXsvAp4I3A-5LaPvNk4RSvKe5fesxKhRzh");
            MaxSdk.InitializeSdk();
#else
            OtherInit();
#endif
        }

        static Potato potato;

        static void OtherInit()
        {
            if (initialized) return;


            potatoGo =  MonoBehaviour.Instantiate( Resources.Load<GameObject>("PotatoSDK"));
            potato = potatoGo.GetComponent<Potato>();
            GameObject.DontDestroyOnLoad(potatoGo);

            foreach (Transform tr in potatoGo.transform)
            {
                foreach (var mono in tr.GetComponents<MonoBehaviour>())
                {
                    IPotatoInitiatable ip = mono as IPotatoInitiatable;
                    if (ip != null)
                    {
                        potato.initialCount++;
                        potato.mods.Add(ip);
                        if (potato.disablePotatoLogs) ip.ForceDisableLogs();
                        try
                        {
                            ip.InitializeSuperEarly(hasConsent, ModuleReadyToUse);
                        }
                        catch(Exception e)
                        {
                            Debug.LogError(e.Message);
                            Debug.LogErrorFormat("A potato SDK wrapper has failed to initialize properly!: {0}", mono.GetType());
                            if (potato.mods.Contains(ip))
                            {
                                potato.mods.Remove(ip);
                            }
                        }
                    }
                }
            }


            potato.StartCoroutine(potato.WaitForReady());

            onInitializationCallback?.Invoke(hasConsent);
            initialized = true;

        }

        static void ModuleReadyToUse(IPotatoInitiatable readyPot)
        {
            if (potato.mods.Contains(readyPot))
            {
                potato.mods.Remove(readyPot);
            }
            else Debug.LogError("Ready potato is not listed!");
        }

        float initialCount;
        List<IPotatoInitiatable> mods = new List<IPotatoInitiatable>();
        IEnumerator WaitForReady()
        {
            if (!potato.disablePotatoLogs) ("Potato Created").Log(LogColorCode);
            float lastReadiness=-1;
            while (mods.Count>0)
            {
                progress = 1 - (mods.Count / initialCount);
                if (progress != lastReadiness)
                {
                    if (!potato.disablePotatoLogs)
                    {
                        if (PotatoSplash.Instance)
                        {
                            PotatoSplash.Instance.SetProgress(progress);
                        }
                        else
                        {
                            string.Format("Potato readiness: {0}", progress).Log(LogColorCode);
                        }
                    }
                }
                yield return new WaitForSeconds(0.25f);
                lastReadiness = progress;
            }
            if (!potato.disablePotatoLogs) ("Potato Is Ready").Log(LogColorCode);
            if (!skipUsingSplashScene)
            {
                while (PotatoSplash.Instance == null)
                {
                    yield return null;
                }
                PotatoSplash.Instance.SetProgress(1);
                yield return new WaitForSeconds(0.5f);
                if (!skipAutoLoadNextScene)
                {
#if AP_LIONSTUDIO_SDK_INSTALLED
                    LionAnalytics.GameStart();
                    LionDebugger.Hide();
#endif
                    if (autoLoadSceneIndex > 0)
                    {
                        SceneManager.LoadScene(autoLoadSceneIndex);
                    }
                    else
                    {
                        SceneManager.LoadScene(1);
                    }
                }
            }

            IsReady = true;
        }


        public static void ExecuteOnInitConfirmed(System.Action<bool> onConfirmed)
        {
            if (initialized)
            {
                onConfirmed?.Invoke(hasConsent);
            }
            else
            {
                onInitializationCallback += onConfirmed;
            }
        }
    }

    public interface IPotatoInitiatable
    {
        void InitializeSuperEarly(bool hasConsent, System.Action<IPotatoInitiatable> onModuleReadyToUse);
        string LogColorCode { get; }
        bool IsReady { get; set; }
        void ForceDisableLogs();

    }

    public enum LogLevel
    {
        None = 0,
        Critical = 1,
        Important = 2,
        All = 3,
    }
}

