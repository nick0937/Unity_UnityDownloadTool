using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace yoyohan
{
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
        public string contentLengthStr
        {
            get
            {
                return Util.HumanReadableFilesize(contentLength);
            }
        }

        // 已下载的大小字符串 （人类可读的）
        public string downloadedLengthStr
        {
            get
            {
                return Util.HumanReadableFilesize(downloadedLength);
            }
        }

        // 百分比字符串 70.00%
        public string precentStr
        {
            get
            {
                return ((float)downloadedLength / contentLength).ToString("P");
            }
        }

        public SDownloadFileResult(JsonData jsonData)
        {
            contentLength = jsonData.GetValue<int>("contentLength");
            downloadedLength = jsonData.GetValue<int>("downloadedLength");
        }

        public JsonData ToJsonData()
        {
            JsonData jsonData = new JsonData();
            jsonData.SetKeyValue("contentLength", contentLength)
                .SetKeyValue("downloadedLength", downloadedLength);
            return jsonData;
        }

    }
}