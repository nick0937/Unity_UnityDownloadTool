using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace QP.Framework
{
    public struct RequestHandler
    {
        public UnityWebRequest request;
        public UnityDownloadFileHandler handler;
        public RequestHandler(UnityWebRequest request, UnityDownloadFileHandler handler)
        {
            this.request = request;
            this.handler = handler;
        }
    }
    public class UnityWeb : MonoBehaviour, IWebDownload
    {
        private Dictionary<string, RequestHandler> requestDict = new Dictionary<string, RequestHandler>();
        private DownloadObj mDownloadObj;

        public void DownloadFileSize(DownloadObj downloadObj)
        {
            this.mDownloadObj = downloadObj;
            mDownloadObj.ChangeDownloadState(DownloadState.downloading);
            UnityDownloadManager.instance.UpdateAllDownload();
            StartCoroutine(DownloadFileSizeHandler());
        }
        public void DownloadFile(DownloadObj downloadObj)
        {
            this.mDownloadObj = downloadObj;
            StartCoroutine(DownloadFileHandler());
        }
        IEnumerator DownloadFileSizeHandler()
        {
            HeadHandler handler = new HeadHandler();
            UnityWebRequest request = UnityWebRequest.Head(mDownloadObj.url);
            request.downloadHandler = handler;
            request.timeout = UnityDownloadManager.TIMEOUT;
            request.chunkedTransfer = true;
            request.disposeDownloadHandlerOnDispose = true;
            yield return request.SendWebRequest();

            //Dictionary<string, string> dic = request.GetResponseHeaders();
            //foreach (var item in dic) Debug.Log(item.Key + "     " + item.Value);
            //item.Key:Content-Type  item.Value:video/mp4
            //item.Key:Content-Disposition  item.Value:inline; filename = "o_1d6hoeipu1bem1rdjvqtush1gu8l.mp4"; filename *= utf - 8' 'o_1d6hoeipu1bem1rdjvqtush1gu8l.mp4

            //设置文件名的后缀
            string content_Type = request.GetResponseHeader("Content-Type");

            string[] arr = content_Type.Split('/');
            string houzhui = string.Format(".{0}", arr[arr.Length - 1]);

            if (!mDownloadObj.fileName.Contains("."))
            {
                mDownloadObj.fileName += houzhui;
                Debug.Log("添加后缀：" + mDownloadObj.fileName);
            }

            if (mDownloadObj.OnGetFileSizeAction != null)
            {
                mDownloadObj.SetContentLength(handler.ContentLength);
                mDownloadObj.OnGetFileSizeAction((int)request.responseCode, mDownloadObj);//request.responseCode == 200 代表获取文件大小成功
            }
            request.Abort();
            request.Dispose();
        }
        IEnumerator DownloadFileHandler()
        {
            UnityDownloadFileHandler handler = new UnityDownloadFileHandler(mDownloadObj);
            UnityWebRequest request = UnityWebRequest.Get(mDownloadObj.url);
            requestDict.Add(mDownloadObj.url, new RequestHandler(request, handler));

            int rangeFrom = handler.GetDownloadObj().sDownloadFileResult.downloadedLength;
            int rangeTo = handler.GetDownloadObj().sDownloadFileResult.contentLength;
            if (rangeFrom >= rangeTo)
            {
                Debug.Log("设置Range超出！ rangeFrom:" + rangeFrom + " rangeTo:" + rangeTo);
                OnResponse(200);//防止range太大 出现416报错
                yield break;
            }

            request.SetRequestHeader("Range", string.Format("bytes={0}-", rangeFrom)); //成功返回206
            request.downloadHandler = handler;
            request.timeout = UnityDownloadManager.TIMEOUT;
            request.chunkedTransfer = true;
            request.disposeDownloadHandlerOnDispose = true;

            yield return request.SendWebRequest();
            int code = (int)request.responseCode;
            OnResponse(code);
        }

        private void OnResponse(int code)
        {
            Dispose(mDownloadObj.url);//结束UnityDownloadFileHandler的文件操作内存共享

            Debug.Log("下载结束!!!!!!   code:" + code + "  localPath:" + mDownloadObj.parentPath);
            if (code == 200 || code == 0 || code == 206)
            {
                if (mDownloadObj.sDownloadFileResult.contentLength == mDownloadObj.sDownloadFileResult.downloadedLength)
                {
                    string tmp = mDownloadObj.fullPath + ".tmp";
                    if (File.Exists(tmp))
                    {
                        if (File.Exists(mDownloadObj.fullPath))
                            File.Delete(mDownloadObj.fullPath);
                        File.Move(tmp, mDownloadObj.fullPath);
                        //下载完成更新本地Data
                        mDownloadObj.ChangeDownloadState(DownloadState.downloaded);
                        UnityDownloadManager.instance.UpdateAllDownload();
                        //Gobal.youDaSdkOtherController.updateMedia(mDownloadObj.loaclUrl);
                        Debug.Log("下载成功 并移除tmp后缀 并保存到本地下载历史！");
                    }
                }
                else
                {
                    Debug.Log("下载得大小不等于总大小！可能是由于中止！造成206");
                }

            }


            if (mDownloadObj.OnCompleteAction != null)
                mDownloadObj.OnCompleteAction(code, mDownloadObj);
        }


        private void Dispose(string key)
        {
            RequestHandler result;
            if (requestDict.TryGetValue(key, out result))
            {
                result.handler.Dispose();
                result.request.Abort();
                requestDict.Remove(key);
            }
        }

        /// <summary>
        /// 停止下载
        /// </summary>
        public void Close()
        {
            Debug.Log("停止下载-------------------------------------Close");
            StopAllCoroutines();
            Dispose(mDownloadObj.url);
            UnityDownloadManager.instance.GetUnityDownloadPool().RestoreOneGo(this.gameObject);

            //暂停更新本地Data
            mDownloadObj.ChangeDownloadState(DownloadState.pause);
            UnityDownloadManager.instance.UpdateAllDownload();
        }

        void OnDestroy()
        {
            Dictionary<string, RequestHandler>.Enumerator e = requestDict.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Value.handler.Dispose();
                e.Current.Value.request.Abort();
            }
            e.Dispose();
            StopAllCoroutines();
            requestDict.Clear();
            requestDict = null;
        }
    }
}

