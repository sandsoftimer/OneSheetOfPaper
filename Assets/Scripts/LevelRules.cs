using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRules : APBehaviour
{
    [Header("Level Data")]
    public Texture2D topTexture;
    public Texture2D bottomTexture;
    public float cuttingSize = 1;
    public float foldingAngle = 15;
    public Transform[] positiveCheckingPoints, negetiveCheckingPoints;

    Material tearPartMaterial;
    OneSheetPeelMan oneSheetPeelMan;
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

    public void Initialize(OneSheetPeelMan oneSheetPeelMan)
    {
        this.oneSheetPeelMan = oneSheetPeelMan;
        tearPartMaterial = oneSheetPeelMan.tearPartMaterial;
        tearPartMaterial.SetTexture("TopTexture", topTexture);

        bottomTexture = bottomTexture == null ? new Texture2D(1, 1) : bottomTexture;
        tearPartMaterial.SetTexture("BottomTexture", bottomTexture);

        oneSheetPeelMan.cuttingSize = cuttingSize;
        oneSheetPeelMan.foldingAngle = foldingAngle;
    }
    
    #endregion ALL SELF DECLEAR FUNCTIONS

}
