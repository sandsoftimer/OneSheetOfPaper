using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if POTATO_ADJUST
using com.adjust.sdk;
#endif


namespace PotatoSDK
{
    public class AdjustMan : MonoBehaviour, IPotatoInitiatable
    {
        public IDKeeper appToken;

        bool enableModuleLogs = true;
        public string LogColorCode => "4B5F81";
        public bool IsReady { get; set; }
        void IPotatoInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }
#if !POTATO_ADJUST
        void IPotatoInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IPotatoInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }
#else
        private const string errorMsgEditor = "[Adjust]: SDK can not be used in Editor.";
        private const string errorMsgStart = "[Adjust]: SDK not started. Start it manually using the 'start' method.";
        private const string errorMsgPlatform = "[Adjust]: SDK can only be used in Android, iOS, Windows Phone 8.1, Windows Store or Universal Windows apps.";
        public static AdjustMan Instance { get; private set; }


        public static bool isTestMode
        {
            get
            {
                return string.Equals(Application.identifier, "com.portbliss.icycake");

            }
        }

        private IDKeeper _appTokenTest;
        private IDKeeper appTokenTest
        {
            get
            {
                if (_appTokenTest == null)
                {
                    _appTokenTest = new IDKeeper("345cj8h1np1c", "fkj5de2klvcw");
                }
                return _appTokenTest;
            }
        }
       
        public string activeAppToken
        {
            get
            {
                return isTestMode ? appTokenTest.id : appToken.id;
            }
        }

       
        void IPotatoInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IPotatoInitiatable> onModuleReadyToUse)
        {
            AdjustConfig adjustConfig = new AdjustConfig(activeAppToken, AdjustEnvironment.Production, false);
            adjustConfig.setLogLevel(AdjustLogLevel.Info);
            adjustConfig.setSendInBackground(false);
            adjustConfig.setEventBufferingEnabled(false);
            adjustConfig.setLaunchDeferredDeeplink(true);
            adjustConfig.setDelayStart(0);

            Adjust.start(adjustConfig);

            if (isTestMode)
            {
                IDKeeper testEventID = new IDKeeper(androidID: "jd8jb6", iosID: "o99mnl");
                AdjustEvent testAdjustEvent = new AdjustEvent(testEventID.id);
                Adjust.trackEvent(testAdjustEvent);
            }
            Instance = this; 
            
            onModuleReadyToUse?.Invoke(this);
            IsReady = true;
        }
        void OnApplicationPause(bool pauseStatus)
        {
            if (!IsReady) return;

#if UNITY_IOS && !UNITY_EDITOR
            return;
#endif
            if (IsEditor())
            {
                return;
            }

#if UNITY_ANDROID
            if (pauseStatus)
            {
                AdjustAndroid.OnPause();
            }
            else
            {
                AdjustAndroid.OnResume();
            }
#else
            if (enableModuleLogs) errorMsgPlatform.Log(LogColorCode);
#endif
        }
        private  bool IsEditor()
        {
#if UNITY_EDITOR
            if (enableModuleLogs) errorMsgPlatform.Log(errorMsgEditor);
            return true;
#else
            return false;
#endif

        }


#endif
        }
}
