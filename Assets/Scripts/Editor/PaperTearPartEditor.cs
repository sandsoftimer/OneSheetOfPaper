#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Com.AlphaPotato.Utility;

[CustomEditor(typeof(PaperTearPart))]
public class PaperTearPartEditor : APEditor
{
    private PaperTearPart scriptReference;

    // All SerializedProperties
    #region ALL_PUBLIC_PROPERTIES
    private SerializedProperty draggingType;
    private SerializedProperty dependencyData;
    #endregion ALL_PUBLIC_PROPERTIES

    bool drawProperties = false;
    DraggingType draggingTypeLocal;
    public void OnEnable()
    {
        scriptReference = (PaperTearPart)target;
        #region FINDER_ALL_PUBLIC_PROPERTIES_FINDER
        draggingType = serializedObject.FindProperty("draggingType");
        dependencyData = serializedObject.FindProperty("dependencyData");
        #endregion FINDER_ALL_PUBLIC_PROPERTIES
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        draggingTypeLocal = (DraggingType)draggingType.enumValueIndex;
        DrawProperty(draggingType);

        if (draggingTypeLocal.Equals(DraggingType.DEPENDENT))
        {
            DrawHorizontalLine();
            DrawProperty(dependencyData);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
