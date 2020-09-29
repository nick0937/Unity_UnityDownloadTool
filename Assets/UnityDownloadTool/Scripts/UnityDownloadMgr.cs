using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

/// <summary>
/// 描述：
/// 功能：
/// 作者：yoyohan
/// 创建时间：2019-05-13 10:59:56
/// </summary>

namespace yoyohan
{
    public class UnityDownloadMgr
    {
        private static UnityDownloadMgr _instance;
        public static UnityDownloadMgr instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UnityDownloadMgr();
                }
                return _instance;
            }
        }

        public const int TIMEOUT = 1000;

        public Action<DownloadObj> OnDownloadingAction;
        public Action<DownloadObj> OnDownloadedAction;
        public Action<DownloadObj> OnGetFileSizeAction;

        /// <summary>
        /// 触发下载事件 0GetFileSize 1Downloading 2Downloaded
        /// </summary>
        public void FireAction(int id, DownloadObj downloadObj)
        {
            if (id == 0 && OnGetFileSizeAction != null)
            {
                OnGetFileSizeAction(downloadObj);
            }
            else if (id == 1 && OnDownloadingAction != null)
            {
                OnDownloadingAction(downloadObj);
            }
            else if (id == 2 && OnDownloadedAction != null)
            {
                OnDownloadedAction(downloadObj);
            }
        }

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
                    _unityDownloadPool = PoolMgr.instance.CreatPool(go, true, "UnityDownloadPool");
                    go.transform.SetParent(_unityDownloadPool.transform);
                    go.SetActive(false);
                }
                return _unityDownloadPool;
            }
        }

        private List<DownloadObj> lisDownloadObj;


        #region 文件操作
        private string _DownloadConfigPath;
        private string DownloadConfigPath
        {
            get
            {
                if (string.IsNullOrEmpty(_DownloadConfigPath))
                {
                    _DownloadConfigPath = Path.Combine(Application.persistentDataPath, "DownloadConfig.json");
                }
                return _DownloadConfigPath;
            }
            set
            {
                _DownloadConfigPath = value;
            }
        }

        public void SetDownloadConfigPath(string path)
        {
            DownloadConfigPath = path;
        }

        private string ReadDownloadConfig()
        {
            if (!File.Exists(DownloadConfigPath))
            {
                WriteDownloadConfig("[]");
                return "[]";
            }
            return File.ReadAllText(DownloadConfigPath);
        }

        private void WriteDownloadConfig(string str)
        {
            if (Directory.Exists(DownloadConfigPath) == false)
            {
                DirectoryInfo pathInfo = new DirectoryInfo(DownloadConfigPath);
                pathInfo.Parent.Create();
            }
            File.WriteAllText(DownloadConfigPath, str);
        }
        #endregion


        public Pool GetUnityDownloadPool()
        {
            return unityDownloadPool;
        }

        public DownloadObj GetDownloadObjByID(string id)
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

        public void AddDownloadObj(DownloadObj downloadObj)
        {
            bool isFind = false;
            //必须这样赋值 才生效
            for (int i = 0; i < lisDownloadObj.Count; i++)
            {
                if (lisDownloadObj[i].id == downloadObj.id)
                {
                    lisDownloadObj[i] = downloadObj;
                    isFind = true;
                }
            }

            if (isFind == false)
            {
                lisDownloadObj.Add(downloadObj);
            }

            this.UpdateAllDownload();
        }

        /// <summary>
        /// 下载完成时保存到本地 ，开始下载时保存到本地
        /// </summary>
        public void UpdateAllDownload()
        {
            string str = "[]";

            if (lisDownloadObj.Count > 0)
            {
                JsonData jsonData = new JsonData();
                for (int i = 0; i < lisDownloadObj.Count; i++)
                {
                    jsonData.AddToIList(lisDownloadObj[i].ToJsonData());
                }
                str = jsonData.ToJson();
            }
            Debug.Log("更新下载历史存本地:" + str);
            WriteDownloadConfig(str);
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

            if (downloadObj.currentDownloadState == DownloadState.downloading)
            {
                downloadObj.webDownload.CloseDownLoad();
            }

            lisDownloadObj.Remove(downloadObj);
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

            foreach (JsonData item in JsonMapper.ToObject(ReadDownloadConfig()))
            {
                lisDownloadObj.Add(new DownloadObj(item));
            }

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
                        //Debug.Log("程序启动时下载 id:" + lisDownloadObj[i].id + " url" + lisDownloadObj[i].url + " localurl:" + lisDownloadObj[i].parentPath + " downloadLength" + lisDownloadObj[i].sDownloadFileResult.downloadedLengthStr);
                        //StartDownloadOne(lisDownloadObj[i]);

                        Debug.Log("程序启动时检查到正在下载 id:" + lisDownloadObj[i].id);
                        lisDownloadObj[i].ChangeDownloadState(DownloadState.pause);
                        UpdateAllDownload();
                    }
                }

            }

        }


        public DownloadObj StartDownloadOne(DownloadObj downloadObj)
        {
            UnityWeb unityWeb = unityDownloadPool.GetOneGo().GetComponent<UnityWeb>();
            unityWeb.gameObject.SetActive(true);
            downloadObj.SetWebDownload(unityWeb);

            unityWeb.DownloadFileSize(downloadObj);
            return downloadObj;
        }

        public void DeleteOneDownload(DownloadObj downloadObj, bool isDeleteLoaclFile)
        {
            RemoveDownloadObj(downloadObj);
            if (isDeleteLoaclFile == true)
            {
                if (File.Exists(downloadObj.fullPath))
                    File.Delete(downloadObj.fullPath);

                if (File.Exists(downloadObj.fullPath + ".tmp"))
                    File.Delete(downloadObj.fullPath + ".tmp");
            }
        }



    }
}



