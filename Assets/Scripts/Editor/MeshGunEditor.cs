using UnityEditor;
using UnityEngine;
using Com.AlphaPotato.Utility;

[CustomEditor(typeof(MeshGun))]
public class MeshGunEditor : APEditor
{
    private MeshGun scriptRef;
    
    //private SerializedProperty propertyName;

    public void OnEnable()
    {
        scriptRef = (MeshGun)target;
        //propertyName = serializedObject.FindProperty("propertyName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //DrawProperty(propertyName);

        Space();
        DrawHorizontalLine();

        OnButtonPressed("Fetch",
            ()=> {
                //scriptRef.OnFetchButtonPress();
                //propertyName.serializedObject.ApplyModifiedProperties();
                Debug.LogError("Nothing has implemented. Double-Click to implement.");
            });

        OnButtonPressed("Delete",
            ()=> {
                //scriptRef.DeleteData();
                //propertyName.serializedObject.ApplyModifiedProperties();
                Debug.LogError("Nothing has implemented. Double-Click to implement.");
            },
            new EditorButtonStyle {
                buttonColor = Color.yellow,
                buttonTextColor = Color.red
            });

        serializedObject.ApplyModifiedProperties();
    }
}
