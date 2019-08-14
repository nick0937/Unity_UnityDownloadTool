using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace QP.Framework
{
    public class UnityDownloadFileHandler : DownloadHandlerScript
    {
        private FileStream _fileStream;
        private DownloadObj mDownloadObj;

        public DownloadObj GetDownloadObj()
        {
            return mDownloadObj;
        }

        public UnityDownloadFileHandler(DownloadObj downloadObj) : base(new byte[1024 * 200])
        {
            this.mDownloadObj = downloadObj;

            string dir = Path.GetDirectoryName(mDownloadObj.fullPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _fileStream = new FileStream(downloadObj.fullPath + ".tmp", FileMode.Append, FileAccess.Write);

            mDownloadObj.ChangeDownloadLength((int)_fileStream.Length);
        }
        /// <summary>
        /// 文件数据长度
        /// </summary>
        protected override void ReceiveContentLength(int contentLength)
        {
            mDownloadObj.SetContentLength(mDownloadObj.sDownloadFileResult.downloadedLength + contentLength);
        }
        /// <summary>
        /// 接受到数据时
        /// </summary>
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || dataLength == 0 || _fileStream == null)
            {
                return false;
            }
            _fileStream.Write(data, 0, dataLength);

            mDownloadObj.ChangeDownloadLength(mDownloadObj.sDownloadFileResult.downloadedLength += dataLength);
            //Debug.Log("正在下载：" + mDownloadObj.sDownloadFileResult.downloadedLength);
            if (mDownloadObj.OnProgressAction != null)
            {
                mDownloadObj.OnProgressAction(mDownloadObj);
            }
            return true;
        }
        protected override void CompleteContent()
        {
            //Debug.Log(_download_url + "  下载完毕");
        }
        public void Dispose()
        {
            if (_fileStream != null)
            {
                //Debug.Log("关闭流");
                _fileStream.Close();
                _fileStream.Dispose();
                _fileStream = null;
            }
        }
    }
}

