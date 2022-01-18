using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PaperHolder : APBehaviour
{
    public string levelText;
    //public Transform cameraPoint;
    //public float cameraFieldOfView = 29f;
    public Color backgroundColor = Color.white;
    public AnimationData defaultDatas, rollingDatas, levelSuccessDatas;
    public string texturePath;

    AnimationData currentAnimationData;
    TextureSequence currentTextureSequence;
    Animator anim;

    Material paperMaterial;
    int currentTextureSequenceIndex;
    int currentImageIndex = -1, loopCount;
    float currentFps;
    int totalTearParts;
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();

        //if (cameraPoint != null)
        //{
        //    Camera.main.transform.position = cameraPoint.position;
        //    Camera.main.transform.rotation = cameraPoint.rotation;
        //}
        //Camera.main.orthographicSize = cameraFieldOfView;

        //Camera.main.backgroundColor = "#F1F1F1";
        paperMaterial = Resources.Load("PaperMaterial") as Material;
        anim = GetComponent<Animator>();
        currentAnimationData = null;
        currentTextureSequence = null;
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
            RaycastHit hit = new RaycastHit();
            hit.GetRaycastFromScreenTouch(1 << ConstantManager.PAPER_DRAG_PART);

            if(hit.collider != null)
            {
                hit.collider.transform.parent.GetComponent<PaperTearPart>().SelectThisPart();
                ActiveRollingAnimation();
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

    public override void OnPaperPartReset()
    {
        base.OnPaperPartReset();

        gameManager.totalGivenTask++;

        if(gameManager.totalGivenTask == totalTearParts)
        {
            
        }
    }

    public override void OnGameStart()
    {
        base.OnGameStart();
        totalTearParts = gameManager.totalGivenTask;
    }

    public override void OnGameInitializing()
    {
        base.OnGameInitializing();

        ((GameManager)gameManager).levelInfoText.text = levelText;
        ActiveDefaultAnimation();
        DistributeMaterial();
    }

    public override void OnGameOver()
    {
        base.OnGameOver();

        ActiveCompleteAnimation();
        anim.SetTrigger("Execute");
    }

    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS

    void DistributeMaterial()
    {
        //paperMaterial.SetTexture("_BaseMap", currentTextureSequence.textures[0]);
        Transform[] childList = GetComponentsInChildren<Transform>(false);
        for (int i = 0; i < childList.Length; i++)
        {
            Renderer _renderer = childList[i].GetComponent<Renderer>();
            if (_renderer != null && !childList[i].gameObject.layer.Equals(ConstantManager.PAPER_DRAG_PART))
            {
                _renderer.material = paperMaterial;
            }
        }
    }

    public void ActiveDefaultAnimation()
    {
        currentTextureSequenceIndex = -1;
        currentAnimationData = defaultDatas;
        NextTextureSequence();
    }

    public void ActiveRollingAnimation()
    {
        currentTextureSequenceIndex = -1;
        currentAnimationData = rollingDatas;
        NextTextureSequence();
    }

    public void ActiveCompleteAnimation()
    {
        currentTextureSequenceIndex = -1;
        currentAnimationData = levelSuccessDatas;
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
        else if(loopCount > 0 && currentImageIndex == currentTextureSequence.textures.Length)
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
        paperMaterial.SetTexture("_BaseMap", currentTextureSequence.textures[currentImageIndex]);

        if(currentImageIndex > currentTextureSequence.textures.Length)
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

    #endregion ALL SELF DECLEAR FUNCTIONS

}
