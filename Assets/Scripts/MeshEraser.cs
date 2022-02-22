using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshEraser : APBehaviour
{
    public MeshUVPainter meshUVPainter = new MeshUVPainter();
    public GameObject foldingObj;
    public SkinnedMeshRenderer foldinMeshFilter;
    public Material outputMaterial;
    public Color paintColor;
    public float paintRadious, draggingThreshold = 0.25f;
    public Vector2 rectengle;

    Texture2D outputTex;
    float bendValue = 0;
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
        }

        if (Input.GetMouseButton(0))
        {
            RaycastHit raycastHit = new RaycastHit();
            raycastHit.GetRaycastFromScreenTouch();
            if (raycastHit.collider != null)
            {
                if ((raycastHit.point - preHit.point).magnitude > draggingThreshold)
                {
                    bendValue += Time.deltaTime * 20;
                    foldingObj.transform.position = raycastHit.point;
                    //foldingObj.transform.position = Vector3.Lerp(
                    //    transform.position,
                    //    raycastHit.point,
                    //    0.95f);
                    foldingObj.transform.rotation = Quaternion.Lerp(
                        foldingObj.transform.rotation,
                        Quaternion.LookRotation(raycastHit.point - previousPosition, Vector3.up),
                        Time.deltaTime * 5);
                    //foldingObj.transform.localEulerAngles = new Vector3(0,(previousPosition - raycastHit.point).y - 90, 0);
                    //Vector3 look = new Vector3(0, (previousPosition - raycastHit.point).y - 90, 0);
                    //foldingObj.transform.localEulerAngles = previousPosition - raycastHit.point;
                    outputTex = meshUVPainter.PaintOnUV(raycastHit, preHit, paintColor, paintRadious, 1000, rectengle);
                    outputMaterial.SetTexture("MaskInput", outputTex);

                    bendValue = Mathf.Clamp(bendValue, 0, 100);
                    foldinMeshFilter.SetBlendShapeWeight(0, bendValue);
                    preHit = raycastHit;
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
        previousPosition = foldingObj.transform.position;
    }

    #endregion ALL UNITY FUNCTIONS
    //=================================   
    #region ALL OVERRIDING FUNCTIONS
    
    
    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS
    
    
    #endregion ALL SELF DECLEAR FUNCTIONS

}
