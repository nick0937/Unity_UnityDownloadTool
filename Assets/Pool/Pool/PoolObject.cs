using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    [Header("Pool对象池属性")]
    public string poolName;
    public Pool pool;

    private GameObject _mGameObject;
    private Transform _mTransform;
    public GameObject mGameObject { get { if (_mGameObject == null)_mGameObject = this.gameObject; return _mGameObject; } }
    public Transform mTransform { get { if (_mTransform == null)_mTransform = this.transform; return _mTransform; } }

    /// <summary>
    /// 当该PoolObject打开时
    /// </summary>
    public virtual void OnUse()
    {

    }

    /// <summary>
    /// 当该PoolObject收回时
    /// </summary>
    public virtual void OnRestore()
    {

    }

    /// <summary>
    /// Restore方法
    /// </summary>
    public virtual void Restore()
    {
        pool.RestoreOneGo(mGameObject);
        Debug.Log("OnRestore成功!");
    }

}
