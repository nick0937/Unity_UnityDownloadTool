using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace QP.Framework
{
    public class HeadHandler : DownloadHandlerScript
    {
        public int ContentLength { get; private set; }
        /// <summary>
        /// 文件数据长度
        /// </summary>
        protected override void ReceiveContentLength(int contentLength)
        {
            ContentLength = contentLength;
        }
        /// <summary>
        /// 接受到数据时
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataLength"></param>
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            return true;
        }

        public void Dispose()
        {

        }
    }
}

