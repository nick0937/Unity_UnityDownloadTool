using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 下载进度 回调数据
/// </summary>
public struct SDownloadFileResult
{
    /// <summary>
    /// 文件总大小
    /// </summary>
    public int contentLength;
    /// <summary>
    /// 已下载的大小
    /// </summary>
    public int downloadedLength;


    // 文件总大小字符串 （人类可读的）
    [Newtonsoft.Json.JsonIgnore]
    public string contentLengthStr
    {
        get
        {
            return QP.Framework.Util.HumanReadableFilesize(contentLength);
        }
    }

    // 已下载的大小字符串 （人类可读的）
    [Newtonsoft.Json.JsonIgnore]
    public string downloadedLengthStr
    {
        get
        {
            return QP.Framework.Util.HumanReadableFilesize(downloadedLength);
        }
    }

}