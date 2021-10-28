using System;
using UnityEngine;
using com.alphapotato.utility;
using System.Collections;
using TMPro;

[DefaultExecutionOrder(ConstantManager.APManagerOrder)]
public class APManager : MonoBehaviour, IHierarchyIcon
{
    public static Action OnAddAPBehaviour;
    public static Action<APManager> OnSelfDistributionAction;
    public static Action<GameState> OnChangeGameState;
    public static Action OnNone;
    public static Action OnGameDataLoad;
    public static Action OnGameInitialize;
    public static Action OnGameStart;
    public static Action OnGameOver;
    public static Action OnCompleteTask;
    public static Action OnIncompleteTask;
    public static Action OnPauseGamePlay;

    public static Action<TappingType, Vector3> OnTap;
    public static Action<Vector3> OnDrag;
    public static Action<SwippingType> OnSwip;

    public GameplayData gameplayData = new GameplayData();
    public GameState gameState;
    public ParticleSystem gameOverEffect;
    public Animator gameStartingUI, gamePlayUI, gameSuccessUI, gameFaildUI, gameCustomUI;
    public TextMeshProUGUI levelText;
    public AudioClip gameWinAudioClip, gameLoseFailAudioClip;

    //[HideInInspector]
    public int totalGivenTask = 0, totalCompletedTask = 0, totalIncompleteTask = 0;
    [HideInInspector]
    public APTools APTools;
    public bool debugModeOn;

    GameState previousState;
    AudioSource runtimeAudioSource;

    private void OnEnable()
    {
        OnAddAPBehaviour += AddAPBehaviour;
    }

    private void OnDisable()
    {
        OnAddAPBehaviour -= AddAPBehaviour;
    }

    public virtual void Awake()
    {
        APTools = APTools.Instance;
        runtimeAudioSource = gameObject.AddComponent<AudioSource>();
        runtimeAudioSource.playOnAwake = false;
    }

    public virtual void Start()
    {
    }

    void AddAPBehaviour()
    {
        //Debug.LogError("New APBehaviour Added Successfully.");
        OnSelfDistributionAction?.Invoke(this);
    }

    public virtual void ProcessTapping(TappingType tappingType, Vector3 tapOnWorldSpace)
    {
        OnTap?.Invoke(tappingType, tapOnWorldSpace);
    }
    public virtual void ProcessDragging(Vector3 dragAmount)
    {
        OnDrag?.Invoke(dragAmount);
    }
    public virtual void ProcessSwipping(SwippingType swippingType)
    {
        OnSwip?.Invoke(swippingType);
    }

    public virtual void OnCompleteATask()
    {
        totalCompletedTask++;
        OnCompleteTask?.Invoke();

        if (totalCompletedTask.Equals(totalGivenTask))
        {
            gameplayData.isGameoverSuccess = true;
            gameplayData.gameEndTime = Time.time;

            ChangeGameState(GameState.GAME_PLAY_ENDED);
        }
    }

    public virtual void OnIncompleteATask()
    {
        totalIncompleteTask++;
        OnIncompleteTask?.Invoke();
    }

    public int GetModedLevelNumber()
    {
        return gameplayData.currentLevelNumber % ConstantManager.TOTAL_GAME_LEVELS;
    }

    public int GetValidLevelIndex()
    {
        int sceneIndex = GetModedLevelNumber() % (UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1);
        return (sceneIndex < 1 ? 0 : sceneIndex) + 1;
    }

    public int GetLevelMultiplayer()
    {
        return gameplayData.currentLevelNumber / ConstantManager.TOTAL_GAME_LEVELS;
    }

    public virtual void NextLevel()
    {
        APTools.sceneManager.LoadNextLevel();
    }

    public virtual void ReloadLevel()
    {
        APTools.sceneManager.ReLoadLevel();
    }

    public virtual void ChangeGameState(GameState gameState)
    {
        StartCoroutine(ChangeState(gameState));
    }

    IEnumerator ChangeState(GameState gameState)
    {
        yield return null;
        switch (gameState)
        {
            case GameState.NONE:
                None();
                break;
            case GameState.GAME_DATA_LOADED:
                GameDataLoad();
                break;
            case GameState.GAME_INITIALIZED:
                GameInitialize();
                break;
            case GameState.GAME_PLAY_STARTED:
                GameStart();
                break;
            case GameState.GAME_PLAY_ENDED:
                GameOver();
                break;
            case GameState.GAME_PLAY_PAUSED:
                PauseGamePlay();
                break;
            case GameState.GAME_PLAY_UNPAUSED:
                gameState = UnPauseGame(gameState);
                break;
        }

        Debug.Log("Executing: " + gameState.ToString());
        this.gameState = gameState;
        OnChangeGameState?.Invoke(gameState);
    }

    public virtual void None()
    {
        OnNone?.Invoke();
    }

    public virtual void GameDataLoad()
    {
        gameplayData = APTools.savefileManager.LoadGameData();
        OnSelfDistributionAction?.Invoke(this);
        OnGameDataLoad?.Invoke();
    }

    public virtual void GameInitialize()
    {
        OnGameInitialize?.Invoke();
    }

    public virtual void GameStart()
    {
        gameplayData.gameStartTime = Time.time;
        levelText.text = "Level - " + (gameplayData.currentLevelNumber + 1);
        OnGameStart?.Invoke();
    }

    public virtual void GameOver()
    {
        gameplayData.gameEndTime = Time.time;
        OnGameOver?.Invoke();

        if (gameplayData.isGameoverSuccess)
        {
            gameplayData.currentLevelNumber++;
            gameSuccessUI.SetBool("Hide", false);
            gameOverEffect.Play();
            PlayThisSoundEffect(gameWinAudioClip);
        }
        else
        {
            gameFaildUI.SetBool("Hide", false);
            PlayThisSoundEffect(gameLoseFailAudioClip);
        }

        SaveGame();
    }

    public virtual void PlayThisSoundEffect(AudioClip audioClip)
    {
        runtimeAudioSource.clip = audioClip;
        runtimeAudioSource.Play();
    }

    public virtual void SaveGame()
    {
        APTools.savefileManager.SaveGameData(gameplayData);
    }

    public virtual void PauseGamePlay()
    {
        previousState = gameState;
    }

    public virtual GameState UnPauseGame(GameState gameState)
    {
        gameState = previousState;
        return gameState;
    }

    public virtual void DistributeAPManager()
    {
        OnSelfDistributionAction?.Invoke(this);
    }

    public string EditorIconPath { get { return "APManagerIcon"; } }
}
