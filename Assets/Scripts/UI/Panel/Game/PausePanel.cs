using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    public class PausePanel : PanelBase
    {
        private Button btnContinue;
        private Button btnExit;
        #region 生命周期
    public override void Init(params object[] args)
        {
            base.Init(args);

            skinPath = "Panel/Game/PausePanel";
            layer = PanelLayer.Panel;
        }

        public override void OnShowing()
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            btnContinue = skinTrans.Find("BG/BtnContinue").GetComponent<Button>();
            btnExit = skinTrans.Find("BG/BtnExit").GetComponent<Button>();
        }

        public override void OnShowed()
        {
            base.OnShowed();

            btnContinue.onClick.AddListener(BtnContinue);
            btnExit.onClick.AddListener(BtnExit);
        }

        #endregion

        #region 点击事件
        private void BtnContinue()
        {
            Close();
        }
        private void BtnExit()
        {
            NetMgr.srvConn.gameNet.SendStatus(Global.Instance.gameInfo.IsHost,GameNet.GameStatus.Lose);

            GridContainer.Instance.TerrainLayer.gameObject.SetActive(false);
            GridContainer.Instance.UnitLayer.gameObject.SetActive(false);

            PanelMgr.Instance.ClosePanel("UI.Tip.WarningTip");
            PanelMgr.Instance.ClosePanel("UI.Panel.GridDetailPanel");
            PanelMgr.Instance.OpenPanel<GameStatPanel>("");
            Close();
        }
        #endregion
    }
}