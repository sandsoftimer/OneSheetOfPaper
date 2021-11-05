using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PotatoSDK
{
    [CustomEditor(typeof(ABMan))]
    public class ABMan_Editor : SymbolControlledModuleEditor
    {

        bool wasDirty;

        public override string OnCoreModuleGUI()
        {

#if POTATO_AB_TEST && POTATO_MAX
            ABMan abman = target as ABMan;

            EditorGUI.BeginChangeCheck();
            base.OnCoreModuleGUI();
            if (EditorGUI.EndChangeCheck())
            {
                wasDirty = true;

            }
            serializedObject.ApplyModifiedProperties();
            if (wasDirty)
            {
                if (GUILayout.Button("Force Refresh Enum", GUILayout.Height(35)))
                {
                    RefreshEnum();
                }
            }

#endif
#if !POTATO_MAX
#if POTATO_AB_TEST
            EditorGUILayout.HelpBox("You must have MAX sdk active to use AB testing", MessageType.Error);
#else
            EditorGUILayout.HelpBox("You must have MAX sdk active to use AB testing", MessageType.Warning);
#endif
#endif

            return "POTATO_AB_TEST";
        }



        public void RefreshEnum()
        {
            ABMan abMan = target as ABMan;

            int N = abMan.AB_entries.Count;
            string[] enumEntries = new string[N];
            int k = 0;
            foreach (var abEntry in abMan.AB_entries)
            {
                abEntry.title = abEntry.title.CleanWhiteSpace();
                abEntry.ab_type = (ABtype)k;
                enumEntries[k] = string.Format("AB{0}_{1}", k, abEntry.title);
                k++;
            }


            string enumName = "ABtype";
            string filePathAndName = "Assets/PotatoSDK/Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("namespace PotatoSDK");
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("\tpublic enum " + enumName);
                streamWriter.WriteLine("\t{");
                for (int i = 0; i < enumEntries.Length; i++)
                {
                    streamWriter.WriteLine(string.Format("\t\t{0} = {1},", enumEntries[i], i));
                }
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
            }
            SDKEditorUtilityMan.SetObjectDirty(abMan);
            AssetDatabase.Refresh();
            wasDirty = false;
        }
    }
}