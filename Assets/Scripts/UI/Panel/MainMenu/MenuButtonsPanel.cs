using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Author: MaxLykoS
//CreatedTime: 2017.11.20

namespace UI.Panel
{
    public class MenuButtonsPanel : PanelBase
    {
        private Button btnLobby;
        private Button btnMapEditor;
        private Button optionBtn;
        private Button quitBtn;

        #region 生命周期
        public override void Init(params object[] args)
        {
            base.Init(args);
            skinPath = "Panel/MainMenu/MenuButtonsPanel";
            layer = PanelLayer.Panel;
        }

        public override void OnShowing()
        {
            base.OnShowed();
            Transform skinTrans = skin.transform;
            btnLobby = skinTrans.Find("BtnLobby").GetComponent<Button>();
            btnMapEditor = skinTrans.Find("BtnMapEditor").GetComponent<Button>();
            optionBtn = skinTrans.Find("BtnMapEditor").GetComponent<Button>();
            quitBtn = skinTrans.Find("BtnMapEditor").GetComponent<Button>();

            btnLobby.onClick.AddListener(BtnLobby);
            btnMapEditor.onClick.AddListener(BtnMapEditor);
            optionBtn.onClick.AddListener(BtnOption);
            quitBtn.onClick.AddListener(BtnQuit);
        }
        #endregion

        #region 按钮监听
        private void BtnLobby()
        {
            PanelMgr.Instance.OpenPanel<LobbyPanel>("");
            Close();
        }

        private void BtnQuit()
        {
            Application.Quit();
        }

        private void BtnOption()
        {
            Debug.LogError("Option界面未创建");
        }

        private void BtnMapEditor()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
        }
        #endregion

    }
}
