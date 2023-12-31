using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : APBehaviour
{
    public LevelPrefabData levelPrefabData;
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
            Instantiate(levelPrefabData.levelDatas[gameManager.GetModedLevelNumber()], transform).SetActive(true);
        }
    }

    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS


    #endregion ALL SELF DECLEAR FUNCTIONS

}
