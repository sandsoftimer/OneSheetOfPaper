using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PaperHolder : APBehaviour
{
    public Material paperMaterial;
    public float fps = 60;
    public Texture2D[] defaultTextures, rollingTextures, levelCompleteTextures;

    SkinnedMeshRenderer currentPaperPart;
    Texture2D[] currentActiveSet;
    Animator anim;

    int currentImageIndex = -1;
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();

        anim = GetComponent<Animator>();
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

    public override void OnGameInitializing()
    {
        base.OnGameInitializing();

        DistributeMaterial();
        ActiveDefaultAnimation();
        InvokeRepeating("UpdateNextTexture", 1f / fps, 1f / fps);
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
        Transform[] childList = GetComponentsInChildren<Transform>();
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
        currentImageIndex = -1;
        currentActiveSet = defaultTextures;
    }

    public void ActiveRollingAnimation()
    {
        currentImageIndex = -1;
        currentActiveSet = rollingTextures;
    }

    public void ActiveCompleteAnimation()
    {
        currentImageIndex = -1;
        currentActiveSet = levelCompleteTextures;
    }

    void UpdateNextTexture()
    {
        currentImageIndex++;
        currentImageIndex %= currentActiveSet.Length;
        paperMaterial.SetTexture("_BaseMap", currentActiveSet[currentImageIndex]);
    }

    #endregion ALL SELF DECLEAR FUNCTIONS

}
