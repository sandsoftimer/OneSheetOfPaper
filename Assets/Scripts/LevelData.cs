using System.Collections;
using System.Collections.Generic;
using PotatoSDK;
using UnityEngine;

public class LevelData : APBehaviour
{
    public RuntimeLevelType runtimeLevelType;
    string nonSerializedLevelPath = "Levels/Level ";
    string serializedLevelPath = "Serialize Levels/new ";

    int totalGameLevels = 0;
    int nonSerializeLevelsCount = 200;
    int serializeLevelsCount = 20;
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.N))
        {
            gameplayData.currentLevelNumber++;
            gameManager.SaveGame();
            gameManager.NextLevel();
        }
#endif
        if (gameState.Equals(GameState.GAME_INITIALIZED) && Input.GetMouseButtonDown(0))
        {
            gameManager.ChangeGameState(GameState.GAME_PLAY_STARTED);
            gameState = GameState.GAME_PLAY_STARTED;
        }

        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

    }

    void FixedUpdate()
    {
        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

    }

    void LateUpdate()
    {
        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

    }

    #endregion ALL UNITY FUNCTIONS
    //=================================   
    #region ALL OVERRIDING FUNCTIONS

    public override void OnGameDataLoad()
    {
        base.OnGameDataLoad();

        int levelIndex = GetModedLevelNumber();
        if (gameManager.debugModeOn)
        {
            transform.GetChild(levelIndex).gameObject.SetActive(true);
        }
        else
        {
            string path = runtimeLevelType.Equals(RuntimeLevelType.NON_SERIALIZE) ? nonSerializedLevelPath : serializedLevelPath;
            Instantiate(Resources.Load(path + (levelIndex + 1)) as GameObject, transform).SetActive(true);
        }
    }

    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS

    int GetModedLevelNumber()
    {
        if (gameManager.debugModeOn)
        {
            totalGameLevels = transform.childCount;
        }
        else
        {
            runtimeLevelType = ABMan.GetValue_Int(ABtype.AB0_serialize) == 0? RuntimeLevelType.NON_SERIALIZE : RuntimeLevelType.SERIALIZE;
            totalGameLevels = runtimeLevelType.Equals(RuntimeLevelType.NON_SERIALIZE) ? nonSerializeLevelsCount : serializeLevelsCount;
        }
        return gameplayData.currentLevelNumber % totalGameLevels;
    }

#endregion ALL SELF DECLEAR FUNCTIONS

}
