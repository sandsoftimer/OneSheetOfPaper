using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PotatoSDK
{
    [CustomEditor(typeof(AdjustIAPVerifyMan))]
    public class AdjPurchaseMan_Editor : SymbolControlledModuleEditor
    {
        public override string OnCoreModuleGUI()
        {

#if POTATO_IAP_VALIDATION
            base.OnCoreModuleGUI();

#endif
            return "POTATO_IAP_VALIDATION";
        }
    }
}