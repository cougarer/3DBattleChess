using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    public class ServerOptionPanel : PanelBase
    {
        #region 参数面板
        private InputField inputFieldServerDesc;
        #endregion

        private Button btnClose;
        private Button btnAck;

        #region 生命周期
        public override void Init(params object[] args)
        {
            base.Init(args);

            skinPath = "Panel/MainMenu/ServerOptionPanel";
            layer = PanelLayer.Panel;
        }

        public override void OnShowing()
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            btnClose = skinTrans.Find("BtnClose").GetComponent<Button>();
            btnAck = skinTrans.Find("BtnAck").GetComponent<Button>();
            inputFieldServerDesc = skinTrans.Find("InputFieldServerDesc").GetComponent<InputField>();

            btnClose.onClick.AddListener(OnBtnClose);
            btnAck.onClick.AddListener(OnBtnAck);
        }
        #endregion

        #region 按钮监听
        private void OnBtnClose()
        {
            Close();
        }

        private void OnBtnAck()
        {
            //保存参数
            SaveServerOption();

            Close();
        }
        #endregion

        #region 辅助方法

        private void SaveServerOption()
        {
            ServerOptionInfo optionInfo = new ServerOptionInfo();
            optionInfo.ServerDesc = inputFieldServerDesc.text;

            Global.Instance.gameInfo.serverOptionInfo = optionInfo;
        }

        #endregion
    }
}
