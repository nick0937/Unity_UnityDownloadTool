using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QP.Framework;
using System;
using System.IO;
/// <summary>
/// 描述：
/// 功能：
/// 作者：yoyohan
/// 创建时间：2019-05-13 11:54:35
/// </summary>
public class DownloadObj //: GridDataBase
{
    public new int id;//唯一值 去区分
    public string url;
    public string parentPath;
    public string fileName;
    public SDownloadFileResult sDownloadFileResult = new SDownloadFileResult();
    public DownloadState currentDownloadState;
    public Dictionary<object, object> dicExtra = new Dictionary<object, object>();

    [NonSerialized]
    public IWebDownload webDownload;
    [NonSerialized]
    public Action<DownloadObj> OnProgressAction;
    [NonSerialized]
    public Action<int, DownloadObj> OnCompleteAction;//参数1为code
    [NonSerialized]
    public Action<int, DownloadObj> OnGetFileSizeAction;//参数1为code

    //获取过Filesize 可用 设置过filename了那时
    private string _fullPath;
    [Newtonsoft.Json.JsonIgnore]
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


    #region 链式赋值
    public DownloadObj SetID(int id)
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

    public DownloadObj AddExtra(object key, object value)
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

    public object GetExtra(object key)
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
        //this.sDownloadFileResult.downloadedLengthStr = Util.HumanReadableFilesize(size);

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
    pause,
    downloading,
    downloaded
}