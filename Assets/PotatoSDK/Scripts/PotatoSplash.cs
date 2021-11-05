using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PotatoSDK
{
    public class PotatoSplash : MonoBehaviour
    {
        public static PotatoSplash Instance { get; private set; }
        public Image progressImage;
        public void SetProgress(float progress)
        {
            if (progressImage) progressImage.fillAmount = progress;
        }
        private void Awake()
        {
            Instance = this;
        }
    }
}
