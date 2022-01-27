using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Tutorial_Hand : APBehaviour
{
    Transform endPoint;
    Vector3 startingPosition;
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();
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

    public void Initialize(Transform endPoint)
    {
        this.endPoint = endPoint;
        MoveHand();
    }

    void MoveHand()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.localPosition = Vector3.zero;
        transform.DOMove(endPoint.position, ConstantManager.DEFAULT_ANIMATION_TIME).SetEase(Ease.Linear).OnComplete(()=> {

            transform.GetChild(0).gameObject.SetActive(false);
            Invoke("MoveHand", 2.5f);
        });
    }

    public void Destroy()
    {
        CancelInvoke("MoveHand");
        Destroy(gameObject);
    }

    #endregion ALL SELF DECLEAR FUNCTIONS

}
