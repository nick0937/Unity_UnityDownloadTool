using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace yoyohan.PoolMgrDemo
{
    public class Test : MonoBehaviour
    {
        public bool isCreatPool = false;

        /*
         第一步：SetPoolPrefab（不管是否该GameObject继承于PoolObject）
         第二步 使用：GetOneGoByName或者GetOneGoByType<>
         第三步 销毁：1.调用该继承于PoolObject的父类方法OnRestore 或者2.调用PoolManager.instance.RestoreOneGoByName("Pool_Sphere", gameObject);
        */

        void Start()
        {
            if (isCreatPool)
            {
                PoolMgr.instance.CreatPool(Resources.Load<GameObject>("Cube"), true);
                PoolMgr.instance.CreatPool(Resources.Load<GameObject>("Sphere"), true);
            }
            StartCoroutine(GenerateCube());
        }

        IEnumerator GenerateCube()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                Debug.Log("GetOne!");
                GameObject go = PoolMgr.instance.GetPool("Pool_Sphere").GetOneGo();
                GameObject go2 = PoolMgr.instance.GetPool<EveryCube>().GetOneGo();
            }
        }


        void Update()
        {
            //测试切换场景是否清空或销毁对象池
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SceneManager.LoadScene(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}