using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperTearPart : APBehaviour
{
    Transform dragStartPoint, dragEndPoint;

    GameObject tutorialHand;
    SkinnedMeshRenderer skin;
    bool draggingPaper, selectedPaper, taskCompleted;
    float blendMaximumValue = 100f, revertSpeed = 10f, blendWeight = 0;
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();

        dragStartPoint = transform.GetChild(0);
        dragEndPoint = transform.GetChild(1);

        skin = GetComponent<SkinnedMeshRenderer>();
        skin.SetBlendShapeWeight(0, 0);
        dragEndPoint.gameObject.SetActive(false);
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

        if (Input.GetMouseButtonUp(0))
        {
            draggingPaper = false;
            revertSpeed = 0.25f;
            dragEndPoint.gameObject.SetActive(false);
            CancelInvoke("AlterSprite");
        }

        if (selectedPaper && !taskCompleted)
        {
            if (!draggingPaper && selectedPaper)
            {
                revertSpeed += Time.deltaTime;
                blendWeight = Mathf.Clamp(blendWeight - revertSpeed, 0, blendMaximumValue);
                if (blendWeight <= 0)
                {
                    selectedPaper = false;
                    dragStartPoint.gameObject.SetActive(true);
                    dragEndPoint.gameObject.SetActive(false);
                    CancelInvoke("AlterSprite");
                    CreateHandTutorial();

                    if (gameManager.totalCompletedTask == 0)
                        transform.parent.GetComponent<PaperHolder>().ActiveDefaultAnimation();
                }
            }

            if (draggingPaper)
            {
                Vector3 a = dragStartPoint.position;
                a.y = 0;
                Vector3 b = dragEndPoint.position;
                b.y = 0;
                Vector3 c = APTools.mathManager.GetWorldTouchPosition(Vector3.up);
                c.y = 0;

                blendWeight = Mathf.Clamp(APTools.mathManager.InverseLerp(a, b, c) * 100, 0, blendMaximumValue);
                //blendWeight = Mathf.Lerp(0, 1 / Vector3.Distance(a, b), 1 / Vector3.Distance(c, b)) / 100;
                if (blendWeight > 95)
                {
                    blendWeight = 100;
                    gameManager.OnCompleteATask();
                    CancelInvoke("AlterSprite");
                    taskCompleted = true;
                    dragEndPoint.gameObject.SetActive(false);
                }
            }

            skin.SetBlendShapeWeight(0, blendWeight);
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

    public override void OnGameInitializing()
    {
        base.OnGameInitializing();

        gameManager.totalGivenTask++;

        bool iapDone = false;
        dragStartPoint.GetChild(0).GetComponent<SpriteRenderer>().enabled = iapDone;
        dragEndPoint.GetChild(0).GetComponent<SpriteRenderer>().enabled = iapDone;
        CreateHandTutorial();
    }

    void CreateHandTutorial()
    {
        if (gameplayData.currentLevelNumber < 50)
        {
            tutorialHand = Instantiate(Resources.Load("Tutorial_Hand") as GameObject);
            tutorialHand.transform.parent = dragStartPoint;
            Tutorial_Hand tutorial_Hand = tutorialHand.GetComponent<Tutorial_Hand>();
            tutorial_Hand.Initialize(dragEndPoint);
        }
    }

    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS

    public void SelectThisPart()
    {
        Destroy(tutorialHand);
        selectedPaper = true;
        draggingPaper = true;
        dragStartPoint.gameObject.SetActive(false);
        InvokeRepeating("AlterSprite", 0.2f, 0.2f);
    }

    void AlterSprite()
    {
        dragEndPoint.gameObject.SetActive(!dragEndPoint.gameObject.activeSelf);
    }

    #endregion ALL SELF DECLEAR FUNCTIONS

}
