using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    public class RoomPanel : PanelBase
    {
        private Button btnStartServer;
        private Button btnBackToHostPanel;

        private Transform playerPanel;

        private List<RoomPlayerInfo> infoList;
        private bool isHost = false;

        private Text MapName;

        #region 生命周期
        public override void Init(params object[] args)
        {
            base.Init(args);

            //args[2]是玩家是否是主机
            isHost = (bool)args[2];

            //args[1]是服务器里的玩家信息
            if (!isHost)
                infoList = (List<RoomPlayerInfo>)args[1];

            skinPath = "Panel/MainMenu/RoomPanel";
            layer = PanelLayer.Panel;
        }

        public override void OnShowing()
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            btnStartServer = skinTrans.Find("BtnStartServer").GetComponent<Button>();
            btnBackToHostPanel = skinTrans.Find("BtnBackToHostPanel").GetComponent<Button>();
            playerPanel = skinTrans.Find("PlayerPanel").transform;
            btnStartServer.onClick.AddListener(BtnStartServer);
            btnBackToHostPanel.onClick.AddListener(BtnBackToHostPanel);
        }

        public override void OnShowed()
        {
            base.OnShowed();

            if (!isHost)
            {
                foreach (RoomPlayerInfo info in infoList)
                {
                    CreatePlayerInfoTag(info);
                }
            }
        }
        #endregion

        #region 按钮监听
        private void BtnStartServer()
        {
            Debug.Log("Start Server");
        }

        private void BtnBackToHostPanel()
        {
            PanelMgr.Instance.OpenPanel<HostPanel>("");
            Close();
        }
        #endregion

        #region 辅助方法

        private void CreatePlayerInfoTag(RoomPlayerInfo info)
        {

        }

        #endregion
    }
}
