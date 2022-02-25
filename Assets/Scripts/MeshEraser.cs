using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshEraser : APBehaviour
{
    public MeshUVPainter meshUVPainter = new MeshUVPainter();
    public GameObject foldingPrefab;
    public MeshRenderer behindWhitePaper;
    public Texture2D behindPaperTexture;
    public Material outputMaterial;
    public Color paintColor;
    public float paintRadious, draggingThreshold = 0.25f;
    public float movingSpeed = 0.38f, foldingScalingSpeed = 30f;
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D polygonCollider2D;
    public Transform inputChecker;

    GameObject foldingObj;
    Texture2D outputTex;
    Vector3 cuttingSize;
    Vector2 rectengle;
    float bendValue = 0;
    bool dragging, firstTry;
    bool negetiveAreaCrossed;
    bool levelFailedAnnounced;
    RaycastHit preHit;
    FakeTearingLevelData currentLevelData;

    AnimationData currentAnimationData;
    TextureSequence currentTextureSequence;
    int currentTextureSequenceIndex;
    int currentImageIndex = -1, loopCount;
    float currentFps;

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
        behindPaperTexture = Instantiate(behindWhitePaper.material.mainTexture as Texture2D);
        behindWhitePaper.material.SetTexture("_BaseMap", behindPaperTexture);
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
                ActiveRollingAnimation();
            }
        }

        if (Input.GetMouseButton(0) && dragging)
        {
            RaycastHit currentRayCastHit = new RaycastHit();
            currentRayCastHit.GetRaycastFromScreenTouch(1 << gameObject.layer);
            if (IsDraggingOnPaper(currentRayCastHit.point))
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
                        foldingObj = Instantiate(foldingPrefab, currentRayCastHit.point, Quaternion.identity, transform);
                        foldingObj.transform.localScale = new Vector3(cuttingSize.x, 0, 0);
                        foldingObj.transform.rotation = Quaternion.LookRotation(currentRayCastHit.point - preHit.point, Vector3.up);
                        foldingObj.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.color = currentLevelData.backFaceColor;
                        firstTry = false;
                    }

                    foldingObj.transform.position = Vector3.Lerp(foldingObj.transform.position, currentRayCastHit.point, movingSpeed * Time.deltaTime);
                    foldingObj.transform.rotation = Quaternion.Lerp(
                            foldingObj.transform.rotation,
                            Quaternion.LookRotation(currentRayCastHit.point - preHit.point, Vector3.up),
                            0.75f);
                    //foldingObj.transform.GetChild(0).eulerAngles += Vector3.right * Time.deltaTime;

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

                        UVPaintOutput uVPaintOutput = meshUVPainter.PaintOnUV(currentRayCastHit, preHit, paintColor, paintRadious, 10000000, rectengle);
                        outputTex = uVPaintOutput.texture;
                        outputMaterial.SetTexture("MaskInput", outputTex);
                        behindPaperTexture.SetPixels(uVPaintOutput.pixelBufferWithOffset);
                        behindPaperTexture.Apply();

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

        bool negetiveCheck = false;
        for (int i = 0; i < currentLevelData.negetiveCheckingPoints.childCount; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(currentLevelData.negetiveCheckingPoints.GetChild(i).position, Vector3.up, out hit, 150, 1 << ConstantManager.DESTINATION_LAYER))
            {
                Debug.LogError(i + " : "+ hit.collider.name);
                negetiveCheck = true;
                break;
            }
        }
        if (negetiveCheck)
        {
            gameplayData.isGameoverSuccess = false;
            gameState = GameState.GAME_PLAY_ENDED;
            gameManager.ChangeGameState(GameState.GAME_PLAY_ENDED);
        }
        else
        {
            bool positiveCheck = true;
            for (int i = 0; i < currentLevelData.positiveCheckingPoints.childCount; i++)
            {
                if (!Physics.Raycast(currentLevelData.positiveCheckingPoints.GetChild(i).position, Vector3.up, 150, 1 << ConstantManager.DESTINATION_LAYER))
                {
                    positiveCheck = false;
                    break;
                }
            }
            if (positiveCheck)
            {
                gameplayData.isGameoverSuccess = true;
                gameState = GameState.GAME_PLAY_ENDED;
                gameManager.ChangeGameState(GameState.GAME_PLAY_ENDED);
            }
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

        GameObject currentLevel = Instantiate(Resources.Load("Fake Tearing Levels Data/Level " + (gameManager.GetModedLevelNumber() + 1)) as GameObject, transform);
        currentLevel.SetActive(true);
        currentLevelData = currentLevel.GetComponent<FakeTearingLevelData>();
        cuttingSize = currentLevelData.cuttingSize;
        outputMaterial.SetTexture("MaskInput", currentLevelData.defaultDatas.textureSequences[0].textures[0]);
        outputMaterial.SetTexture("Texture", currentLevelData.defaultDatas.textureSequences[0].textures[0]);
        meshUVPainter.InitializeNewMesh(transform.GetComponent<Renderer>().material);

        rectengle = new Vector2(cuttingSize.x * 120, cuttingSize.z * 100);
        ActiveDefaultAnimation();

    }

    public override void OnGameOver()
    {
        base.OnGameOver();

        if (gameplayData.isGameoverSuccess)
        {
            ActiveCompleteAnimation();
        }
    }

    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS


    public void ActiveDefaultAnimation()
    {
        currentTextureSequenceIndex = -1;
        currentAnimationData = currentLevelData.defaultDatas;
        NextTextureSequence();
    }

    public void ActiveRollingAnimation()
    {
        currentTextureSequenceIndex = -1;
        currentAnimationData = currentLevelData.rollingDatas;
        NextTextureSequence();
    }

    public void ActiveCompleteAnimation()
    {
        currentTextureSequenceIndex = -1;
        currentAnimationData = currentLevelData.levelSuccessDatas;
        NextTextureSequence();
    }

    void UpdateNextTexture()
    {
        currentImageIndex++;
        if (currentTextureSequence.loopCount == 0)
        {
            // Loopping sequence
            currentImageIndex %= currentTextureSequence.textures.Length;
        }
        else if (loopCount > 0 && currentImageIndex == currentTextureSequence.textures.Length)
        {
            --loopCount;
            if (loopCount <= 0)
            {
                CancelInvoke("UpdateNextTexture");
                NextTextureSequence();
                return;
            }
            currentImageIndex %= currentTextureSequence.textures.Length;
        }
        //paperMaterial.SetTexture("_BaseMap", currentTextureSequence.textures[currentImageIndex]);
        outputMaterial.SetTexture("Texture", currentTextureSequence.textures[currentImageIndex]);

        if (currentImageIndex > currentTextureSequence.textures.Length)
        {
            NextTextureSequence();
        }
    }

    void NextTextureSequence()
    {
        currentTextureSequenceIndex++;
        if (currentTextureSequenceIndex >= currentAnimationData.textureSequences.Length)
            return;

        //Debug.LogError(currentTextureSequenceIndex + " == " + currentAnimationData.textureSequences.Length);
        currentTextureSequence = currentAnimationData.textureSequences[currentTextureSequenceIndex];
        loopCount = currentTextureSequence.loopCount;

        currentImageIndex = -1;
        CancelInvoke("UpdateNextTexture");
        InvokeRepeating("UpdateNextTexture", 0, 1f / currentTextureSequence.fps);
    }

    bool IsDraggingOnPaper(Vector3 position)
    {
        position.y = position.z;
        position.z = -10;
        inputChecker.transform.localPosition = position;
        RaycastHit2D hit = Physics2D.Raycast(inputChecker.transform.position, inputChecker.transform.forward, 100, 1 << ConstantManager.SENSOR);

        return hit.collider != null;
    }

#endregion ALL SELF DECLEAR FUNCTIONS

}
