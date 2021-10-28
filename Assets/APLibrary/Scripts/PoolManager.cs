/*
 * Developer E-mail: sandsoftimer@gmail.com
 * Facebook Account: https://www.facebook.com/md.imran.hossain.902
 * 
 * Features:
 * Object Pooling
 * Object Pushing
 * Resetting Manager
 * Pre Instantiate pooling objects 
 */

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace com.alphapotato.utility
{
    public class PoolManager : MonoBehaviour
    {
        Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

        public GameObject Instantiate(GameObject prefabObj)
        {
            // Finally this object will be return
            GameObject obj;

            // Make sure type is not null
            CheckTypeExist(prefabObj.tag);

            // If don't have any item yet then create one & return
            if (poolDictionary[prefabObj.tag].Count == 0)
            {
                obj = GameObject.Instantiate(prefabObj);
                return obj;
            }

            // Finally pool an object & return;
            obj = poolDictionary[prefabObj.tag].Dequeue();
            obj.transform.parent = null;
            obj.SetActive(true);
            return obj;
        }

        public void Destroy(GameObject obj, float killTime = 0)
        {
            if (killTime == 0) Destroy(obj);
            else StartCoroutine(DelayDestroy(obj, killTime));
        }

        IEnumerator DelayDestroy(GameObject obj, float killTime)
        {
            yield return new WaitForSeconds(killTime);

            Destroy(obj);

            //if (obj == null)
            //{
            //    Debug.Log("A null object can't be stored in pooler.");
            //    yield return null;
            //}

            //// Make sure type is not null
            //CheckTypeExist(obj.tag);

            //if (obj == null)
            //    yield break;

            //obj.SetActive(false);
            //obj.transform.parent = transform;
            //poolDictionary[obj.tag].Enqueue(obj);
        }

        private void Destroy(GameObject obj)
        {
            if (obj == null)
            {
                Debug.Log("A null object can't be stored in pooler.");
                return;
            }

            // Make sure type is not null
            CheckTypeExist(obj.tag);

            if (obj == null)
                return;

            obj.SetActive(false);
            obj.transform.parent = transform;
            poolDictionary[obj.tag].Enqueue(obj);
        }

        void CheckTypeExist(string tagName)
        {
            // If this prefab type is not in dictionary,
            // then make a type by it's tag name.
            if (!poolDictionary.ContainsKey(tagName))
                poolDictionary[tagName] = new Queue<GameObject>();
        }

        public void ResetPoolManager()
        {
            //Transform[] children = gameObject.GetComponentsInChildren(typeof(Transform), true) as Transform[];

            for (int i = transform.childCount - 1; i > -1; i--)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }

            //foreach (Transform item in transform)
            //{
            //    Destroy(item.gameObject);
            //}

            poolDictionary = new Dictionary<string, Queue<GameObject>>();
        }

        public void PrePopulateItem(GameObject obj, int howMany)
        {
            // Make sure type is not null
            CheckTypeExist(obj.tag);

            for (int i = 0; i < howMany; i++)
            {
                Destroy(GameObject.Instantiate(obj));
            }
        }

    }
}