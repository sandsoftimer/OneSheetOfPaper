using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PotatoSDK
{
    [CustomEditor(typeof(FaceBookMan))]
    public class FaceBookMan_Editor : SymbolControlledModuleEditor
    {
        public override string OnCoreModuleGUI()
        {
#if POTATO_FACEBOOK
            base.OnCoreModuleGUI();
#endif
            return "POTATO_FACEBOOK";
        }
    }
}