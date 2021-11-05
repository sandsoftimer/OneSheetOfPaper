using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if POTATO_FIREBASE
using Firebase;
#endif

namespace PotatoSDK
{
    public class FireBaseMan : MonoBehaviour,IPotatoInitiatable
    {
        bool enableModuleLogs = true;
        public string LogColorCode => "f5820d";
        public bool IsReady { get; set; }
        void IPotatoInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }

#if !POTATO_FIREBASE
        void IPotatoInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IPotatoInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }
#else
        

        void IPotatoInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IPotatoInitiatable> onModuleReadyToUse)
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    FirebaseApp firebaseApp = FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    onModuleReadyToUse?.Invoke(this);
                    IsReady = true;
                }
                else
                {
                    Debug.LogError(string.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }



        public bool enableTestAnalytics = false;
        IEnumerator Start()
        {
            int i = 0;
            yield return null;
            while (enableTestAnalytics)
            {
                if (IsReady)
                {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("testevent", "timepassed", i);
                    i += 2;
                    if (enableModuleLogs) "FireBase log sent".Log(LogColorCode);
                    yield return new WaitForSeconds(2);
                }
                else yield return null;

            }
        }
#endif
    }
}

