using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace yoyohan
{
    public class Pool : MonoBehaviour
    {
        public string poolName;
        public GameObject prefab;
        public GameObject mGameObject { get { if (_mGameObject == null) _mGameObject = this.gameObject; return _mGameObject; } }
        public Transform mTransform { get { if (_mTransform == null) _mTransform = this.transform; return _mTransform; } }


        private bool isDontDestroyOnLoad = false;
        private bool isInheritPoolObject = false;
        private int maxCapacity = -1;//-1为不限制
        private Transform workGo;
        private Transform idleGo;
        private GameObject _mGameObject;
        private Transform _mTransform;
        private List<GameObject> lisWork = new List<GameObject>();
        private List<GameObject> lisIdle = new List<GameObject>();


        public void InitPool(string poolName, GameObject prefab)
        {
            this.poolName = poolName;
            this.prefab = prefab;

            workGo = new GameObject("work").transform;
            idleGo = new GameObject("idle").transform;
            workGo.SetParent(mTransform);
            idleGo.SetParent(mTransform);
            idleGo.gameObject.SetActive(false);
        }

        public void StartPool()
        {
            workGo = new GameObject("work").transform;
            idleGo = new GameObject("idle").transform;
            workGo.SetParent(mTransform);
            idleGo.SetParent(mTransform);
            idleGo.gameObject.SetActive(false);

            if (isDontDestroyOnLoad)
                DontDestroyOnLoad(mGameObject);
        }


        public Pool SetPoolName(string poolName)
        {
            this.poolName = poolName;
            return this;
        }

        public Pool SetPoolPrefab(GameObject poolPrefab)
        {
            this.prefab = poolPrefab;
            return this;
        }

        public Pool SetDontDestroyOnLoad(bool isDontDestroyOnLoad)
        {
            this.isDontDestroyOnLoad = isDontDestroyOnLoad;
            return this;
        }

        public Pool SetInheritPoolObject(bool isInheritPoolObject)
        {
            this.isInheritPoolObject = isInheritPoolObject;
            return this;
        }

        public Pool SetPoolMaxCapacity(int maxCapacity)
        {
            this.maxCapacity = maxCapacity;
            return this;
        }


        public GameObject GetOneGo()
        {
            if (maxCapacity != -1 && lisWork.Count >= maxCapacity)
                return null;

            GameObject returnGo = null;
            if (lisIdle.Count <= 0)
            {
                returnGo = Instantiate(prefab);
            }
            else
            {
                returnGo = lisIdle[0];
                lisIdle.RemoveAt(0);
            }
            returnGo.transform.SetParent(workGo, false);
            lisWork.Add(returnGo);

            //如果继承PoolObject，调用OnUse
            if (isInheritPoolObject)
                returnGo.GetComponent<PoolObject>().OnUse();

            return returnGo;
        }


        public void RestoreOneGo(GameObject go)
        {
            lisIdle.Add(go);
            lisWork.Remove(go);
            go.transform.SetParent(idleGo, false);

            //如果继承PoolObject，调用OnRestore
            if (isInheritPoolObject)
                go.GetComponent<PoolObject>().OnRestore();
        }

        /// <summary>
        /// 可主动调用销毁Pool
        /// </summary>
        public void DestroyThisPool()
        {
            Destroy(this.mGameObject);
        }

        /// <summary>
        /// 被动调用销毁Pool
        /// </summary>
        void OnDestroy()
        {
            PoolMgr.instance.RemovePoolByPoolName(poolName);
        }

    }
}