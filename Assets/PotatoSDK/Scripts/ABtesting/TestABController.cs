using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PotatoSDK
{
    public class TestABController : MonoBehaviour
    {
        public GameObject root;
        public GameObject modulePrefab;
        public Transform spawnRoot;

        public Button applyButton;

        System.Action onTestApply;
        public void Init(System.Action onTestApply, List<ABKeep> keeps)
        {
            this.onTestApply = onTestApply;

            root.SetActive(true);

            foreach (var item in keeps)
            {
                GameObject modG = Instantiate(modulePrefab);
                modG.transform.SetParent(spawnRoot);
                modG.transform.localScale = Vector3.one;
                TestABDataChangeUI uiitem = modG.GetComponent<TestABDataChangeUI>();
                uiitem.Load(item);
                modules.Add(uiitem);
            }
            applyButton.onClick.AddListener(OnApply);
            if (modules.Count == 0)
            {
                root.SetActive(false);
                OnApply();
            }
        }

        public List<TestABDataChangeUI> modules;

        void OnApply()
        {
            foreach (var item in modules)
            {
                if (item.loaded) item.OnApply();
            }
            root.SetActive(false);
            onTestApply?.Invoke();
            onTestApply = null;
        }

    }
}