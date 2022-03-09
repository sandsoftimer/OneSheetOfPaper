using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameAnalyticsSDK;
using TMPro;
using UnityEngine;

public class MeshEraser : APBehaviour
{
    public TMP_InputField speedTextMesh;
    public TextMeshProUGUI levelText;
    public MeshUVPainter meshUVPainter = new MeshUVPainter();
    public GameObject foldingPrefab, tutorialHand, hintsButton;
    public MeshRenderer behindWhitePaper;
    public Texture2D behindPaperTexture;
    public Material outputMaterial;
    public Color paintColor;
    public float paintRadious, draggingThreshold = 0.25f;
    public float movingSpeed = 0.38f, foldingScalingSpeed = 30f;
    public float snappingDistance = 0.5f;
    [Range(1f, 10f)]
    public float snappingSpeed = 10f;
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D polygonCollider2D;
    public Transform inputChecker;
    public InputChecker2D inputChecker2D;

    [HideInInspector]
    public RollProcessor currentrRollProcessor;
    [HideInInspector]
    public Vector3 cuttingSize;
    [HideInInspector]
    public Vector2 rectengle;
    [HideInInspector]
    public FakeTearingLevelData currentLevelData;

    GameObject currentLevelPrefab;
    Texture2D outputTex;
    float bendValue = 0;
    bool dragging, firstTry;
    bool negetiveAreaCrossed;
    bool levelFailedAnnounced;

    public RaycastHit preHit;
    public RaycastHit currentRayCastHit;
    AnimationData currentAnimationData;
    TextureSequence currentTextureSequence;
    int currentTextureSequenceIndex;
    int currentImageIndex = -1, loopCount;
    bool snappedAlready, snappingDone, tutorialActive;

    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();

        tutorialHand.SetActive(false);
        //speedTextMesh.text = PlayerPrefs.GetFloat("RollSpeed", 4).ToString();
        //movingSpeed = float.Parse(speedTextMesh.text);

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
            DOTween.Kill(tutorialHand);
            tutorialHand.SetActive(false);
            tutorialActive = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if(currentrRollProcessor != null)
            {
                currentrRollProcessor = null;
            }    
        }

        if (Input.GetMouseButton(0))
        {
            if (currentrRollProcessor == null)
            {
                //movingSpeed = float.Parse(speedTextMesh.text);
                //PlayerPrefs.SetFloat("RollSpeed", movingSpeed);

                preHit = new RaycastHit();
                preHit.GetRaycastFromScreenTouch(1 << gameObject.layer);

                if (preHit.collider != null)
                {
                    dragging = true;
                    firstTry = true;
                    ActiveRollingAnimation();
                }
            }

            if (dragging)
            {
                currentRayCastHit = new RaycastHit();
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
                            currentrRollProcessor = Instantiate(foldingPrefab, currentRayCastHit.point, Quaternion.identity, transform).GetComponent<RollProcessor>();
                            currentrRollProcessor.gameObject.SetActive(false);
                            currentrRollProcessor.transform.localScale = new Vector3(cuttingSize.x, 0, 0);
                            currentrRollProcessor.gameObject.SetActive(true);
                            currentrRollProcessor.transform.rotation = Quaternion.LookRotation(currentRayCastHit.point - preHit.point, Vector3.up);
                            currentrRollProcessor.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.color = currentLevelData.backFaceColor;
                            currentrRollProcessor.Initializer(this);

                            firstTry = false;
                        }

                        currentrRollProcessor.Fetch(currentRayCastHit);
                    }
                    else
                    {
                        currentrRollProcessor = null;
                    }
                }
                else
                {
                    currentrRollProcessor = null;
                }
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

        //if (negetiveAreaCrossed)
        //    return;

        //for (int i = 0; i < currentLevelData.negetiveCheckingPoints.childCount; i++)
        //{
        //    RaycastHit hit;
        //    if (Physics.Raycast(currentLevelData.negetiveCheckingPoints.GetChild(i).position, Vector3.up, out hit, 150, 1 << ConstantManager.DESTINATION_LAYER))
        //    {
        //        currentLevelData.negetiveCheckingPoints.GetChild(i).localScale *= 3;
        //        Debug.LogError( "Index: " +i + " :=> "+ hit.collider.name);
        //        negetiveAreaCrossed = true;
        //        break;
        //    }
        //}
        //if (!negetiveAreaCrossed)
        //{
        //    bool positiveCheck = true;
        //    for (int i = 0; i < currentLevelData.positiveCheckingPoints.childCount; i++)
        //    {
        //        if (!Physics.Raycast(currentLevelData.positiveCheckingPoints.GetChild(i).position, Vector3.up, 150, 1 << ConstantManager.DESTINATION_LAYER))
        //        {
        //            positiveCheck = false;
        //            break;
        //        }
        //    }
        //    if (positiveCheck)
        //    {
        //        gameplayData.isGameoverSuccess = true;
        //        gameState = GameState.GAME_PLAY_ENDED;
        //        gameManager.ChangeGameState(GameState.GAME_PLAY_ENDED);
        //    }
        //}

    }

