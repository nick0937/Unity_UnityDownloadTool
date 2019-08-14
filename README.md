# Unity Http下载文件工具
- 环境：  
支持UnityWebRequest和DownloadHandlerScript的Unity版本（Unity5.3.2不可以 2017以后都可以 中间的没测试）  
- 描述：  
1. 批量下载、断点续传   
2. 下载进度、完成回调    
3. 支持暂停、继续、删除下载  
4. 本地持久化存储下载历史和下载Extra参数，下载工具类启动时把正在下载的自动开始下载  
- 示例代码：（项目里也有demo场景）
```
1. 第一步启动下载器
            UnityDownloadManager.instance.StartDownloadManager();
            
2. 第二步调用下载（批量下载就多次调用）
            DownloadObj downloadObj = UnityDownloadManager.instance.GetDownloadObjByID(PlayVideoManager.instance.GetPlayObj().resourceID);
            if (downloadObj == null)
            {
                localPath = Gobal.youDaSdkOtherController.getExternalPath() + "/DownloadMovies/" + resourceId + "_" + PlayVideoManager.instance.GetPlayObj().videoName;
                downloadObj = new DownloadObj().SetID(PlayVideoManager.instance.GetPlayObj().resourceID).SetUrl(PlayVideoManager.instance.GetPlayObj().videoPath).SetLoaclUrl(localPath).SetFileName(PlayVideoManager.instance.GetPlayObj().videoName).AddExtra("videoType", PlayVideoManager.instance.GetPlayObj().videoType).AddExtra("coverUrl", PlayVideoManager.instance.GetPlayObj().coverUrl);
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

3. 暂停、继续下载、删除下载
            //暂停下载
            mDownloadObj.webDownload.Close();
            //继续下载（即再次调用下载）
            UnityDownloadManager.instance.StartDownloadOne(mDownloadObj);
            //删除下载（可选是否删除已下载的文件）
            UnityDownloadManager.instance.DeleteOneDownload(mDownloadObj, true);
            
4. 下载历史数据相关接口
            //获取下载历史
            List<DownloadObj> downloadObjs = UnityDownloadManager.instance.GetAllDownloadObj();
            //更新下载历史（可手动调用  工具类里自动更新时机为：下载完成时保存到本地 ，开始下载时保存到本地）
            UnityDownloadManager.instance.UpdateAllDownload();
            //获取单个下载历史（根据id）
            DownloadObj downloadObj = UnityDownloadManager.instance.GetDownloadObjByID(id);                 
```
- DownloadObj类解析
![image](https://github.com/yoyohan1/Unity_HttpDownloadTool/blob/master/DownloadObj%E7%B1%BB%E8%A7%A3%E6%9E%90.png)
