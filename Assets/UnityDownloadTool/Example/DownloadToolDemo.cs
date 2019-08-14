using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadToolDemo : MonoBehaviour
{
    void Start()
    {
        //1.第一步启动下载器
        UnityDownloadManager.instance.StartDownloadManager();

        this.DownloadOne();
    }


    void DownloadOne()
    {
        int id = 1;

        //2.第二步调用下载（批量下载就多次调用）
        DownloadObj downloadObj = UnityDownloadManager.instance.GetDownloadObjByID(id);
        Debug.Log(downloadObj.ToString());
        if (downloadObj == null)
        {
            string parentPath = Environment.CurrentDirectory + "\\下载Data";
            downloadObj = new DownloadObj().SetID(id).SetUrl("https://yoyohan1.gitee.io/logo.ico").SetParentPath(parentPath).SetFileName("example1.ico").AddExtra("videoType", 1).AddExtra("coverUrl", "coverUrl");
            downloadObj.OnProgressAction += OnProgress;
            downloadObj.OnCompleteAction += OnComplete;
        }
        else if (downloadObj.currentDownloadState == DownloadState.pause)
        {
            downloadObj.OnProgressAction += OnProgress;
            downloadObj.OnCompleteAction += OnComplete;
        }

        UnityDownloadManager.instance.StartDownloadOne(downloadObj);


        //TODO..处理开始下载的逻辑
    }


    void OnProgress(DownloadObj downloadObj)
    {
        Debug.Log(downloadObj.sDownloadFileResult.downloadedLength + "   " + downloadObj.sDownloadFileResult.contentLength);

        //if (downloadAmount == null)
        //{
        //    downloadObj.OnProgressAction -= OnProgress;
        //    downloadObj.OnCompleteAction -= OnComplete;
        //    return;
        //}

        //downloadAmount.fillAmount = (float)downloadObj.sDownloadFileResult.downloadedLength / downloadObj.sDownloadFileResult.contentLength;
    }

    void OnComplete(int code, DownloadObj downloadObj)
    {
        Debug.Log("收到下载完成回调!   code:" + code + "  localPath:" + downloadObj.parentPath);
        if (code == 200 || code == 0)
        {

        }
        else
        {
            //downloadAmount.gameObject.SetActive(false);
        }

        downloadObj.OnProgressAction -= OnProgress;
        downloadObj.OnCompleteAction -= OnComplete;
    }



}
