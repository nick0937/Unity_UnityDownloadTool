using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QP.Framework;
using System.IO;
using Newtonsoft.Json;

/// <summary>
/// 描述：
/// 功能：
/// 作者：yoyohan
/// 创建时间：2019-05-13 10:59:56
/// </summary>
public class UnityDownloadManager
{
    private readonly static UnityDownloadManager _instance = new UnityDownloadManager();
    public static UnityDownloadManager instance
    {
        get
        {
            return _instance;
        }
    }

    public const int TIMEOUT = 1000;


    #region 定义的私有变量
    private Pool _unityDownloadPool = null;
    private Pool unityDownloadPool
    {
        get
        {
            if (_unityDownloadPool == null)
            {
                GameObject go = new GameObject("UnityDownload");
                go.AddComponent<UnityWeb>();
                _unityDownloadPool = PoolManager.instance.CreatPool(go, true, "UnityDownloadPool");
                go.transform.SetParent(_unityDownloadPool.transform);
            }
            return _unityDownloadPool;
        }
    }
    private const string allDownloadKey = "AllDownload";
    private List<DownloadObj> lisDownloadObj;


    public Pool GetUnityDownloadPool()
    {
        return unityDownloadPool;
    }

    public DownloadObj GetDownloadObjByID(int id)
    {
        for (int i = 0; i < lisDownloadObj.Count; i++)
        {
            if (lisDownloadObj[i].id == id)
            {
                return lisDownloadObj[i];
            }
        }
        return null;
    }

    private void AddDownloadObj(DownloadObj downloadingDetail)
    {
        DownloadObj temp = GetDownloadObjByID(downloadingDetail.id);
        if (temp == null)
        {
            lisDownloadObj.Add(downloadingDetail);
        }
        else
        {
            temp = downloadingDetail;
        }
        this.UpdateAllDownload();
    }

    /// <summary>
    /// 下载完成时保存到本地 ，开始下载时保存到本地
    /// </summary>
    public void UpdateAllDownload()
    {
        Debug.Log(JsonConvert.SerializeObject(lisDownloadObj));
        PlayerPrefs.SetString(allDownloadKey, JsonConvert.SerializeObject(lisDownloadObj));
        PlayerPrefs.Save();
    }

    public List<DownloadObj> GetAllDownloadObj()
    {
        return lisDownloadObj;
    }

    private void RemoveDownloadObj(DownloadObj downloadObj)
    {
        DownloadObj temp = GetDownloadObjByID(downloadObj.id);
        if (temp == null)
            return;

        lisDownloadObj.Remove(downloadObj);
        if (downloadObj.currentDownloadState == DownloadState.downloading)
        {
            downloadObj.webDownload.Close();
        }
        this.UpdateAllDownload();
    }
    #endregion


    /// <summary>
    /// 开启下载器 初始化下载器
    /// </summary>
    public void StartDownloadManager()
    {
        Debug.Log("下载文件工具类开始启动！");

        lisDownloadObj = new List<DownloadObj>();
        string defaultStr = JsonConvert.SerializeObject(new List<DownloadObj>());

        lisDownloadObj = JsonConvert.DeserializeObject<List<DownloadObj>>(PlayerPrefs.GetString(allDownloadKey, defaultStr)) ;
        
        for (int i = 0; i < lisDownloadObj.Count; i++)
        {
            if (lisDownloadObj[i].currentDownloadState != DownloadState.downloaded)
            {
                if (File.Exists(lisDownloadObj[i].parentPath + ".tmp"))
                {
                    using (FileStream _fileStream = new FileStream(lisDownloadObj[i].parentPath + ".tmp", FileMode.Open, FileAccess.Read))
                    {
                        lisDownloadObj[i].sDownloadFileResult.downloadedLength = (int)_fileStream.Length;
                    }
                }

                if (lisDownloadObj[i].currentDownloadState == DownloadState.downloading)
                {
                    //程序初始时把正在下载的开始下载
                    //包含异常崩溃 重新下载
                    Debug.Log("程序启动时下载 id:" + lisDownloadObj[i].id + " url" + lisDownloadObj[i].url + " localurl:" + lisDownloadObj[i].parentPath + " downloadLength" + lisDownloadObj[i].sDownloadFileResult.downloadedLengthStr);
                    StartDownloadOne(lisDownloadObj[i]);
                }
            }

        }

    }


    public void StartDownloadOne(DownloadObj downloadObj)
    {
        UnityWeb unityWeb = unityDownloadPool.GetOneGo().GetComponent<UnityWeb>();
        downloadObj.SetWebDownload(unityWeb);
        if (downloadObj.sDownloadFileResult.contentLength == 0)
        {
            downloadObj.OnGetFileSizeAction = (int code, DownloadObj obj) =>
            {
                Debug.Log("获取下载的文件大小成功！code：" + code + " size:" + obj.sDownloadFileResult.contentLengthStr);
                if (code == 200 || code == 0)
                {
                    this.AddDownloadObj(downloadObj);
                    unityWeb.DownloadFile(obj);
                }
            };
            unityWeb.DownloadFileSize(downloadObj);

        }
        else
        {
            Debug.Log("有下载过，直接开始下载，已下载:" + downloadObj.sDownloadFileResult.downloadedLengthStr);
            unityWeb.DownloadFile(downloadObj);
        }

    }


    public void DeleteOneDownload(DownloadObj downloadObj, bool isDeleteLoaclFile)
    {
        RemoveDownloadObj(downloadObj);
        if (isDeleteLoaclFile == true)
        {
            if (File.Exists(downloadObj.parentPath))
                File.Delete(downloadObj.parentPath);

            if (File.Exists(downloadObj.parentPath + ".tmp"))
                File.Delete(downloadObj.parentPath + ".tmp");
        }
    }

}
