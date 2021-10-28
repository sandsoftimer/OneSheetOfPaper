using UnityEngine;
using com.alphapotato.utility;
using System;

[DefaultExecutionOrder(ConstantManager.APBehaviourOrder)]
public class APBehaviour : MonoBehaviour, IHierarchyIcon
{
    [HideInInspector]
    public APManager gameManager;
    [HideInInspector]
    public APTools APTools;

    [HideInInspector]
    public GameplayData gameplayData;
    //[HideInInspector]
    public GameState gameState;

    bool registeredForInput;

    public virtual void Awake()
    {
        APTools = APTools.Instance;
    }

    public virtual void Start()
    {
    }

    public virtual void OnEnable()
    {
        APManager.OnSelfDistributionAction += OnSelfDistributionAction;
        APManager.OnNone += OnNone;
        APManager.OnGameDataLoad += OnGameDataLoad;
        APManager.OnGameInitialize += OnGameInitializing;
        APManager.OnGameStart += OnGameStart;
        APManager.OnGameOver += OnGameOver;
        APManager.OnChangeGameState += OnChangeGameState;
        APManager.OnCompleteTask += OnCompleteTask;
        APManager.OnIncompleteTask += OnIncompleteTask;

        if (registeredForInput)
        {
            APManager.OnTap += ProccessInputTapping;
            APManager.OnDrag += OnDrag;
            APManager.OnSwip += ProcessInputSwipping;
        }

        APManager.OnAddAPBehaviour?.Invoke();

    }

    public virtual void OnDisable()
    {
        APManager.OnSelfDistributionAction -= OnSelfDistributionAction;
        APManager.OnNone -= OnNone;
        APManager.OnGameDataLoad -= OnGameDataLoad;
        APManager.OnGameInitialize -= OnGameInitializing;
        APManager.OnGameStart -= OnGameStart;
        APManager.OnGameOver -= OnGameOver;
        APManager.OnChangeGameState -= OnChangeGameState;
        APManager.OnCompleteTask -= OnCompleteTask;
        APManager.OnIncompleteTask -= OnIncompleteTask;

        if (registeredForInput)
        {
            APManager.OnTap -= ProccessInputTapping;
            APManager.OnDrag -= OnDrag;
            APManager.OnSwip -= ProcessInputSwipping;
        }

    }

    public virtual void OnStartFire()
    {
    }

    public virtual void OnStopFire()
    {
    }

    public void Registar_For_Input_Callback()
    {
        registeredForInput = true;
    }

    void ProccessInputTapping(TappingType inputType, Vector3 tapOnWorldSpace)
    {
        switch (inputType)
        {
            case TappingType.NONE:
                break;
            case TappingType.TAP_START:
                OnTapStart(tapOnWorldSpace);
                break;
            case TappingType.TAP_END:
                OnTapEnd(tapOnWorldSpace);
                break;
            case TappingType.TAP_N_HOLD:
                OnTapAndHold(tapOnWorldSpace);
                break;
        }
    }

    public virtual void OnTapStart(Vector3 tapOnWorldSpace)
    {        
    }

    public virtual void OnTapAndHold(Vector3 tapOnWorldSpace)
    {
    }

    public virtual void OnTapEnd(Vector3 tapOnWorldSpace)
    {
    }

    public virtual void OnDrag(Vector3 dragAmount)
    {
    }

    void ProcessInputSwipping(SwippingType swippingType)
    {
        switch (swippingType)
        {
            case SwippingType.SWIPE_UP:
                OnSwipeUp();
                break;
            case SwippingType.SWIPE_DOWN:
                OnSwipeDown();
                break;
            case SwippingType.SWIPE_LEFT:
                OnSwipeLeft();
                break;
            case SwippingType.SWIPE_RIGHT:
                OnSwipeRight();
                break;
        }
    }

    public virtual void OnSwipeUp()
    {
    }

    public virtual void OnSwipeDown()
    {
    }

    public virtual void OnSwipeLeft()
    {
    }

    public virtual void OnSwipeRight()
    {
    }

    public virtual void OnSelfDistributionAction(APManager aPManager)
    {
        gameManager = aPManager;
        gameplayData = aPManager.gameplayData;
        gameState = aPManager.gameState;
    }

    public virtual void OnChangeGameState(GameState gameState)
    {
        this.gameState = gameState;
    }

    public virtual void OnIncompleteTask()
    {
    }

    public virtual void OnNone()
    {
    }

    public virtual void OnGameDataLoad()
    {
    }

    public virtual void OnGameInitializing()
    {        
    }

    public virtual void OnGameStart()
    {        
    }

    public virtual void OnGameOver()
    {
    }

    public virtual void OnCompleteTask()
    {
    }

    public string EditorIconPath { get { return "APBehaviourIcon"; } }
}
