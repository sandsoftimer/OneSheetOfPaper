#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Com.AlphaPotato.Utility;

[CustomEditor(typeof(MeshDoctor))]
public class MeshDoctorEditor : APEditor
{
    private MeshDoctor scriptRef;
    
    private SerializedProperty createPlane, addCollider, planeXSize, planeZSize, planeGap, planeMaterial, pivot;

    public void OnEnable()
    {
        scriptRef = (MeshDoctor)target;
        createPlane = serializedObject.FindProperty("createPlane");
        addCollider = serializedObject.FindProperty("addCollider");
        planeXSize = serializedObject.FindProperty("planeXSize");
        planeZSize = serializedObject.FindProperty("planeZSize");
        planeGap = serializedObject.FindProperty("planeGap");
        planeMaterial = serializedObject.FindProperty("planeMaterial");
        pivot = serializedObject.FindProperty("pivot");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawProperty(createPlane);
        if (scriptRef.createPlane)
        {
            DrawProperty(addCollider);
            DrawProperty(planeXSize);
            DrawProperty(planeZSize);
            DrawProperty(planeGap);
            DrawProperty(planeMaterial);
            DrawProperty(pivot);

            Space(10);
            DrawHorizontalLine();
            OnButtonPressed("Create Mesh",
                ()=> {
                    scriptRef.CreateEditorPlane();
                },
                new EditorButtonStyle {
                    buttonColor = Color.yellow,
                    buttonTextColor = Color.white
                });
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif