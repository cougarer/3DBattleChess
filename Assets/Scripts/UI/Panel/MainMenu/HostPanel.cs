using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.Tip;

namespace UI.Panel
{
    public class HostPanel : PanelBase
    {
        private Button btnHostGame;
        private Button btnBackToLobby;
        private Transform MapContent;

        private Button btnServerOption;

        private string MapName;

        #region 生命周期
        public override void Init(params object[] args)
        {
            base.Init(args);

            skinPath = "Panel/MainMenu/HostPanel";
            layer = PanelLayer.Panel;
        }

        public override void OnShowing()
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            btnHostGame = skinTrans.Find("BtnHostGame").GetComponent<Button>();
            btnBackToLobby = skinTrans.Find("BtnBackToLobby").GetComponent<Button>();
            MapContent = skinTrans.Find("Scroll View/Viewport/Content");
            btnServerOption = skinTrans.Find("BtnServerOption").GetComponent<Button>();

            btnHostGame.onClick.AddListener(BtnHostGame);
            btnBackToLobby.onClick.AddListener(BtnBackToLobbyPanel);
            btnServerOption.onClick.AddListener(OnBtnServerOption);
        }

        public override void OnShowed()
        {
            base.OnShowed();

            LoadMap();
        }

        #endregion

        #region 按钮监听
        private void OnBtnServerOption()
        {
            PanelMgr.Instance.OpenPanel<ServerOptionPanel>("");

            Close();
        }

        private void BtnHostGame()
        {
            if (MapName == null)
            {
                PanelMgr.Instance.OpenPanel<WarningTip>("","请选择地图！");
                return;
            }

            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("CreateServer");
            protocol.AddString(MapName);
            protocol.AddString(Global.Instance.gameInfo.serverOptionInfo.ServerDesc);
            NetMgr.srvConn.Send(protocol, RecvCreateServer);
        }

        private void BtnBackToLobbyPanel()
        {
            PanelMgr.Instance.OpenPanel<LobbyPanel>("");
            Close();
        }

        private void BtnChooseMap(string name)
        {
            MapName = name;
            Debug.Log(name);
        }

        private void BtnServerOption()
        {
            PanelMgr.Instance.OpenPanel<ServerOptionPanel>("");
            Close();
        }
        #endregion

        #region 网络监听

        private void RecvCreateServer(ProtocolBase protocol)
        {
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            proto.GetString(start, ref start);  //"CreateServer"
            int status = proto.GetInt(start, ref start);   //1代表成功，0失败

            if (status == 0)
            {
                PanelMgr.Instance.OpenPanel<WarningTip>("", "创建房间失败!");
            }
            else
            {
                PanelMgr.Instance.OpenPanel<WarningTip>("", "创建房间成功!");
                PanelMgr.Instance.OpenPanel<RoomPanel>("");

                Close();
            }
        }

        #endregion

        #region 辅助方法
        private void LoadMap()
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Lobby/LevelTag");
            //加载内置地图
            foreach (string levelName in MapLoader.LevelDic.Keys)
            {
                GameObject go = Instantiate(prefab);
                go.transform.GetComponentInChildren<Text>().text = levelName;    //关卡名
                go.GetComponent<Button>().onClick.AddListener(delegate () { BtnChooseMap(levelName); });//关卡名
                go.transform.SetParent(MapContent);
            }
            //加载第三方地图
            foreach (string levelName in MapLoader.CustomLevelDic.Keys)
            {
                GameObject go = Instantiate(prefab);
                go.transform.GetComponentInChildren<Text>().text = levelName;    //关卡名
                go.GetComponent<Button>().onClick.AddListener(delegate () { BtnChooseMap(levelName); });//关卡名
                go.transform.SetParent(MapContent);
            }
        }
        #endregion
    }
}
