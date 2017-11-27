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

        #region 声明周期
        public override void Init(params object[] args)
        {
            base.Init(args);

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
    }
}
