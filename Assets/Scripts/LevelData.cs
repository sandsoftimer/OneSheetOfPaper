using System.Collections;
using System.Collections.Generic;
using PotatoSDK;
using UnityEngine;

public class LevelData : APBehaviour
{
    public RuntimeLevelType runtimeLevelType;
    string nonSerializedLevelPath = "Levels/Level ";
    string serializedLevelPath = "Serialize Levels/new ";
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

        if (gameManager.debugModeOn)
        {
            transform.GetChild(gameManager.GetModedLevelNumber()).gameObject.SetActive(true);
        }
        else
        {
#if !UNITY_EDITOR
            runtimeLevelType = ABMan.GetValue_Int(ABtype.AB0_serialize) == 0? RuntimeLevelType.NON_SERIALIZE : RuntimeLevelType.SERIALIZE;
#endif
            string path = runtimeLevelType.Equals(RuntimeLevelType.NON_SERIALIZE) ? nonSerializedLevelPath : serializedLevelPath;
            Instantiate(Resources.Load(path + (gameManager.GetModedLevelNumber() + 1)) as GameObject, transform).SetActive(true);
        }
    }

    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS


    #endregion ALL SELF DECLEAR FUNCTIONS

}
