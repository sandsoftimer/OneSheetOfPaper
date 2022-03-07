using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputChecker2D : APBehaviour
{
    PolygonCollider2D polygonCollider2D;
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
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

    public void UpdateCollider()
    {
        if (polygonCollider2D != null)
            DestroyImmediate(polygonCollider2D);

        polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
    }
    
    #endregion ALL SELF DECLEAR FUNCTIONS

}
