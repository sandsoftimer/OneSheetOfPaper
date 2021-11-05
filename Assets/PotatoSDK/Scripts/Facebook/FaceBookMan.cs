
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if POTATO_FACEBOOK
using Facebook.Unity;
#endif
namespace PotatoSDK
{
    public class FaceBookMan : MonoBehaviour, IPotatoInitiatable
    {
        bool enableModuleLogs = true;
        public string LogColorCode => "29487d";
        public bool IsReady { get; set; }
        void IPotatoInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }

#if !POTATO_FACEBOOK
        void IPotatoInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IPotatoInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }
#else
        public bool enableTestAnalytics = false;

        void IPotatoInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IPotatoInitiatable> onModuleReadyToUse)
        {
            if (FB.IsInitialized)
            {
                OnFBInitializationConfirmed(hasConsent);

                onModuleReadyToUse?.Invoke(this);
                IsReady = true;
            }
            else
            {

                FB.Init(() => {
                    OnFBInitializationConfirmed(hasConsent);

                    onModuleReadyToUse?.Invoke(this);
                    IsReady = true;
                });
            }
        }
        void OnFBInitializationConfirmed(bool hasConsent)
        {
            FB.ActivateApp();
#if UNITY_IOS

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if POTATO_MAX
                FB.Mobile.SetAdvertiserTrackingEnabled(hasConsent);
                AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(advertiserTrackingEnabled: hasConsent);
#endif
            }
            if (enableModuleLogs) string.Format("Consent={0}, will be sent if run from iOS device",hasConsent).Log(LogColorCode);
#endif
        }


        IEnumerator Start()
        {
            int i = 0;
            while (enableTestAnalytics)
            {
                if (FB.IsInitialized)
                {
                    var paramDic = new Dictionary<string, object>();
                    paramDic.Add("TP", i);
                    FB.LogAppEvent("time_passed", parameters: paramDic);
#if !UNITY_EDITOR
                    if (enableModuleLogs) "FB log sent".Log(LogColorCode);
#endif
                    i += 2;
                }
                yield return new WaitForSeconds(2);
            }
        }
#endif
        }
    }

