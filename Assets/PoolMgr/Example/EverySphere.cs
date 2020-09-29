using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace yoyohan.PoolMgrDemo
{
    public class EverySphere : MonoBehaviour
    {
        void OnEnable()
        {
            Debug.Log("0.5s发出Restore请求!");
            Invoke("Restore", 0.5f);
        }

        void Restore()
        {
            PoolMgr.instance.GetPool("Pool_Sphere").RestoreOneGo(gameObject);
        }

    }
}