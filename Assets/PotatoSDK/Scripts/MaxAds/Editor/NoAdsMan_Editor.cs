using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace PotatoSDK
{
    [CustomEditor(typeof(NoAdsMan))]
    public class NoAdsMan_Editor : SymbolControlledModuleEditor
    {
        public override string OnCoreModuleGUI()
        {

            bool noad_active = false;
            bool iap_active = false;
            bool max_active = false;
#if POTATO_NOAD
            noad_active = true;
#endif
#if POTATO_MAX 
            max_active = true;
#endif
#if POTATO_IAP
            iap_active = true;
#endif 
            if (noad_active && iap_active)
            {
                base.OnCoreModuleGUI();
                if (!max_active) EditorGUILayout.HelpBox("MAX sdk/wrapper not enabled, no_ads wrapper will not have any automatic effects", MessageType.Warning);
            }
            else if (noad_active && !iap_active)
            {
                EditorGUILayout.HelpBox("You must have InAppPurchase sdk+wrapper active to use no ads", MessageType.Error);
            }
            else if (!noad_active && !iap_active)
            {
                EditorGUILayout.HelpBox("You must have InAppPurchase sdk+wrapper active to use no ads", MessageType.Info);
            }


            return "POTATO_NOAD";
        }
    }
}