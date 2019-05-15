using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    public class RoomPanel : PanelBase
    {
        private Button btnStartServer;
        private Button btnBackToLobbyPanel;

        private Transform side1Content;
        private Transform side2Content;

        private Transform playerPanel;

        private List<RoomPlayerInfo> infoList;
        private bool isHost = false;

        private Button btnMapName;
        private string mapName;
        private Text textDownloadStatus;
        private Text textServerFunc;

        private string hostName;

        private GameObject PlayerInfoTag;

        #region 生命周期
        public override void Init(params object[] args)
        {
            base.Init(args);

            //args[1]是玩家是否是主机
            isHost = (bool)args[1];
            Global.Instance.gameInfo.IsHost = isHost;

            //args[0]是服务器里的玩家信息
            infoList = (List<RoomPlayerInfo>)args[0];

            hostName = (string)args[2];  //2是主机名字
            mapName = (string)args[3];   //3是地图名字
            Global.Instance.gameInfo.MapName = mapName;

            skinPath = "Panel/MainMenu/RoomPanel";
            layer = PanelLayer.Panel;
        }

        public override void OnShowing()
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            btnStartServer = skinTrans.Find("BtnStartServer").GetComponent<Button>();
            btnBackToLobbyPanel = skinTrans.Find("BtnBackToLobbyPanel").GetComponent<Button>();
            playerPanel = skinTrans.Find("PlayerPanel").transform;
            side1Content = skinTrans.Find("PlayerPanel/Side1Panel/Side1Content").transform;
            side2Content = skinTrans.Find("PlayerPanel/Side2Panel/Side2Content").transform;
            PlayerInfoTag = Resources.Load<GameObject>("Prefabs/Lobby/PlayerInfoTag");
            btnMapName = skinTrans.Find("BtnMapName").GetComponent<Button>();
            textDownloadStatus = skinTrans.Find("BtnMapName/TextDownloadStatus").GetComponent<Text>();
            textServerFunc = skinTrans.Find("BtnStartServer/Text").GetComponent<Text>();

            btnStartServer.onClick.AddListener(BtnStartServer);
            btnBackToLobbyPanel.onClick.AddListener(BtnBackToLobbyPanel);
        }

        public override void OnShowed()
        {
            base.OnShowed();

            btnMapName.transform.Find("TextMapName").GetComponent<Text>().text = mapName;

            if (!isHost)//非主机
            {
                CreatePlayerInfoTag(side1Content, infoList[0]);
                CreatePlayerInfoTag(side2Content, infoList[1]);
                NetMgr.srvConn.msgDist.AddOnceListener("KickServer", RecvKickServer);   //监听被踢事件
                NetMgr.srvConn.msgDist.AddOnceListener("MapPrepare", RecvMapPrepare);      //监听地图包文件开头

                textServerFunc.text = "等待下载";
                btnStartServer.enabled = false;
            }
            else        //主机
            {
                CreatePlayerInfoTag(side1Content, infoList[0]);
                //等待加入客机
                NetMgr.srvConn.msgDist.AddListener("AddClient", RecvAddClient);   //监听加入事件
                //等待离开房间
                NetMgr.srvConn.msgDist.AddListener("DelClient", RecvDelClient);   //监听离开事件

                textDownloadStatus.text = "";
            }
            NetMgr.srvConn.msgDist.AddListener("LobbyStatus", RecvLobbyStatus);   //监听准备状态事件
        }
        #endregion

        #region 按钮监听
        private bool unclicked=true;
        private void BtnStartServer()
        {
            /*
            None,          //未加入任何房间               0
            NotPrepared,   //加入房间了但处于未准备状态   1
            Prepare,       //加入房间了并处于准备状态     2
            Fighting,      //加入房间了并处于游戏战斗状态 3
            */
            if (infoList.Count == 1)
                return;
            if (isHost)
            {
                if (unclicked)
                {
                    textServerFunc.text = "开始游戏";
                    //发送准备指令
                    ProtocolBytes readyInfo = new ProtocolBytes();
                    readyInfo.AddString("LobbyStatus");
                    readyInfo.AddString(Global.Instance.gameInfo.playerInfo.PlayerName);
                    readyInfo.AddInt(2);
                    NetMgr.srvConn.Send(readyInfo);
                    unclicked = !unclicked;
                }
                else
                {
                    if (infoList.Count == 1)
                    {
                        PanelMgr.Instance.OpenPanel<Tip.WarningTip>("", "人数不足，无法进入游戏!");
                        return;
                    }
                    bool allReady = true;
                    foreach (RoomPlayerInfo info in infoList)
                    {
                        if (info.Status != 2)
                        {
                            allReady = false;
                            break;
                        }
                    }
                    if (allReady)
                    {
                        ProtocolBytes readyInfo = new ProtocolBytes();
                        readyInfo.AddString("LobbyStatus");
                        readyInfo.AddString(Global.Instance.gameInfo.playerInfo.PlayerName);
                        readyInfo.AddInt(3);
                        NetMgr.srvConn.Send(readyInfo);

                        StartGame();
                    }
                    else
                    {
                        PanelMgr.Instance.OpenPanel<Tip.WarningTip>("","请等待所有人准备!");
                        return;
                    }
                }
            }
            else
            {
                if (unclicked)
                {
                    textServerFunc.text = "未准备";
                    //发送准备指令 
                    ProtocolBytes readyInfo = new ProtocolBytes();
                    readyInfo.AddString("LobbyStatus");
                    readyInfo.AddString(Global.Instance.gameInfo.playerInfo.PlayerName);
                    readyInfo.AddInt(2);
                    NetMgr.srvConn.Send(readyInfo);
                }
                else
                {
                    textServerFunc.text = "准备";
                    //发送未准备指令
                    ProtocolBytes readyInfo = new ProtocolBytes();
                    readyInfo.AddString("LobbyStatus");
                    readyInfo.AddString(Global.Instance.gameInfo.playerInfo.PlayerName);
                    readyInfo.AddInt(1);
                    NetMgr.srvConn.Send(readyInfo);
                }
                unclicked = !unclicked;
            }
        }

        private void BtnBackToLobbyPanel()
        {
            PanelMgr.Instance.OpenPanel<LobbyPanel>("");

            if (!isHost) //不是主机
            {
                ProtocolBytes protocol = new ProtocolBytes();
                protocol.AddString("LeaveServer");
                protocol.AddString(Global.Instance.gameInfo.playerInfo.PlayerName);
                NetMgr.srvConn.Send(protocol);
            }
            else        //是主机
            {
                ProtocolBytes protocol = new ProtocolBytes();
                protocol.AddString("DelServer");
                protocol.AddString(Global.Instance.gameInfo.playerInfo.PlayerName);
                NetMgr.srvConn.Send(protocol);
            }

            Close();
        }
        #endregion

        #region 辅助方法

        private void CreatePlayerInfoTag(Transform content,RoomPlayerInfo info)
        {
            GameObject go = Instantiate(PlayerInfoTag, content);
            go.transform.localPosition = Vector3.zero;
            info.SetInfoTag(go);
        }

        private void StartGame()
        {
            Debug.Log("进入游戏");
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Level");
            NetMgr.srvConn.msgDist.DelListener("LobbyStatus", RecvLobbyStatus);
            NetMgr.srvConn.msgDist.DelListener("AddClient", RecvAddClient);
            NetMgr.srvConn.msgDist.DelListener("DelClient", RecvDelClient);
        }
        #endregion

        #region 回调
        //参数：AddClient，客机名，客机状态
        //发送：MapData，地图文件
        private void RecvAddClient(ProtocolBase protocol)
        {
            ProtocolBytes pb = (ProtocolBytes)protocol;
            int start = 0;
            pb.GetString(start, ref start);  //AddClient
            string clientName = pb.GetString(start, ref start);
            int status = pb.GetInt(start, ref start);

            infoList.Add(new RoomPlayerInfo(clientName, status));
            CreatePlayerInfoTag(side2Content, infoList[infoList.Count-1]);

            #region 得到地图包，计算包数
            string mapData = MapLoader.GetLevel(mapName).ToString();
            int count = mapData.Length / 1000;
            #endregion
            #region 发送地图包数量
            ProtocolBytes mapPrepare = new ProtocolBytes();
            mapPrepare.AddString("MapPrepare");
            mapPrepare.AddInt(count);
            NetMgr.srvConn.Send(mapPrepare);
            #endregion
            #region 发送每个地图包
            for (int i = 0; i < count; i++)
            {
                ProtocolBytes mapPb = new ProtocolBytes();
                mapPb.AddString("MapData");
                mapPb.AddInt(i);
                mapPb.AddString(mapData.Substring(i*1000,1000));   
                NetMgr.srvConn.Send(mapPb);
            }
            ProtocolBytes mapPbLast = new ProtocolBytes();
            mapPbLast.AddString("MapData");
            mapPbLast.AddInt(count);
            mapPbLast.AddString(mapData.Substring(count*1000));
            NetMgr.srvConn.Send(mapPbLast);
            #endregion
        }

        //参数：DelClient，客机名
        private void RecvDelClient(ProtocolBase protocol)
        {
            ProtocolBytes pb = (ProtocolBytes)protocol;
            int start = 0;
            pb.GetString(start, ref start);//"DelClient"
            string clientName = pb.GetString(start, ref start);
            for (int i = 0; i < side2Content.childCount;i++)
            {
                Destroy(side2Content.GetChild(i).gameObject);
                infoList.Remove(infoList.Find((RoomPlayerInfo info) => info.playerName == clientName));
            }
        }

        //参数:KickServer，自己的名字
        //当主机退出，自己被踢时调用
        private void RecvKickServer(ProtocolBase protocol)
        {
            ProtocolBytes pb = (ProtocolBytes)protocol;
            int start = 0;
            pb.GetString(start, ref start);//"KickServer"
            string name = pb.GetString(start, ref start);
            if (name==Global.Instance.gameInfo.playerInfo.PlayerName)
            {
                BtnBackToLobbyPanel();
            }
        }

        private int mapDataCount;
        private string[] mapBuff;
        //参数：MapPrepare ,文件个数
        //收到主机发送的地图文件开头
        private void RecvMapPrepare(ProtocolBase protocol)
        {
            ProtocolBytes info = (ProtocolBytes)protocol;
            int start = 0;
            info.GetString(start, ref start);//"MapPrepare"
            mapDataCount = info.GetInt(start, ref start);
            NetMgr.srvConn.msgDist.AddListener("MapData", RecvMapData);
            textDownloadStatus.text = "下载开始";
            mapBuff = new string[mapDataCount + 1];
        }

        //参数：MapData，文件序号,地图文件
        //收到主机发送的地图文件
        private void RecvMapData(ProtocolBase protocol)
        {
            ProtocolBytes info = (ProtocolBytes)protocol;
            int start = 0;
            info.GetString(start, ref start);//"MapData"
            int packageIndex = info.GetInt(start, ref start);
            string mapDataPackage = info.GetString(start, ref start);
            mapBuff[packageIndex] = mapDataPackage;
            textDownloadStatus.text = "下载进度%" + (packageIndex * 100 / mapDataCount).ToString();
            if (packageIndex == mapDataCount)
            {
                string finalMapData=null;
                for (int i = 0; i <= mapDataCount; i++)
                {
                    finalMapData += mapBuff[i];
                }
                textDownloadStatus.text = "下载完成";
                NetMgr.srvConn.msgDist.DelListener("MapData", RecvMapData);
                Level downloadedLevel = JsonUtility.FromJson<Level>(finalMapData);
                MapLoader.SaveMap(downloadedLevel,false);
                MapLoader.CustomLevelDic[mapName] = downloadedLevel;

                btnStartServer.enabled = true;
                textServerFunc.text = "准备";
            }
        }

        //参数:LobbyStatus 玩家名 状态值
        //表示服务器收到玩家准备状态的回调
        private void RecvLobbyStatus(ProtocolBase protocol)
        {
            ProtocolBytes info = (ProtocolBytes)protocol;
            int start = 0;
            info.GetString(start, ref start);//"LobbyStatus"
            string n = info.GetString(start, ref start);
            int lobbyStatus = info.GetInt(start, ref start);
            if (lobbyStatus == 1)
            {
                infoList.Find((RoomPlayerInfo i) => i.playerName == n).Status = 1;
            }
            else if (lobbyStatus == 2)
            {
                infoList.Find((RoomPlayerInfo i) => i.playerName == n).Status = 2;
            }
            else if (lobbyStatus == 3)
            {
                if (hostName == n&&Global.Instance.gameInfo.playerInfo.PlayerName!=n)
                {
                    Debug.Log("跟随主机进入游戏");
                    ProtocolBytes readyInfo = new ProtocolBytes();
                    readyInfo.AddString("LobbyStatus");
                    readyInfo.AddString(Global.Instance.gameInfo.playerInfo.PlayerName);
                    readyInfo.AddInt(3);
                    NetMgr.srvConn.Send(readyInfo);
                    NetMgr.srvConn.msgDist.DelListener("LobbyStatus", RecvLobbyStatus);

                    StartGame();
                }
            }
        }
        #endregion
    }
}