#endregion ALL UNITY FUNCTIONS
    //=================================   
#region ALL OVERRIDING FUNCTIONS

    public override void OnCheckLevelTearing()
    {
        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

        if (levelFailedAnnounced)
            return;

        base.OnCheckLevelTearing();

        bool positiveCheck = true;

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
                negetiveAreaCrossed = true;
                break;
            }
        }

        gameplayData.isGameoverSuccess = positiveCheck && !negetiveAreaCrossed;
        gameManager.ChangeGameState(GameState.GAME_PLAY_ENDED);
    }

    public override void OnGameInitializing()
    {
        base.OnGameInitializing();

        if (gameManager.debugModeOn)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            currentLevelPrefab = transform.GetChild(gameManager.GetModedLevelNumber()).gameObject;
        }
        else
            currentLevelPrefab = Instantiate(Resources.Load("Fake Tearing Levels Data/Level " + (gameManager.GetModedLevelNumber() + 1)) as GameObject, transform);
        currentLevelPrefab.SetActive(true);
        currentLevelData = currentLevelPrefab.GetComponent<FakeTearingLevelData>();

        levelText.text = currentLevelData.levelText;
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

    public void OnHintsButtonPress()
    {
#if AP_GAMEANALYTICS_SDK_INSTALLED
        GameAnalytics.NewDesignEvent("Hints_Level_" + (gameplayData.currentLevelNumber + 1));
#endif
        tutorialActive = true;
        hintsButton.SetActive(false);
        DOTween.Kill(tutorialHand);
        Transform hints = currentLevelPrefab.transform.GetChild(3);
        TransitTutorialHand(hints);
    }

    void TransitTutorialHand(Transform hints)
    {
        tutorialHand.transform.position = hints.GetChild(0).position;
        tutorialHand.SetActive(true);
        tutorialHand.transform.DOMove(hints.GetChild(1).position, ConstantManager.DEFAULT_ANIMATION_TIME).SetEase(Ease.Linear).OnComplete(
            () => {

                //tutorialHand.SetActive(false);
                if(tutorialActive)
                    TransitTutorialHand(hints);
            });
    }

    public Vector3 ClampVector(Vector3 value, Vector3 minVector, Vector3 maxVector)
    {
        value.x = Mathf.Clamp(value.x, minVector.x, maxVector.x);
        value.y = Mathf.Clamp(value.y, minVector.y, maxVector.y);
        value.z = Mathf.Clamp(value.z, minVector.z, maxVector.z);
        return value;
    }

    void CheckSnappingPoint()
    {
        if (levelFailedAnnounced)
            return;
        if (snappedAlready)
            return;

        if (Vector3.Distance(currentrRollProcessor.transform.position, currentLevelData.snappingPoint.position) <= snappingDistance &&
            (currentrRollProcessor.transform.localScale - cuttingSize).magnitude <= 0.5f)
        {
            snappedAlready = true;
            Physics.Raycast(new Ray(currentLevelData.snappingPoint.position.ModifyThisVector(0, 1, 0), Vector3.down), out currentRayCastHit, Mathf.Infinity, 1 << transform.gameObject.layer);
            currentrRollProcessor.transform.DOMove(currentLevelData.snappingPoint.position, 0.25f);
            currentrRollProcessor.transform.DOScale(cuttingSize, 0.25f);
        }
    }
    //void FallTheRollingPaper()
    //{
    //    if (currentrRollProcessor != null)
    //    {
    //        currentrRollProcessor.transform.GetChild(0).GetChild(0).GetComponent<MeshCollider>().enabled = false;
    //        GameObject go = new GameObject("A Detuched Roll");
    //        go.transform.position = currentrRollProcessor.transform.position;
    //        currentrRollProcessor.transform.parent = go.transform;
    //        go.transform.DOMove(
    //            go.transform.position.ModifyThisVector(0, 0, -20),
    //            ConstantManager.DEFAULT_ANIMATION_TIME).SetEase(Ease.InBack);
    //    }
    //    currentrRollProcessor = null;
    //    dragging = false;
    //    Debug.LogError("");
    //}

    void ThrowDelayFailed()
    {
        APTools.functionManager.ExecuteAfterSecond(0.5f, () => {

            gameplayData.isGameoverSuccess = false;
            gameState = GameState.GAME_PLAY_ENDED;
            gameManager.ChangeGameState(GameState.GAME_PLAY_ENDED);
        });
    }

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
