using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    public class LobbyPanel : PanelBase
    {
        private Button btnHostGame;
        private Button btnRefresh;
        private Button btnJoin;
        private Button btnBackToMenu;
        private Transform ServerContent;

        private GameObject prefabServer;

        #region 生命周期
        public override void Init(params object[] args)
        {
            base.Init(args);
            skinPath = "Panel/MainMenu/LobbyPanel";
            layer = PanelLayer.Panel;
        }

        public override void OnShowing()
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            btnHostGame = skinTrans.Find("BtnHostGame").GetComponent<Button>();
            btnRefresh = skinTrans.Find("BtnRefresh").GetComponent<Button>();
            btnJoin = skinTrans.Find("BtnJoin").GetComponent<Button>();
            btnBackToMenu = skinTrans.Find("BtnBackToMenu").GetComponent<Button>();
            ServerContent = skinTrans.Find("Scroll View/Viewport/Content");
            prefabServer = Resources.Load<GameObject>("Prefabs/Lobby/ServerTag");

            btnHostGame.onClick.AddListener(BtnHostGame);
            btnRefresh.onClick.AddListener(BtnRefresh);
            btnJoin.onClick.AddListener(BtnJoin);
            btnBackToMenu.onClick.AddListener(BtnBackToMenu);
        }
        #endregion

        #region 按钮监听
        private void BtnHostGame()
        {
            PanelMgr.Instance.OpenPanel<HostPanel>("");

            Close();
        }
        private void BtnRefresh()
        {
            ClearServerList();

            NetMgr.srvConn.proto = new ProtocolBytes();
            NetMgr.srvConn.Connect("127.0.0.1", 1234);   //这里注意删除

            NetMgr.srvConn.msgDist.AddListener("GetServerList",RecvGetServerList);

            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("GetServerList");
            NetMgr.srvConn.Send(protocol);
            Debug.Log("Refresh");
        }
        private void BtnJoin()
        {
            Debug.Log("Join");
        }

        private void BtnBackToMenu()
        {
            PanelMgr.Instance.OpenPanel<MenuButtonsPanel>("");
            Close();
        }

        private void BtnGetAchieve()
        {

        }
        #endregion

        #region 网络监听
        private void RecvGetServerList(ProtocolBase protocol)
        {
            //解析协议
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);   //“GetServerList”
            string protoDesc = proto.GetString(start, ref start);   //服务器描述
            string protoHostName = proto.GetString(start, ref start);//服务器房主名字
            CreateServerTag(protoDesc, protoHostName);
        }
        #endregion

        #region 其他辅助函数
        private void ClearServerList()
        {
            for (int i = 0; i < ServerContent.childCount; i++)
            {
                Destroy(ServerContent.GetChild(i).gameObject);
            }
            NetMgr.srvConn.msgDist.DelListener("GetServerList", RecvGetServerList);
        }

        private void CreateServerTag(string serverDesc, string hostName)
        {
            Transform tr= Instantiate(prefabServer, ServerContent).transform;
            Text textServerDesc = tr.Find("TextServerDesc").GetComponent<Text>();
            Text textHostName = tr.Find("TextHostName").GetComponent<Text>();
            Text textServerPing = tr.Find("TextServerPing").GetComponent<Text>();

            textServerDesc.text = serverDesc;
            textHostName.text = hostName;
            tr.gameObject.GetComponent<Button>().onClick.AddListener(BtnGetAchieve);
        }
        #endregion
    }

    internal class AchieveTip:PanelBase
    {
        private Text textHostName;
        private Text textWinTimes;
        private Text textFailTimes;

        private string hostName = "";
        private string winTimes = "";
        private string failTimes = "";

        #region 声明周期
        public override void Init(params object[] args)
        {
            base.Init(args);

            skinPath = "Tip/General/AchieveTip";
            layer = PanelLayer.Tip;

            if (args.Length == 1)
            {
                hostName = (string)args[0];
                winTimes = (string)args[1];
                failTimes = (string)args[2];
            }
        }

        public override void OnShowing()
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            textHostName = skinTrans.Find("TextHostName").GetComponent<Text>();
            textWinTimes = skinTrans.Find("TextWinTimes").GetComponent<Text>();
            textFailTimes = skinTrans.Find("TextFailTimes").GetComponent<Text>();

            textHostName.text = hostName;
            textWinTimes.text = winTimes;
            textFailTimes.text = failTimes;
        }
        #endregion
    }
}
