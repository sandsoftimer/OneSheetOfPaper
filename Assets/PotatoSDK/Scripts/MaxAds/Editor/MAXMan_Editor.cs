using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PotatoSDK
{
    [CustomEditor(typeof(MAXMan))]
    public class MAXMan_Editor : SymbolControlledModuleEditor
    {

        public override string OnCoreModuleGUI()
        {
#if POTATO_MAX
            base.OnCoreModuleGUI();
#endif
            return "POTATO_MAX";
        }
    }
}