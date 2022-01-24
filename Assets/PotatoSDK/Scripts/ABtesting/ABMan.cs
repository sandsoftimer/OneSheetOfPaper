using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoSDK
{
    public class ABMan : MonoBehaviour, IPotatoInitiatable
    {
        public LogLevel loglevel = LogLevel.Important;
        public string LogColorCode => "22AA22";
        public bool IsReady { get; set; }
        void IPotatoInitiatable.ForceDisableLogs()
        {
            loglevel = LogLevel.None;
        }




        private static ABMan Instance;
        private Dictionary<ABtype, HardAB> _allSettings;
        public Dictionary<ABtype, HardAB> allSettings
        {
            get
            {
                if (_allSettings == null) Init();
                return _allSettings;
            }
        }
        private void Init()
        {
            Instance = this;
            HardAB.logColorCode = LogColorCode;
            HardAB.ActiveLogLevel = loglevel;
#if POTATO_MAX
            _allSettings = new Dictionary<ABtype, HardAB>();

            foreach (var entry in AB_entries)
            {
                HardAB hardAB=null;
#if UNITY_IOS

                if (!entry.disable_iOS) hardAB = new HardAB(entry.ab_type, entry.title, entry.editorDefaultValue, entry.isVolatile);
#elif UNITY_ANDROID
                if (!entry.disable_Android) hardAB = new HardAB(entry.ab_type, entry.title, entry.editorDefaultValue, entry.isVolatile);
#endif
                if (hardAB != null)
                {
                    entry.hardAB = hardAB;
                    allSettings.Add(entry.ab_type, hardAB);
                    hardAB.Assign_IfUnassigned(MaxSdk.VariableService.GetString(hardAB.GetKey()));
                }
            }
#endif
        }

        public static HardAB GetABAccess(ABtype type)
        {
            if (!Instance) return null;
            return Instance.allSettings[type];
        }
        public GameObject prefabReference;
        public bool enableABTestPanel;
        public List<ABKeep> AB_entries;

        void IPotatoInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IPotatoInitiatable> onModuleReadyToUse)
        {
#if POTATO_AB_TEST && POTATO_MAX
            Init();
            if (enableABTestPanel)
            {

                GameObject go = Instantiate(prefabReference);
                go.transform.SetParent(this.transform);
                go.GetComponent<TestABController>().Init(()=> 
                {
                    onModuleReadyToUse?.Invoke(this);
                    IsReady = true;
                }, AB_entries);

            }
            else
            {
                onModuleReadyToUse?.Invoke(this);
                IsReady = true;
            }
#else
            onModuleReadyToUse?.Invoke(this);
            IsReady = true;
#endif
        }

        public static int GetValue_Int(ABtype type)
        {
            if (!Instance) Debug.LogError("AB testing instance not found");
            int value;
            GetABAccess(type).GetValue(out value);
            return value;
        }
        public static float GetValue_Float(ABtype type)
        {
            if (!Instance) Debug.LogError("AB testing instance not found");
            float value;
            GetABAccess(type).GetValue_float(out value);
            return value;
        }
        public static bool GetValue_Bool(ABtype type)
        {
            if (!Instance) Debug.LogError("AB testing instance not found");
            bool value;
            GetABAccess(type).GetValue(out value);
            return value;
        }
        public static string GetValue_String(ABtype type)
        {
            if (!Instance) Debug.LogError("AB testing instance not found");
            string value;
            GetABAccess(type).GetValue(out value);
            return value;
        }
    }
    [System.Serializable]
    public class ABKeep
    {
        public string title;
        public ABtype ab_type;
        public bool disable_iOS;
        public bool disable_Android;
        public bool isVolatile;
        public string editorDefaultValue;
        public List<ABDisplayDefinitions> displayDefinitions;

        [System.NonSerialized] public HardAB hardAB;
    }

    [System.Serializable]
    public class ABDisplayDefinitions
    {
        public string value;
        public string significance;
    }

}
