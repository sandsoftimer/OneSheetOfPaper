using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RollProcessor : APBehaviour
{
    MeshEraser meshEraser;
    MeshUVPainter meshUVPainter;
    MeshRenderer behindWhitePaper;
    Texture2D behindPaperTexture;
    Material outputMaterial;
    Color paintColor;

    float paintRadious, draggingThreshold = 0.25f;
    float movingSpeed = 0.38f, foldingScalingSpeed = 30f;
    float snappingDistance = 0.5f;
    float snappingSpeed = 10f;
    SpriteRenderer spriteRenderer;
    PolygonCollider2D polygonCollider2D;
    Transform inputChecker;
    
    Texture2D outputTex;
    Vector3 cuttingSize;
    Vector2 rectengle;
    float bendValue = 0;
    bool dragging;
    bool negetiveAreaCrossed;
    bool levelFailedAnnounced;

    RaycastHit preHit;
    RaycastHit finalRayCasHit;
    FakeTearingLevelData currentLevelData;
    bool snappedAlready, snappingDone;

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
        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

        if (snappedAlready && !snappingDone)
        {
            transform.position = Vector3.Lerp(transform.position, currentLevelData.snappingPoint.position, snappingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    currentLevelData.snappingPoint.rotation,
                    snappingSpeed * snappingSpeed * Time.deltaTime);

            //bendValue += Time.deltaTime * foldingScalingSpeed + (currentRayCastHit.point - preHit.point).magnitude;
            //Physics.Raycast(new Ray(foldingObj.transform.position.ModifyThisVector(0, 1, 0), Vector3.down), out currentRayCastHit, 100, 1 << gameObject.layer);

            //UVPaintOutput uVPaintOutput = meshUVPainter.PaintOnUV(currentRayCastHit, preHit, paintColor, paintRadious, 10000000, rectengle);
            //outputTex = uVPaintOutput.texture;
            //outputMaterial.SetTexture("MaskInput", outputTex);
            //behindPaperTexture.SetPixels(uVPaintOutput.pixelBufferWithOffset);
            //behindPaperTexture.Apply();

            //bendValue = Mathf.Clamp01(bendValue);
            //foldingObj.transform.localScale = Vector3.Lerp(
            //        foldingObj.transform.localScale,
            //    cuttingSize, foldingScalingSpeed * Time.deltaTime);

            //preHit = currentRayCastHit;

            //Sprite sprite = Sprite.Create(outputTex, new Rect(0f, 0f, outputTex.width, outputTex.width), new Vector3(0.5f, 0.5f), 25f);
            //spriteRenderer.sprite = sprite;
            //Destroy(polygonCollider2D);
            //polygonCollider2D = spriteRenderer.gameObject.AddComponent<PolygonCollider2D>();
            if ((transform.position - currentLevelData.snappingPoint.position).magnitude < 0.75f)
            {
                snappingDone = true;
                gameplayData.isGameoverSuccess = true;
                gameManager.ChangeGameState(GameState.GAME_PLAY_ENDED);
            }
        }

        if (!snappedAlready)
        {
            if (dragging)
            {
                transform.position = Vector3.Lerp(transform.position, finalRayCasHit.point, movingSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(
                        transform.rotation,
                        Quaternion.LookRotation(finalRayCasHit.point - preHit.point, Vector3.up),
                        0.75f);

                RaycastHit lastRayCastHit;
                Physics.Raycast(new Ray(transform.position.ModifyThisVector(0, 1, 0), Vector3.down), out lastRayCastHit, 100, 1 << meshEraser.gameObject.layer);
                if (IsDraggingOnPaper(lastRayCastHit.point))
                {
                    if ((finalRayCasHit.point - preHit.point).magnitude > draggingThreshold)
                    {
                        RaycastHit inputCheckCasting = new RaycastHit();
                        Physics.Raycast(new Ray(transform.position.ModifyThisVector(0, 1, 0), Vector3.down), out inputCheckCasting, 100, 1 << ConstantManager.ENEMY_LAYER);

                        if (inputCheckCasting.collider != null)
                        {
                            negetiveAreaCrossed = true;
                        }

                        if (negetiveAreaCrossed && !levelFailedAnnounced)
                        {
                            levelFailedAnnounced = true;
                            ThrowDelayFailed();
                        }

                        //Physics.Raycast(new Ray(transform.position.ModifyThisVector(0, 1, 0), Vector3.down), out lastRayCastHit, 100, 1 << meshEraser.gameObject.layer);
                        bendValue = foldingScalingSpeed * (lastRayCastHit.point - preHit.point).magnitude;

                        UVPaintOutput uVPaintOutput = meshUVPainter.PaintOnUV(lastRayCastHit, preHit, paintColor, paintRadious, 10000000, rectengle);
                        outputTex = uVPaintOutput.texture;
                        outputMaterial.SetTexture("MaskInput", outputTex);
                        behindPaperTexture.SetPixels(uVPaintOutput.pixelBufferWithOffset);
                        behindPaperTexture.Apply();

                        Vector3 nextScale = ClampVector(transform.localScale + Vector3.one * bendValue, transform.localScale, cuttingSize);
                        transform.localScale = nextScale;

                        preHit = lastRayCastHit;

                        Sprite sprite = Sprite.Create(outputTex, new Rect(0f, 0f, outputTex.width, outputTex.width), new Vector3(0.5f, 0.5f), 25f);
                        spriteRenderer.sprite = sprite;
                        //polygonCollider2D = spriteRenderer.gameObject.GetComponent<PolygonCollider2D>();
                        //if(polygonCollider2D != null)
                        //    Destroy(polygonCollider2D);
                        //polygonCollider2D = spriteRenderer.gameObject.AddComponent<PolygonCollider2D>();
                        meshEraser.inputChecker2D.UpdateCollider();

                    }
                    CheckSnappingPoint();

                    if ((lastRayCastHit.point - finalRayCasHit.point).magnitude <= draggingThreshold)
                    {
                        if (meshEraser.currentrRollProcessor != null)
                        {
                            if (meshEraser.currentrRollProcessor != this)
                                FallTheRollingPaper();
                        }
                        else
                            FallTheRollingPaper();
                    }

                    if (lastRayCastHit.collider == null)
                    {
                        FallTheRollingPaper();
                    }
                }
                else
                {
                    FallTheRollingPaper();
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
        if (negetiveAreaCrossed)
            return;

        for (int i = 0; i < currentLevelData.negetiveCheckingPoints.childCount; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(currentLevelData.negetiveCheckingPoints.GetChild(i).position, Vector3.up, out hit, 150, 1 << ConstantManager.DESTINATION_LAYER))
            {
                currentLevelData.negetiveCheckingPoints.GetChild(i).localScale *= 3;
                //Debug.LogError("Index: " + i + " :=> " + hit.collider.name);
                negetiveAreaCrossed = true;
                break;
            }
        }
        if (!negetiveAreaCrossed)
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


    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS

    public void Fetch(RaycastHit currentRayCastHit)
    {
        this.finalRayCasHit = currentRayCastHit;
    }

    public void Initializer(MeshEraser meshEraser)
    {
        this.meshEraser = meshEraser;
        this.meshUVPainter = meshEraser.meshUVPainter;
        this.currentLevelData = meshEraser.currentLevelData;
        this.preHit = meshEraser.preHit;
        this.cuttingSize = meshEraser.cuttingSize;

        this.paintRadious = meshEraser.paintRadious;
        this.draggingThreshold = meshEraser.draggingThreshold;
        this.movingSpeed = meshEraser.movingSpeed;
        this.foldingScalingSpeed = meshEraser.foldingScalingSpeed;
        this.snappingDistance = meshEraser.snappingDistance;
        this.snappingSpeed = meshEraser.snappingSpeed;
        this.rectengle = meshEraser.rectengle;

        this.outputMaterial = meshEraser.outputMaterial;
        this.behindPaperTexture = meshEraser.behindPaperTexture;
        this.behindWhitePaper = meshEraser.behindWhitePaper;

        this.inputChecker = meshEraser.inputChecker;
        this.spriteRenderer = meshEraser.spriteRenderer;
        this.polygonCollider2D = meshEraser.polygonCollider2D;

        //this.spriteRenderer = meshEraser.spriteRenderer;
        //this.spriteRenderer = meshEraser.spriteRenderer;
        //this.spriteRenderer = meshEraser.spriteRenderer;


        dragging = true;
    }

    void ThrowDelayFailed()
    {
        APTools.functionManager.ExecuteAfterSecond(0.5f, () => {

            gameplayData.isGameoverSuccess = false;
            gameState = GameState.GAME_PLAY_ENDED;
            gameManager.ChangeGameState(GameState.GAME_PLAY_ENDED);
        });
    }

    Vector3 ClampVector(Vector3 value, Vector3 minVector, Vector3 maxVector)
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

        if (Vector3.Distance(transform.position, meshEraser.currentLevelData.snappingPoint.position) <= snappingDistance &&
            (transform.localScale - cuttingSize).magnitude <= 0.5f)
        {
            snappedAlready = true;
            Physics.Raycast(new Ray(currentLevelData.snappingPoint.position.ModifyThisVector(0, 1, 0), Vector3.down), out finalRayCasHit, Mathf.Infinity, 1 << transform.gameObject.layer);
            transform.DOMove(currentLevelData.snappingPoint.position, 0.25f);
            transform.DOScale(cuttingSize, 0.25f);
        }
    }

    public void FallTheRollingPaper()
    {
        transform.GetChild(0).GetChild(0).GetComponent<MeshCollider>().enabled = false;
        GameObject go = new GameObject("A Detuched Roll");
        go.transform.position = transform.position;
        transform.parent = go.transform;
        go.transform.DOMove(
            go.transform.position.ModifyThisVector(0, 0, -20),
            ConstantManager.DEFAULT_ANIMATION_TIME).SetEase(Ease.InBack);
        dragging = false;
    }

    bool IsDraggingOnPaper(Vector3 position)
    {
        position.y = position.z;
        position.z = -10;
        meshEraser.inputChecker.transform.localPosition = position;
        RaycastHit2D hit = Physics2D.Raycast(inputChecker.transform.position, inputChecker.transform.forward, 100, 1 << ConstantManager.SENSOR);

        return hit.collider != null;
    }
    #endregion ALL SELF DECLEAR FUNCTIONS

}
