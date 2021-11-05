/*
 * Developer Name: Md. Imran Hossain
 * E-mail: sandsoftimer@gmail.com
 * FB: https://www.facebook.com/md.imran.hossain.902
 * in: https://www.linkedin.com/in/md-imran-hossain-69768826/
 * 
 * This is a manager which will give common modular supports. 
 * like, 
 * Making a TextMeshPro(with text values) speach effect etc.
 *  
 */

using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Com.AlphaPotato.Utility
{
    public class FunctionManager : MonoBehaviour
    {
        public void ToFloat(ref float value, float destinationValue)
        {
            value = 0.5f;
        }

        //IEnumerator TuneFloat(ref float value, float destinationValue)
        //{
        //    yield return null;
        //}

        public void ExecuteAfterSecond(float time, Action action = null)
        {
            StartCoroutine(WaitForSecond(time, action)); 
        }

        IEnumerator WaitForSecond(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }

        public void SpeakThisTextMeshValue(TextMeshProUGUI textMeshProUGUI, float perWordDelay, AudioSource audioSource = null, Action action = null)
        {
            StartCoroutine(SpeakThisText(textMeshProUGUI, perWordDelay, audioSource, action));
        }

        IEnumerator SpeakThisText(TextMeshProUGUI textMeshProUGUI, float perWordDelay, AudioSource audioSource, Action action = null)
        {
            string value = textMeshProUGUI.text;
            textMeshProUGUI.text = string.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                textMeshProUGUI.text += value[i];
                audioSource.Play();
                yield return new WaitForSeconds(perWordDelay);
                if(value[i].Equals(' '))
                    yield return new WaitForSeconds(perWordDelay * 2);
            }
            action?.Invoke();
        }
    }
}
