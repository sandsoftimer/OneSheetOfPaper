using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PotatoSDK
{
    public class QuickLinkButton : MonoBehaviour
    {
        [SerializeField] Button button;
        [TextArea]
        public string link = "";

        private void Start()
        {
            Init();
        }
        void Init()
        {
            if (button == null || string.IsNullOrEmpty(link))
            {
                Debug.LogError("Please set link properly!");
            }
            else
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    Application.OpenURL(link);
                });
            }
        }
        public void SetLink(string newLink)
        {
            link = newLink;
            button = GetComponent<Button>();
            Text linkText = this.transform.GetComponentInChildren<Text>();
            linkText.text = link;
            Vector2 sd = (this.transform as RectTransform).sizeDelta;
            sd.y = link.Length >= 65 ? 95 : 65;
            (this.transform as RectTransform).sizeDelta = sd;
            string[] parts = link.Split('.');
            this.gameObject.name = parts[1];
            Init();
        }

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(QuickLinkButton))]
    public class QuickLinkButton_Editor : Editor
    {
        public string temp_link;
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField (serializedObject.FindProperty("link"));
            EditorGUI.EndDisabledGroup();
            temp_link = EditorGUILayout.TextField(new GUIContent("New Link"), temp_link);
            if (!string.IsNullOrEmpty(temp_link))
            {
                if (GUILayout.Button("Set New Link"))
                {
                    (target as QuickLinkButton).SetLink(temp_link);
                    temp_link = "";
                }
            }

        }
    }
#endif

}
