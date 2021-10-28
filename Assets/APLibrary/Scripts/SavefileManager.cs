/*
 * Developer E-mail: sandsoftimer@gmail.com
 * Facebook Account: https://www.facebook.com/md.imran.hossain.902
 * 
 * Features: 
 * Saving gameplay data
 * Loading gameplay data  
 */

using UnityEngine;

namespace com.alphapotato.utility
{
    public class SavefileManager : MonoBehaviour
    {
        public void SaveGameData(GameplayData gameplayData)
        {
            PlayerPrefsX.SetBool(ConstantManager.isGameoverSuccess, gameplayData.isGameoverSuccess);

            PlayerPrefs.SetInt(ConstantManager.gameScore, gameplayData.gameScore);
            PlayerPrefs.SetInt(ConstantManager.currentLevelNumber, gameplayData.currentLevelNumber);

            PlayerPrefs.SetFloat(ConstantManager.gameStartTime, gameplayData.gameStartTime);
            PlayerPrefs.SetFloat(ConstantManager.gameEndTime, gameplayData.gameEndTime);
            PlayerPrefs.SetFloat(ConstantManager.totalLevelCompletedTime, gameplayData.totalLevelCompletedTime);
        }

        public GameplayData LoadGameData()
        {
            GameplayData gameplayData = new GameplayData();

            gameplayData.isGameoverSuccess = PlayerPrefsX.GetBool(ConstantManager.isGameoverSuccess, false);

            gameplayData.gameScore = PlayerPrefs.GetInt(ConstantManager.gameScore, 0);
            gameplayData.currentLevelNumber = PlayerPrefs.GetInt(ConstantManager.currentLevelNumber, 0);

            gameplayData.gameStartTime = PlayerPrefs.GetFloat(ConstantManager.gameStartTime, 0f);
            gameplayData.gameEndTime = PlayerPrefs.GetFloat(ConstantManager.gameEndTime, 0f);
            gameplayData.totalLevelCompletedTime = PlayerPrefs.GetFloat(ConstantManager.totalLevelCompletedTime, 0f);

            return gameplayData;
        }
    }
}