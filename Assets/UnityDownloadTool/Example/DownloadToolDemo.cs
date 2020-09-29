using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace yoyohan.DownloadToolDemo
{
    public class DownloadToolDemo : MonoBehaviour
    {

        private DownloadObj mDownloadObj;

        void Start()
        {
            //1.第一步启动下载器
            UnityDownloadMgr.instance.StartDownloadManager();

            this.DownloadOne();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                this.DownloadOne();
            }
        }


        void DownloadOne()
        {
            string id = "1";

            //2.第二步调用下载（批量下载就多次调用）
            mDownloadObj = UnityDownloadMgr.instance.GetDownloadObjByID(id);

            if (mDownloadObj == null)
            {
                string parentPath = Environment.CurrentDirectory + "\\下载Data";
                mDownloadObj = new DownloadObj().SetID(id).SetUrl("https://yoyohan1.gitee.io/logo.ico").SetParentPath(parentPath).SetFileName("example1.ico").AddExtra("videoType", 1).AddExtra("coverUrl", "coverUrl");
            }
            UnityDownloadMgr.instance.StartDownloadOne(mDownloadObj);


            //TODO..处理开始下载的逻辑
        }


        void OnProgress(DownloadObj downloadObj)
        {
            if (mDownloadObj == null || mDownloadObj.id != downloadObj.id)
                return;

            Debug.Log(downloadObj.sDownloadFileResult.downloadedLength + "   " + downloadObj.sDownloadFileResult.contentLength);

            //if (downloadAmount == null)
            //{
            //    downloadObj.OnProgressAction -= OnProgress;
            //    downloadObj.OnCompleteAction -= OnComplete;
            //    return;
            //}

            //downloadAmount.fillAmount = (float)downloadObj.sDownloadFileResult.downloadedLength / downloadObj.sDownloadFileResult.contentLength;
        }

        void OnComplete(DownloadObj downloadObj)
        {
            if (mDownloadObj == null || mDownloadObj.id != downloadObj.id)
                return;

            Debug.Log("收到下载完成回调!  下载状态:" + downloadObj.currentDownloadState.ToString() + "  下载路径:" + downloadObj.parentPath);

        }

        void OnGetFileSize(DownloadObj downloadObj)
        {
            if (mDownloadObj == null || mDownloadObj.id != downloadObj.id)
                return;

            Debug.Log("获取下载的文件大小成功！ 下载状态：" + downloadObj.currentDownloadState.ToString() + " 文件大小:" + downloadObj.sDownloadFileResult.contentLengthStr);

            downloadObj.webDownload.DownloadFile(downloadObj);
        }


        void OnEnable()
        {
            UnityDownloadMgr.instance.OnDownloadingAction += OnProgress;
            UnityDownloadMgr.instance.OnDownloadedAction += OnComplete;
            UnityDownloadMgr.instance.OnGetFileSizeAction += OnGetFileSize;
        }
        void OnDisable()
        {
            UnityDownloadMgr.instance.OnDownloadingAction -= OnProgress;
            UnityDownloadMgr.instance.OnDownloadedAction -= OnComplete;
            UnityDownloadMgr.instance.OnGetFileSizeAction -= OnGetFileSize;
        }
    }
}