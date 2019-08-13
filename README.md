# Unity Http下载文件工具
- 环境：支持UnityWebRequest的Unity版本  
- 描述：  
1. 批量下载、断点续传   
2. 下载进度、完成回调    
3. 支持暂停、继续、删除下载  
4. 本地持久化存储下载历史，下载工具类启动时把正在下载的自动开始下载   
```
//第一步启动下载器
            UnityDownloadManager.instance.StartDownloadManager();
            
//第二步调用下载（批量下载就多次调用）
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

//第三步暂停、继续下载
            
```
