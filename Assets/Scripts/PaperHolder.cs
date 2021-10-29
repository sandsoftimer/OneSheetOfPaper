using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperHolder : APBehaviour
{
    public Transform[] paperPieces;

    SkinnedMeshRenderer currentPaperPart;
    bool draggingPaper;
    float blendMaximumValue = 50f, revertSpeed = 10f;
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
        if (gameState.Equals(GameState.GAME_INITIALIZED) && Input.GetMouseButtonDown(0))
        {
            gameManager.ChangeGameState(GameState.GAME_PLAY_STARTED);
            gameState = GameState.GAME_PLAY_STARTED;
        }

        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();
            hit.GetRaycastFromScreenTouch(1 << ConstantManager.PAPER_DRAG_PART);

            if(hit.collider != null)
            {
                hit.collider.transform.parent.GetComponent<PaperTearPart>().SelectThisPart();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            draggingPaper = false;
        }

        if(!draggingPaper && currentPaperPart != null)
        {
            currentPaperPart.SetBlendShapeWeight(0, currentPaperPart.GetBlendShapeWeight(0) - Time.deltaTime * revertSpeed);
            if (currentPaperPart.GetBlendShapeWeight(0) <= 0)
            {
                currentPaperPart = null;
            }
        }
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
