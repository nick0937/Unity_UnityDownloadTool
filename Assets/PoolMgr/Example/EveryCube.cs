using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace yoyohan.PoolMgrDemo
{
    public class EveryCube : PoolObject
    {
        void OnEnable()
        {
            Debug.Log("0.5s发出Restore请求!");
            Invoke("Restore", 0.5f);//调用父类的OnRestore方法
        }

    }
}
