/*
 * Developer Name: Md. Imran Hossain
 * E-mail: sandsoftimer@gmail.com
 * FB: https://www.facebook.com/md.imran.hossain.902
 * in: https://www.linkedin.com/in/md-imran-hossain-69768826/
 * 
 * Features:
 * Scene FadeIn-Out Transition
 * Loading Next level
 * Reloading Current Level
 * Get level Index  
 */


using UnityEngine.SceneManagement;
using UnityEngine;

namespace Com.AlphaPotato.Utility
{
    [RequireComponent(typeof(Animator))]
    public class SceneManager : MonoBehaviour
    {
        bool isBusy;
        Animator sceneFadeanimator;
        Animator SceneFadeanimator
        {
            get
            {
                if (sceneFadeanimator == null)
                    sceneFadeanimator = GetComponent<Animator>();
                return sceneFadeanimator;
            }
        }
        string levelToLoadByName;
        int levelToLoadByIndex;

        LoadSceneType loadLevelType;

        public void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLoadCallback;
        }

        public void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLoadCallback;
        }

        void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
        {
            //Debug.LogError("Fade calling");
            SceneFadeanimator.SetTrigger("FadeOut");
        }

        public void OnFadeOutComplete()
        {
            isBusy = false;
        }

        public void OnFadeInComplete()
        {
            switch (loadLevelType)
            {
                case LoadSceneType.LOAD_BY_NAME:

                    UnityEngine.SceneManagement.SceneManager.LoadScene(levelToLoadByName);
                    break;
                case LoadSceneType.LOAD_BY_INDEX:
                    //Debug.LogError("From Complete: " + levelToLoadByIndex);
                    UnityEngine.SceneManagement.SceneManager.LoadScene(levelToLoadByIndex);
                    break;
                default:
                    break;
            }
        }

        public void LoadLevel(string levelName)
        {
            isBusy = true;
            levelToLoadByName = levelName;
            loadLevelType = LoadSceneType.LOAD_BY_NAME;
            SceneFadeanimator.SetTrigger("FadeIn");
        }

        public void LoadLevel(int levelIndex)
        {
            isBusy = true;
            levelToLoadByIndex = levelIndex;
            loadLevelType = LoadSceneType.LOAD_BY_INDEX;
            SceneFadeanimator.SetTrigger("FadeIn");
        }

        // This will re-load current level;
        public void ReLoadLevel()
        {
            if (isBusy)
                return;

            LoadLevel(GetLevelIndex());
        }

        // This will load next index scene
        // If not exist the it will open auto First scene of BuildIndex.
        public void LoadNextLevel()
        {
            if (isBusy)
                return;

            int loadedIndex = GetLevelIndex() + 1;

            //Debug.LogError("Count: "+ UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings);
            //if (loadedIndex <= ConstantManager.TOTAL_GAME_LEVELS)
            if (loadedIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings
                && loadedIndex <= ConstantManager.TOTAL_GAME_LEVELS)
                LoadLevel(loadedIndex);
            else
                LoadLevel(1); // We skipping Boot Scene (This is not a gameplay level)
        }

        public int GetLevelIndex()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.R))
                ReLoadLevel();
#endif
        }
    }
}
