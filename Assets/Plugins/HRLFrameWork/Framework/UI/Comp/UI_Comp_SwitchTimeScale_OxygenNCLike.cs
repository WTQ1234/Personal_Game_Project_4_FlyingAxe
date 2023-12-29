using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HRL;
using Sirenix.OdinInspector;

namespace HRL
{
    public class UI_Comp_SwitchTimeScale_OxygenNCLike : UI_Comp_SwitchTimeScale
    {
        [Title("Pause")]
        public Button btn_pause;
        public Sprite sprite_pause;
        public Sprite sprite_continue;
        [Title("Time_1")]
        public Button btn_time_1;
        public Sprite sprite_time_1;
        public Sprite sprite_time_1_pressed;
        [Title("Time_2")]
        public Button btn_time_2;
        public Sprite sprite_time_2;
        public Sprite sprite_time_2_pressed;
        [Title("Time_3")]
        public Button btn_time_3;
        public Sprite sprite_time_3;
        public Sprite sprite_time_3_pressed;

        protected override void Init()
        {
            base.Init();
            btn_pause.onClick.AddListener(SwitchPause);
            btn_time_1.onClick.AddListener(() => { SetTimeScale(0); });
            btn_time_2.onClick.AddListener(() => { SetTimeScale(1); });
            btn_time_3.onClick.AddListener(() => { SetTimeScale(2); });
        }

        protected override void _AfterSetPause()
        {
            base._AfterSetPause();
            btn_pause.image.sprite = isPause ? sprite_pause : sprite_continue;
        }

        protected override void _AfterSetTimeScale()
        {
            base._AfterSetTimeScale();
            btn_time_1.image.sprite = (timescale_state == 0) ? sprite_time_1_pressed : sprite_time_1;
            btn_time_2.image.sprite = (timescale_state == 1) ? sprite_time_2_pressed : sprite_time_2;
            btn_time_3.image.sprite = (timescale_state == 2) ? sprite_time_3_pressed : sprite_time_3;
        }
    }
}