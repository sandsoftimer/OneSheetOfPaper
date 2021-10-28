//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

//[CreateAssetMenu(menuName = "Level/GameLevelsData", fileName = "GameLevelsData")]
//public class GameLevelsData : ScriptableObject
//{
//    public TextAsset csvFile;
//    public List<LevelData> levelDatas;

//    public void OnFetchButtonPress()
//    {
//        string[] allRow = csvFile.text.Split('\n');

//        levelDatas = new List<LevelData>();

//        for (int i = 0; i < allRow.Length; i++)
//        {
//            LevelData levelData = new LevelData();

//            string[] column = allRow[i].Split(',');
//            levelData.levelNo = int.Parse(column[0]);
//            levelData.itemNames = column[1].Split(' ');
//            levelData.LevelEndMessage = column[2];

//            levelDatas.Add(levelData);

//        }
//    }

//    public void DeleteData()
//    {
//        levelDatas = new List<LevelData>();
//    }
//}

//=============================== Editor Script CSV Fetch & Delete Serialization ==================================

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(GameLevelsData))]
//public class GameLevelsDataEditor : APEditor
//{
//    private GameLevelsData gameLevelData;
//    private SerializedProperty csvFile, levelDatas;

//    public void OnEnable()
//    {
//        gameLevelData = (GameLevelsData)target;
//        csvFile = serializedObject.FindProperty("csvFile");
//        levelDatas = serializedObject.FindProperty("levelDatas");
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        EditorGUILayout.PropertyField(csvFile);

//        EditorGUILayout.Space();
//        if (GUILayout.Button("Fetch"))
//        {
//            gameLevelData.OnFetchButtonPress();
//            csvFile.serializedObject.ApplyModifiedProperties();
//            levelDatas.serializedObject.ApplyModifiedProperties();
//        }

//        GUIStyle style = new GUIStyle(GUI.skin.button);
//        style.normal.textColor = Color.red;
//        if (GUILayout.Button("Delete", style))
//        {
//            gameLevelData.DeleteData();
//        }

//        EditorGUILayout.Space();

//        DrawHorizontalLine();

//        EditorGUILayout.PropertyField(levelDatas);
//        serializedObject.ApplyModifiedProperties();
//    }
//}
