using System;
using System.Collections;
using UnityEngine;

#if POTATO_IAP
using UnityEngine.Purchasing;
#if POTATO_IAP_VALIDATION
using com.adjust.sdk.purchase;
#endif
#endif


namespace PotatoSDK
{
    public class AdjustIAPVerifyMan : MonoBehaviour, IPotatoInitiatable
    {
        bool enableModuleLogs = true;
        public string LogColorCode => "b78628";
        public bool IsReady { get; set; }
        void IPotatoInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }
#if !POTATO_IAP_VALIDATION || !POTATO_IAP
        void IPotatoInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IPotatoInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }
#else
        [SerializeField] bool verifyConsumable = false;
        [SerializeField] bool verifyNonConsumable = true;
        bool verifySubscription = false;
        public static AdjustIAPVerifyMan Instance { get; private set; }

        System.Action<IPotatoInitiatable> onModuleReadyToUse;
        void IPotatoInitiatable.InitializeSuperEarly(bool hasConsent, System.Action<IPotatoInitiatable> onModuleReadyToUse)
        {
            this.onModuleReadyToUse = onModuleReadyToUse;
            VerificationRequest.enableModuleLogs = enableModuleLogs;
            VerificationRequest.LogColorCode = LogColorCode;
            StartCoroutine(WaitForAdjust());
        }
        IEnumerator WaitForAdjust()
        {
            while (!AdjustMan.Instance)
            {
                yield return null;
            }
            if (enableModuleLogs) "Adjust purchase verifier initiated".Log(LogColorCode);
            ADJPConfig config = new ADJPConfig(AdjustMan.Instance.appToken, ADJPEnvironment.Production);
            config.SetLogLevel(ADJPLogLevel.Error);
            AdjustPurchase.Init(config);
            Instance = this;
            onModuleReadyToUse?.Invoke(this);
            IsReady = true;
        }

        public static void VerifyPurchase(Product product, Action<bool> onVerificationPassedOrNotRequired)
        {
            if (Instance == null)
            {
                Debug.LogError("Adjust Purchase not initialized!");
                onVerificationPassedOrNotRequired?.Invoke(false);
                return;
            }
            switch (product.definition.type)
            {
                case ProductType.Consumable:
                    if (!Instance.verifyConsumable)
                    {
                        onVerificationPassedOrNotRequired?.Invoke(true);
                        return;
                    }
                    break;
                case ProductType.NonConsumable:
                    if(!Instance.verifyNonConsumable)
                    {
                        onVerificationPassedOrNotRequired?.Invoke(true);
                        return;
                    }
                    break;
                case ProductType.Subscription:
                    if (!Instance.verifySubscription)
                    {
                        onVerificationPassedOrNotRequired?.Invoke(true);
                        return;
                    }
                    break;
                default:
                    {
                        Debug.LogError("Unknown situation!");
                        onVerificationPassedOrNotRequired?.Invoke(true);
                        return;
                    }
            }
            new VerificationRequest(product, onVerificationPassedOrNotRequired);
        }
#endif

    }
}