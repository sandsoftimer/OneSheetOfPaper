using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperTearPart : APBehaviour
{
    Transform dragStartPoint, dragEndPoint;

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
                }
            }

            if (draggingPaper)
            {
                Vector3 a = Camera.main.WorldToScreenPoint(dragStartPoint.position);
                a.y = 0;
                Vector3 b = Camera.main.WorldToScreenPoint(dragEndPoint.position);
                b.y = 0;
                Vector3 c = Input.mousePosition;

                blendWeight = Mathf.Clamp(InverseLerp(a, b, c) * 100, 0, blendMaximumValue);
                //blendWeight = blendMaximumValue - Mathf.Clamp(Vector3.Distance(a,b), 0, blendMaximumValue) - Mathf.Clamp(Vector3.Distance(b, c), 0, blendMaximumValue);
                if (blendWeight > 95)
                {
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
    }

    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS

    public float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }

    public void SelectThisPart()
    {
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
