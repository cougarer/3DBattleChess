using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    public class GridDetailPanel : PanelBase
    {

        private RawImage img;
        private Text txt1;
        private Text txt2;
        private Text txt3;
        private Text txt4;
        private Text txt5;

        Texture m;
        string s1, s2, s3, s4, s5;
        #region 生命周期
        public override void Init(params object[] args)
        {
            base.Init(args);

            skinPath = "Panel/Game/GridDetailPanel";
            layer = PanelLayer.Panel;

            //部队  名字 血量 伤害 护甲 伤害类型
            if (args.Length == 6)
            {
                m = (Texture)args[0];
                s1 = (string)args[1];
                s2 = (string)args[2];
                s3 = (string)args[3];
                s4 = (string)args[4];
                s5 = (string)args[5];
            }
            //地形  图片 名字 防御力 油量消耗 
            else if (args.Length == 4)
            {
                m = (Texture)args[0];
                s1 = (string)args[1];
                s2 = (string)args[2];
                s3 = (string)args[3];
            }
        }

        public override void OnShowing()
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            img = skinTrans.Find("BG/Image").GetComponent<RawImage>();
            txt1 = skinTrans.Find("BG/Text1").GetComponent<Text>();
            txt2 = skinTrans.Find("BG/Text2").GetComponent<Text>();
            txt3 = skinTrans.Find("BG/Text3").GetComponent<Text>();
            txt4 = skinTrans.Find("BG/Text4").GetComponent<Text>();
            txt5 = skinTrans.Find("BG/Text5").GetComponent<Text>();
        }

        public override void OnShowed()
        {
            base.OnShowed();

            img.texture = m;
            txt1.text = s1 != null ? s1 : "";
            txt2.text = s2 != null ? s2 : "";
            txt3.text = s3 != null ? s3 : "";
            txt4.text = s4 != null ? s4 : "";
            txt5.text = s5 != null ? s5 : "";
        }
        #endregion


    }
}