using System;
using System.Collections;
using UnityEngine;
namespace PotatoSDK
{
    public class NoAdsMan : MonoBehaviour, IPotatoInitiatable
    {
        public static bool isBlockingAds = false;
        bool enableModuleLogs;
        string IPotatoInitiatable.LogColorCode => "ffffff";

        public bool IsReady { get; set; }

        void IPotatoInitiatable.ForceDisableLogs()
        {
            enableModuleLogs = false;
        }


#if POTATO_NOAD && POTATO_IAP 
        public PurchaseType productIdentifier;
        NonConsumablePotato noadProduct;
        void IPotatoInitiatable.InitializeSuperEarly(bool hasConsent, Action<IPotatoInitiatable> onModuleReadyToUse)
        {
            StartCoroutine(WaitForInit());
            onModuleReadyToUse?.Invoke(this);
        }
        IEnumerator WaitForInit()
        {
            while (!InAppPurchaseMan.Instance)
            {
                yield return null;
            }
            noadProduct = InAppPurchaseMan.GetProduct(productIdentifier) as NonConsumablePotato;
            if (noadProduct == null)
            {
                Debug.LogError("no ad product not defined or is not NonConsumable");
            }
            else
            {
                noadProduct.OnItemAvailable ( () =>
                {
                    isBlockingAds = true;
                });
            }
        }

#else
        void IPotatoInitiatable.InitializeSuperEarly(bool hasConsent, Action<IPotatoInitiatable> onModuleReadyToUse)
        {
            onModuleReadyToUse?.Invoke(this);
        }
#endif
    }
}