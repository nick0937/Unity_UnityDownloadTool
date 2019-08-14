using System;
namespace QP.Framework
{
    public interface IWebDownload
    {
        /// <summary>
        /// 获取远程文件大小
        /// </summary>
        void DownloadFileSize(DownloadObj downloadingDetail);
        /// <summary>
        /// 下载一个文件
        /// </summary>
        void DownloadFile(DownloadObj downloadingDetail);

        void Close();
    }
}

