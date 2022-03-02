using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeTearingLevelData : APBehaviour
{
    public string levelText;
    public Color backFaceColor = Color.white;
    public Vector3 cuttingSize;
    public AnimationData defaultDatas, rollingDatas, levelSuccessDatas;

    [HideInInspector]
    public Transform positiveCheckingPoints, negetiveCheckingPoints, snappingPoint;

    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();

        positiveCheckingPoints = transform.GetChild(1);
        negetiveCheckingPoints = transform.GetChild(2);
        snappingPoint = transform.GetChild(0);
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    void Update()
    {
        //if (gameState.Equals(GameState.GAME_INITIALIZED) && Input.GetMouseButtonDown(0))
        //{
        //    gameManager.ChangeGameState(GameState.GAME_PLAY_STARTED);
        //    gameState = GameState.GAME_PLAY_STARTED;
        //}

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
    
    
    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS
    
    
    #endregion ALL SELF DECLEAR FUNCTIONS

}
