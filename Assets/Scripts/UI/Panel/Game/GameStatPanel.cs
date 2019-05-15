using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    public class GameStatPanel : PanelBase
    {
        private Transform statPanel;
        private Text timeText;
        private Text textRound;

        private Text textCreate1;
        private Text textCreate2;
        private Text textElimination1;
        private Text textElimination2;
        private Text textDestroyed1;
        private Text textDestroyed2;
        private Text textCapture1;
        private Text textCapture2;
        private Text textEarn1;
        private Text textEarn2;
        private Text textCost1;
        private Text textCost2;
        private Text textAttack1;
        private Text textAttack2;
        private Text textAttacked1;
        private Text textAttacked2;
        private Text textKD1;
        private Text textKD2;

        private Button btnEndGame;

        #region 生命周期
        public override void Init(params object[] args)
        {
            base.Init(args);

            skinPath = "Panel/Game/GameStatPanel";
            layer = PanelLayer.Panel;
        }

        public override void OnShowing()
        {
            base.OnShowing();

            Transform skinTrans = skin.transform;
            statPanel = skinTrans.Find("StatPanel");
            timeText = skinTrans.Find("TimeText").GetComponent<Text>();
            textRound = skinTrans.Find("TextRound").GetComponent<Text>();
            textCreate1 = skinTrans.Find("StatPanel/TextCreate1").GetComponent<Text>();
            textCreate2 = skinTrans.Find("StatPanel/TextCreate2").GetComponent<Text>();
            textElimination1 = skinTrans.Find("StatPanel/TextElimination1").GetComponent<Text>();
            textElimination2 = skinTrans.Find("StatPanel/TextElimination2").GetComponent<Text>();
            textDestroyed1 = skinTrans.Find("StatPanel/TextDestroyed1").GetComponent<Text>();
            textDestroyed2 = skinTrans.Find("StatPanel/TextDestroyed2").GetComponent<Text>();
            textCapture1 = skinTrans.Find("StatPanel/TextCapture1").GetComponent<Text>();
            textCapture2 = skinTrans.Find("StatPanel/TextCapture2").GetComponent<Text>();
            textEarn1 = skinTrans.Find("StatPanel/TextEarn1").GetComponent<Text>();
            textEarn2 = skinTrans.Find("StatPanel/TextEarn2").GetComponent<Text>();
            textCost1 = skinTrans.Find("StatPanel/TextCost1").GetComponent<Text>();
            textCost2 = skinTrans.Find("StatPanel/TextCost2").GetComponent<Text>();
            textAttack1 = skinTrans.Find("StatPanel/TextAttack1").GetComponent<Text>();
            textAttack2 = skinTrans.Find("StatPanel/TextAttack2").GetComponent<Text>();
            textAttacked1 = skinTrans.Find("StatPanel/TextAttacked1").GetComponent<Text>();
            textAttacked2 = skinTrans.Find("StatPanel/TextAttacked2").GetComponent<Text>();
            textKD1 = skinTrans.Find("StatPanel/TextKD1").GetComponent<Text>();
            textKD2 = skinTrans.Find("StatPanel/TextKD2").GetComponent<Text>();

            btnEndGame = skinTrans.Find("BtnEndGame").GetComponent<Button>();
        }

        public override void OnShowed()
        {
            base.OnShowed();

            string time = CalculateTime(GameStatNotifier.Instance.StopTimer());
            timeText.text = time;
            textRound.text = GameStatNotifier.Instance.Round.ToString();
            textCreate1.text = GameStatNotifier.Instance.Create[0].ToString();
            textCreate2.text = GameStatNotifier.Instance.Create[1].ToString();
            textElimination1.text = GameStatNotifier.Instance.Elimination[0].ToString();
            textElimination2.text = GameStatNotifier.Instance.Elimination[1].ToString();
            textDestroyed1.text = GameStatNotifier.Instance.Destroyed[0].ToString();
            textDestroyed2.text = GameStatNotifier.Instance.Destroyed[1].ToString();
            textCapture1.text = GameStatNotifier.Instance.Capture[0].ToString();
            textCapture2.text = GameStatNotifier.Instance.Capture[1].ToString();
            textEarn1.text = GameStatNotifier.Instance.Earn[0].ToString();
            textEarn2.text = GameStatNotifier.Instance.Earn[1].ToString();
            textCost1.text = GameStatNotifier.Instance.Cost[0].ToString();
            textCost2.text = GameStatNotifier.Instance.Cost[1].ToString();
            textAttack1.text = Math.Round(GameStatNotifier.Instance.Attack[0]).ToString();
            textAttack2.text = Math.Round(GameStatNotifier.Instance.Attack[1]).ToString();
            textAttacked1.text = Math.Round(GameStatNotifier.Instance.BeAttack[0]).ToString();
            textAttacked2.text = Math.Round(GameStatNotifier.Instance.BeAttack[1]).ToString();
            textKD1.text = (Math.Round((float)GameStatNotifier.Instance.Attack[0] /
                (float)GameStatNotifier.Instance.BeAttack[0], 2) * 100).ToString() + "%";
            textKD2.text = (Math.Round((float)GameStatNotifier.Instance.Attack[1] /
                (float)GameStatNotifier.Instance.BeAttack[1], 2) * 100).ToString() + "%";

            btnEndGame.onClick.AddListener(BtnEndGame);
        }

        #endregion

        #region 辅助
        string CalculateTime(long time)
        {
            long hours,mins,sec;
            string s="";
            hours = time / 3600;
            mins = (time - hours * 3600)/60;
            sec = time - hours * 3600 - mins * 60;

            if (hours != 0)
                s = s + hours.ToString() + "时";
            if (mins != 0)
                s = s + mins.ToString() + "分";
            if (sec != 0)
                s = s + sec.ToString() + "秒";
            return s;
        }
        #endregion

        #region 按钮监听
        private void BtnEndGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Menu");
        }
        #endregion
    }
}