/*
 * Developer E-mail: sandsoftimer@gmail.com
 * Facebook Account: https://www.facebook.com/md.imran.hossain.902
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
            SceneFadeanimator.SetTrigger("FadeOut");
        }

        public void FadeOutComplete()
        {
            switch (loadLevelType)
            {
                case LoadSceneType.LOAD_BY_NAME:

                    UnityEngine.SceneManagement.SceneManager.LoadScene(levelToLoadByName);
                    break;
                case LoadSceneType.LOAD_BY_INDEX:

                    UnityEngine.SceneManagement.SceneManager.LoadScene(levelToLoadByIndex);
                    break;
                default:
                    break;
            }

        }

        public void LoadLevel(string levelName)
        {

            levelToLoadByName = levelName;
            loadLevelType = LoadSceneType.LOAD_BY_NAME;
            SceneFadeanimator.SetTrigger("FadeIn");
        }

        public void LoadLevel(int levelIndex)
        {

            levelToLoadByIndex = levelIndex;
            loadLevelType = LoadSceneType.LOAD_BY_INDEX;
            SceneFadeanimator.SetTrigger("FadeIn");
        }

        // This will re-load current level;
        public void ReLoadLevel()
        {
            LoadLevel(GetLevelIndex());
        }

        // This will load next index scene
        // If not exist the it will open auto First scene of BuildIndex.
        public void LoadNextLevel()
        {
            int loadedIndex = GetLevelIndex() + 1;

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
            if(Input.GetKeyDown(KeyCode.R))
                ReLoadLevel();
#endif
        }
    }
}
