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

        private Transform side1Content;
        private Transform side2Content;
        private Transform spectContent;

        private Transform playerPanel;

        private List<RoomPlayerInfo> infoList;
        private bool isHost = false;

        private Button btnMapName;
        private string mapName;

        private string hostName;

        private GameObject PlayerInfoTag;

        #region 生命周期
        public override void Init(params object[] args)
        {
            base.Init(args);

            //args[1]是玩家是否是主机
            isHost = (bool)args[1];

            //args[0]是服务器里的玩家信息
            if (!isHost)
            {
                infoList = (List<RoomPlayerInfo>)args[0];
            }

            hostName = (string)args[2];  //2是主机名字
            mapName = (string)args[3];   //3是地图名字

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
            side1Content = skinTrans.Find("PlayerPanel/Side1Panel/Side1Content").transform;
            side2Content = skinTrans.Find("PlayerPanel/Side2Panel/Side2Content").transform;
            spectContent = skinTrans.Find("PlayerPanel/SpectPanel").transform;
            PlayerInfoTag = Resources.Load<GameObject>("Prefabs/Lobby/PlayerInfoTag");
            btnMapName = skinTrans.Find("BtnMapName").GetComponent<Button>();

            btnStartServer.onClick.AddListener(BtnStartServer);
            btnBackToHostPanel.onClick.AddListener(BtnBackToHostPanel);
        }

        public override void OnShowed()
        {
            base.OnShowed();

            btnMapName.transform.Find("TextMapName").GetComponent<Text>().text = mapName;

            if (!isHost)
            {
                foreach (RoomPlayerInfo info in infoList)
                {
                    CreatePlayerInfoTag(side2Content, info);
                }
            }
            else
                CreatePlayerInfoTag(side1Content, new RoomPlayerInfo(hostName, 1));
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

        private void CreatePlayerInfoTag(Transform content,RoomPlayerInfo info)
        {
            Debug.Log("shuaxin");
            GameObject go = Instantiate(PlayerInfoTag, side1Content);

            go.transform.localPosition = Vector3.zero;

            go.transform.Find("TextPlayerName").GetComponent<Text>().text = info.playerName;
            go.transform.Find("TextPlayerStatus").GetComponent<Text>().text = info.status == 1 ? "Not Ready" : "Ready";
        }

        #endregion
    }
}
