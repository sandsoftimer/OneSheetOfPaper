using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshEraser : APBehaviour
{
    public MeshUVPainter meshUVPainter = new MeshUVPainter();
    public GameObject foldingPrefab;
    public Material outputMaterial;
    public Color paintColor;
    public float paintRadious, draggingThreshold = 0.25f, foldingSpeed = 50;
    public float movingSpeed = 0.38f, foldingScalingSpeed = 30f;
    public Vector2 rectengle;
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D polygonCollider2D;
    public Transform inputChecker;

    GameObject foldingObj;
    SkinnedMeshRenderer foldinMesh;
    Texture2D outputTex;
    float bendValue = 0;
    bool dragging, firstTry;
    RaycastHit preHit;
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();
        outputMaterial.SetTexture("MaskInput",
                outputMaterial.GetTexture("Texture"));
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

        //if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
        //    return;

        if (Input.GetMouseButtonDown(0))
        {
            preHit = new RaycastHit();
            preHit.GetRaycastFromScreenTouch();
            dragging = true;
            firstTry = true;
        }

        if (Input.GetMouseButton(0) && dragging)
        {
            RaycastHit currentRayCastHit = new RaycastHit();
            currentRayCastHit.GetRaycastFromScreenTouch();
            if (DraggingOnPaper(currentRayCastHit.point))
            {
                if (currentRayCastHit.collider != null)
                {
                    if (firstTry)
                    {
                        meshUVPainter = new MeshUVPainter();
                        meshUVPainter.fl = 0;
                        meshUVPainter.count = 0;
                        bendValue = 0;
                        foldingScalingSpeed = 0;
                        foldingObj = Instantiate(foldingPrefab, transform);
                        foldingObj.transform.position = currentRayCastHit.point;
                        foldinMesh = foldingObj.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
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
                        bendValue += Time.deltaTime * foldingScalingSpeed;
                        Physics.Raycast(new Ray(foldingObj.transform.position.ModifyThisVector(0, 1, 0), Vector3.down), out currentRayCastHit);


                        outputTex = meshUVPainter.PaintOnUV(currentRayCastHit, preHit, paintColor, paintRadious, 10000000, rectengle);
                        outputMaterial.SetTexture("MaskInput", outputTex);

                        bendValue = Mathf.Clamp01(bendValue);
                        //foldinMesh.SetBlendShapeWeight(0, bendValue);
                        foldingObj.transform.localScale = Vector3.Lerp(
                                foldingObj.transform.localScale,
                            new Vector3(1, 1, 1), foldingScalingSpeed * Time.deltaTime);

                        foldingScalingSpeed += Time.deltaTime;

                        preHit = currentRayCastHit;

                        Sprite sprite = Sprite.Create(outputTex, new Rect(0f, 0f, outputTex.width, outputTex.width), new Vector3(0.5f, 0.5f), 25f);
                        spriteRenderer.sprite = sprite;
                        Destroy(polygonCollider2D);
                        polygonCollider2D = spriteRenderer.gameObject.AddComponent<PolygonCollider2D>();
                        
                    }
                }
            }
        }

    }

    void FixedUpdate()
    {
        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

    }

    Vector3 previousPosition;
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
