using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PotatoSDK
{
    [CustomEditor(typeof(FireBaseMan))]
    public class FireBaseMan_Editor : SymbolControlledModuleEditor
    {
        public override string OnCoreModuleGUI()
        {
#if POTATO_FIREBASE
            base.OnCoreModuleGUI();
#endif 
            return "POTATO_FIREBASE";
        }
    }
}