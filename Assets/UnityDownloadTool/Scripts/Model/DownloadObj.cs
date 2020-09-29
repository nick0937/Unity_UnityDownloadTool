using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
/// <summary>
/// 描述：
/// 功能：
/// 作者：yoyohan
/// 创建时间：2019-05-13 11:54:35
/// </summary>
namespace yoyohan
{

    public class DownloadObj
    {
        public string id;//唯一值 去区分
        public string url;
        public string parentPath;
        public string fileName;
        public SDownloadFileResult sDownloadFileResult = new SDownloadFileResult();
        public DownloadState currentDownloadState;
        public Dictionary<string, object> dicExtra = new Dictionary<string, object>();

        [NonSerialized]
        public IWebDownload webDownload;
   
        //获取过Filesize 可用 设置过filename了那时
        private string _fullPath;
        public string fullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_fullPath))
                {
                    _fullPath = string.Format("{0}{1}{2}", this.parentPath, Path.DirectorySeparatorChar, this.fileName);
                }
                return _fullPath;
            }
        }

        public DownloadObj()
        {

        }

        public DownloadObj(JsonData jsonData)
        {
            id = jsonData.GetValue<string>("id");
            url = jsonData.GetValue<string>("url");
            parentPath = jsonData.GetValue<string>("parentPath");
            fileName = jsonData.GetValue<string>("fileName");
            sDownloadFileResult = new SDownloadFileResult(jsonData.GetValue<JsonData>("sDownloadFileResult"));
            currentDownloadState = (DownloadState)jsonData.GetValue<int>("currentDownloadState");
            dicExtra = jsonData.GetValue<Dictionary<string, object>>("dicExtra");

            if (currentDownloadState != DownloadState.downloaded)
            {
                if (File.Exists(fullPath + ".tmp"))
                {
                    sDownloadFileResult.downloadedLength = (int)new FileInfo(fullPath + ".tmp").Length;
                }
                else
                {
                    sDownloadFileResult.downloadedLength = 0;
                }
            }
        }

        public JsonData ToJsonData()
        {
            JsonData jsonData = new JsonData();
            jsonData.SetKeyValue("id", id)
                .SetKeyValue("url", url)
                .SetKeyValue("parentPath", parentPath)
                .SetKeyValue("fileName", fileName)
                .SetKeyValue("currentDownloadState", (int)currentDownloadState)
                .SetKeyValue("sDownloadFileResult", sDownloadFileResult.ToJsonData())
                .SetKeyValue("dicExtra", dicExtra);
            return jsonData;
        }


        #region 链式赋值
        public DownloadObj SetID(string id)
        {
            this.id = id;
            return this;
        }

        public DownloadObj SetUrl(string url)
        {
            this.url = url;
            return this;
        }

        public DownloadObj SetParentPath(string parentPath)
        {
            this.parentPath = parentPath;
            return this;
        }

        /// <summary>
        /// 建议加上后缀 因为自动解析后缀有时候会失误
        /// </summary>
        public DownloadObj SetFileName(string fileName)
        {
            this.fileName = fileName;
            return this;
        }

        public DownloadObj SetContentLength(int size)
        {
            this.sDownloadFileResult.contentLength = size;
            return this;
        }

        public DownloadObj AddExtra(string key, object value)
        {
            if (!dicExtra.ContainsKey(key))
            {
                dicExtra.Add(key, value);
            }
            return this;
        }

        public DownloadObj SetWebDownload(IWebDownload webDownload)
        {
            this.webDownload = webDownload;
            return this;
        }
        #endregion


        public object GetExtra(string key)
        {
            object value = null;
            dicExtra.TryGetValue(key, out value);
            if (value == null)
            {
                Debug.LogError("dicExtra没能找到该Key对应的Value，Key：" + key);
            }
            return value;
        }

        public DownloadObj ChangeDownloadState(DownloadState state)
        {
            this.currentDownloadState = state;
            return this;
        }

        public DownloadObj ChangeDownloadLength(int size)
        {
            this.sDownloadFileResult.downloadedLength = size;

            if (this.sDownloadFileResult.contentLength == this.sDownloadFileResult.downloadedLength)
                currentDownloadState = DownloadState.downloaded;
            else
                currentDownloadState = DownloadState.downloading;

            return this;
        }


        public override string ToString()
        {
            return string.Format("id:{0}    url:{1}    loaclurl:{2}    fileName:{3}", this.id, this.url, this.parentPath, this.fileName);
        }

    }

    public enum DownloadState
    {
        UNKNOW,
        getSize,
        pause,
        downloading,
        downloaded,
        failed
    }
}