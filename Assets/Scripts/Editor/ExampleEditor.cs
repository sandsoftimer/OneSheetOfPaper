using UnityEditor;
using UnityEngine;
using Com.AlphaPotato.Utility;

[CustomEditor(typeof(Example))]
public class ExampleEditor : APEditor
{
    private Example scriptReference;

    // All SerializedProperties
    #region ALL_PUBLIC_PROPERTIES
    private SerializedProperty floatVar;
	private SerializedProperty intVar;
	private SerializedProperty stringVar;
	#endregion ALL_PUBLIC_PROPERTIES

    bool drawProperties = true;
    public void OnEnable()
    {
        scriptReference = (Example)target;
        #region FINDER_ALL_PUBLIC_PROPERTIES_FINDER
        floatVar = serializedObject.FindProperty("floatVar");
		intVar = serializedObject.FindProperty("intVar");
		stringVar = serializedObject.FindProperty("stringVar");
		#endregion FINDER_ALL_PUBLIC_PROPERTIES
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (!drawProperties)
        {

            Space();
            DrawHorizontalLine();
            OnButtonPressed("Draw Properties",
                () =>
                {
                    drawProperties = !drawProperties;
                });
        }
        else
        {
            #region DrawProperty(propertyName)
            DrawProperty(floatVar);
			DrawProperty(intVar);
			DrawProperty(stringVar);
			#endregion DrawProperty(propertyName)

            Space();
            DrawHorizontalLine();
            OnButtonPressed("Hide Properties",
                () =>
                {
                    drawProperties = !drawProperties;
                },
                new EditorButtonStyle
                {
                    buttonColor = Color.yellow,
                    buttonTextColor = Color.red
                });
        }

        serializedObject.ApplyModifiedProperties();
    }
}
