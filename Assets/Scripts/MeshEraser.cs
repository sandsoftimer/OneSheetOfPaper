using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshEraser : APBehaviour
{
    public MeshUVPainter meshUVPainter = new MeshUVPainter();
    public GameObject foldingPrefab;
    public Material outputMaterial;
    public Color paintColor;
    public float paintRadious, draggingThreshold = 0.25f;
    public float movingSpeed = 0.38f, foldingScalingSpeed = 30f;
    public Vector2 rectengle;
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D polygonCollider2D;
    public Transform inputChecker;

    GameObject foldingObj;
    Texture2D outputTex;
    Vector3 cuttingSize;
    float bendValue = 0;
    bool dragging, firstTry;
    bool negetiveAreaCrossed;
    bool levelFailedAnnounced;
    RaycastHit preHit;
    FakeTearingLevelData currentLevelData;
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();
        //outputMaterial.SetTexture("MaskInput",
        //        outputMaterial.GetTexture("Texture"));
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

        if (Input.GetMouseButtonDown(0))
        {
            preHit = new RaycastHit();
            preHit.GetRaycastFromScreenTouch(1 << gameObject.layer);

            if (preHit.collider != null)
            {
                dragging = true;
                firstTry = true;
            }
        }

        if (Input.GetMouseButton(0) && dragging)
        {
            RaycastHit currentRayCastHit = new RaycastHit();
            currentRayCastHit.GetRaycastFromScreenTouch(1 << gameObject.layer);
            if (DraggingOnPaper(currentRayCastHit.point))
            {
                if (currentRayCastHit.collider != null)
                {
                    if (firstTry)
                    {
                        //meshUVPainter = new MeshUVPainter();
                        meshUVPainter.fl = 0;
                        meshUVPainter.count = 0;
                        bendValue = 0;
                        //foldingScalingSpeed = 0;
                        foldingObj = Instantiate(foldingPrefab, transform);
                        foldingObj.transform.position = currentRayCastHit.point;
                        foldingObj.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", currentLevelData.levelTexture);
                        foldingObj.transform.localScale = new Vector3(1, 0, 0);
                        firstTry = false;
                    }

                    foldingObj.transform.position = Vector3.Lerp(foldingObj.transform.position, currentRayCastHit.point, movingSpeed * Time.deltaTime);
                    //foldingObj.transform.LookAt(currentRayCastHit.point);
                    foldingObj.transform.rotation = Quaternion.Lerp(
                            foldingObj.transform.rotation,
                            Quaternion.LookRotation(currentRayCastHit.point - preHit.point, Vector3.up),
                            0.75f);

                    if ((currentRayCastHit.point - preHit.point).magnitude > draggingThreshold)
                    {
                        RaycastHit inputCheckCasting = new RaycastHit();
                        Physics.Raycast(new Ray(foldingObj.transform.position.ModifyThisVector(0, 1, 0), Vector3.down), out inputCheckCasting, 100, 1 << ConstantManager.ENEMY_LAYER);

                        if (inputCheckCasting.collider != null)
                        {
                            negetiveAreaCrossed = true;
                        }

                        if (negetiveAreaCrossed && !levelFailedAnnounced)
                        {
                            levelFailedAnnounced = true;
                            APTools.functionManager.ExecuteAfterSecond(0.5f, ()=> {

                                gameplayData.isGameoverSuccess = false;
                                gameState = GameState.GAME_PLAY_ENDED;
                                gameManager.ChangeGameState(GameState.GAME_PLAY_ENDED);
                            });
                        }

                        bendValue += Time.deltaTime * foldingScalingSpeed + (currentRayCastHit.point - preHit.point).magnitude;
                        Physics.Raycast(new Ray(foldingObj.transform.position.ModifyThisVector(0, 1, 0), Vector3.down), out currentRayCastHit, 100, 1 << gameObject.layer);

                        outputTex = meshUVPainter.PaintOnUV(currentRayCastHit, preHit, paintColor, paintRadious, 10000000, rectengle);
                        outputMaterial.SetTexture("MaskInput", outputTex);

                        bendValue = Mathf.Clamp01(bendValue);
                        //foldinMesh.SetBlendShapeWeight(0, bendValue);
                        foldingObj.transform.localScale = Vector3.Lerp(
                                foldingObj.transform.localScale,
                            cuttingSize, foldingScalingSpeed * Time.deltaTime);

                        //foldingScalingSpeed += Time.deltaTime;

                        preHit = currentRayCastHit;

                        Sprite sprite = Sprite.Create(outputTex, new Rect(0f, 0f, outputTex.width, outputTex.width), new Vector3(0.5f, 0.5f), 25f);
                        spriteRenderer.sprite = sprite;
                        Destroy(polygonCollider2D);
                        polygonCollider2D = spriteRenderer.gameObject.AddComponent<PolygonCollider2D>();
                        
                    }
                }
            }
            else
            {
                dragging = false;
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

        bool positiveCheck = true;
        bool negetiveCheck = false;

        for (int i = 0; i < currentLevelData.positiveCheckingPoints.childCount; i++)
        {
            if (!Physics.Raycast(currentLevelData.positiveCheckingPoints.GetChild(i).position, Vector3.up, 150, 1 << ConstantManager.DESTINATION_LAYER))
            {
                positiveCheck = false;
                break;
            }
        }
        for (int i = 0; i < currentLevelData.negetiveCheckingPoints.childCount; i++)
        {
            if (Physics.Raycast(currentLevelData.negetiveCheckingPoints.GetChild(i).position, Vector3.up, 150, 1 << ConstantManager.DESTINATION_LAYER))
            {
                negetiveCheck = true;
                break;
            }
        }

        positiveCheck = positiveCheck && !negetiveCheck && !negetiveAreaCrossed;
        if (positiveCheck)
        {
            gameplayData.isGameoverSuccess = true;
            gameState = GameState.GAME_PLAY_ENDED;
            gameManager.ChangeGameState(GameState.GAME_PLAY_ENDED);
        }
    }

#endregion ALL UNITY FUNCTIONS
    //=================================   
#region ALL OVERRIDING FUNCTIONS

    public override void OnCheckLevelTearing()
    {
        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

        base.OnCheckLevelTearing();

        bool positiveCheck = true;
        bool negetiveCheck = false;

        for (int i = 0; i < currentLevelData.positiveCheckingPoints.childCount; i++)
        {
            if (!Physics.Raycast(currentLevelData.positiveCheckingPoints.GetChild(i).position, Vector3.up, 150, 1 << ConstantManager.DESTINATION_LAYER))
            {
                positiveCheck = false;
                break;
            }
        }
        for (int i = 0; i < currentLevelData.negetiveCheckingPoints.childCount; i++)
        {
            if (Physics.Raycast(currentLevelData.negetiveCheckingPoints.GetChild(i).position, Vector3.up, 150, 1 << ConstantManager.DESTINATION_LAYER))
            {
                negetiveCheck = true;
                break;
            }
        }

        gameplayData.isGameoverSuccess = positiveCheck && !negetiveCheck && !negetiveAreaCrossed;
        gameManager.ChangeGameState(GameState.GAME_PLAY_ENDED);
    }

    public override void OnGameInitializing()
    {
        base.OnGameInitializing();

        GameObject currentLevel = Instantiate(Resources.Load("Fake Tearing Levels Data/Level " + (gameManager.GetModedLevelNumber() + 1)) as GameObject);
        currentLevel.transform.parent = transform;
        currentLevelData = currentLevel.GetComponent<FakeTearingLevelData>();
        cuttingSize = currentLevelData.cuttingSize;
        outputMaterial.SetTexture("MaskInput", currentLevelData.levelTexture);
        outputMaterial.SetTexture("Texture", currentLevelData.levelTexture);

    }

#endregion ALL OVERRIDING FUNCTIONS
    //=================================
#region ALL SELF DECLEAR FUNCTIONS

    bool DraggingOnPaper(Vector3 position)
    {
        position.y = position.z;
        position.z = -10;
        inputChecker.transform.localPosition = position;
        RaycastHit2D hit = Physics2D.Raycast(inputChecker.transform.position, inputChecker.transform.forward, 100, 1 << ConstantManager.SENSOR);

        return hit.collider != null;
    }

#endregion ALL SELF DECLEAR FUNCTIONS

}
