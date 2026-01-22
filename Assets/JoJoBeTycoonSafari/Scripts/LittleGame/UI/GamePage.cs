using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LittleGame
{

    public class GamePage : UIPage
    {
        private Slider scheduleSlider;

        private Text difficultyLevelText;

        public GamePage() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
        {
            uiPath = "UIPrefab/UIGame";
        }

        public override void Active()
        {
            base.Active();
            Awake();
            Start();
        }

        public void Awake()
        {
            scheduleSlider = transform.GetComponentInChildren<Slider>();
            difficultyLevelText = transform.Find("UP/MoneyGroup/Text").GetComponent<Text>();
        }

        public override void Hide()
        {
            base.Hide();
            OnDestroy();
        }

        void Start()
        {
            EventManager.Add<float>(UICompentEventName.ScheduleSlider, UpdateScheduleSlider);
            EventManager.Add<int>(UICompentEventName.LevelText, UpdateLevelText);

            Init();
        }

        private void Init()
        {
            scheduleSlider.value = 0f;
        }


        private void OnDestroy()
        {
            EventManager.Remove<float>(UICompentEventName.ScheduleSlider, UpdateScheduleSlider);
            EventManager.Remove<int>(UICompentEventName.LevelText, UpdateLevelText);
        }


        /// <summary>
        /// 更新当前的进度条
        /// </summary>
        /// <param name="schedule"></param>
        private void UpdateScheduleSlider(float schedule)
        {
            scheduleSlider.value = schedule;
        }

        /// <summary>
        /// 更新当前的关卡难度显示
        /// </summary>
        /// <param name="msg"></param>
        private void UpdateLevelText(int msg)
        {
            string level = "";
            switch (msg)
            {
                case 1:
                    level = "简单";
                    break;
                case 2:
                    level = "普通";
                    break;
                case 3:
                    level = "困难";
                    break;
                case 4:
                    level = "噩梦";
                    break;
                case 5:
                    level = "地狱";
                    break;
            }
            difficultyLevelText.text = "难度:" + level;
        }

    }
}