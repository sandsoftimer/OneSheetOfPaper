using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PotatoSDK
{
#if POTATO_IAP
    [CustomPropertyDrawer(typeof(NonConsumablePotato))]
    public class NonConsumableDrawer : PurchasableDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }

    }
#endif
}

