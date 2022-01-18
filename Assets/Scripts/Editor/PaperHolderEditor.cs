#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Com.AlphaPotato.Utility;

[CustomEditor(typeof(PaperHolder))]
public class PaperHolderEditor : APEditor
{
    private PaperHolder scriptReference;

    // All SerializedProperties
    #region ALL_PUBLIC_PROPERTIES
    private SerializedProperty levelText;
	//private SerializedProperty cameraPoint;
	//private SerializedProperty cameraFieldOfView;
	private SerializedProperty backgroundColor;
	private SerializedProperty defaultDatas;
	private SerializedProperty rollingDatas;
	private SerializedProperty levelSuccessDatas;
    public SerializedProperty texturePath; 
	#endregion ALL_PUBLIC_PROPERTIES

    public void OnEnable()
    {
        scriptReference = (PaperHolder)target;
        #region FINDER_ALL_PUBLIC_PROPERTIES_FINDER
        levelText = serializedObject.FindProperty("levelText");
		//cameraPoint = serializedObject.FindProperty("cameraPoint");
		//cameraFieldOfView = serializedObject.FindProperty("cameraFieldOfView");
		backgroundColor = serializedObject.FindProperty("backgroundColor");
		defaultDatas = serializedObject.FindProperty("defaultDatas");
		rollingDatas = serializedObject.FindProperty("rollingDatas");
        levelSuccessDatas = serializedObject.FindProperty("levelSuccessDatas");
        texturePath = serializedObject.FindProperty("texturePath");
        #endregion FINDER_ALL_PUBLIC_PROPERTIES
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        #region DrawProperty(propertyName)
        DrawProperty(levelText);
        //DrawProperty(cameraPoint);
        //DrawProperty(cameraFieldOfView);
        DrawProperty(backgroundColor);
        DrawProperty(defaultDatas);
        DrawProperty(rollingDatas);
        DrawProperty(levelSuccessDatas);
        #endregion DrawProperty(propertyName)

        Space();
        DrawHorizontalLine();
        DrawProperty(texturePath);
        Space(1);
        OnButtonPressed("Fetch",
            () =>
            {

            },
            new EditorButtonStyle
            {
                buttonColor = Color.white,
                buttonTextColor = Color.green
            });

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
