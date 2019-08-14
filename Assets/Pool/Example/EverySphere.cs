using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EverySphere : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log("0.5s发出Restore请求!");
        Invoke("Restore", 0.5f);
    }

    void Restore()
    {
        PoolManager.instance.GetPool("Pool_Sphere").RestoreOneGo(gameObject);
    }

}
