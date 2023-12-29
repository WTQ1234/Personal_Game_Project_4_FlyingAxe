using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HRL;
using Sirenix.OdinInspector;

namespace HRL
{
    public class UI_Comp_SwitchTimeScale : UI_Comp_Base
    {
        public int timescale_state = 1;
        public bool isPause = false;

        public List<float> timescale_value;

        public void SetTimeScale(int cur_timescale_state)
        {
            timescale_state = cur_timescale_state;
            if (!isPause)
            {
                float value = _GetTimeScaleValueFromList(cur_timescale_state);
                Time.timeScale = value;
            }
            _AfterSetTimeScale();
        }

        private float _GetTimeScaleValueFromList(int state)
        {
            if (state < 0)
            {
                return 1;
            }
            if (state < timescale_value.Count)
            {
                return timescale_value[state];
            }
            else
            {
                for(int i = state; i > 0; i--)
                {
                    if (state < timescale_value.Count)
                    {
                        return timescale_value[state];
                    }
                }
            }
            return 1;
        }

        public void SetPause(bool cur_pause)
        {
            isPause = cur_pause;
            if (isPause)
            {
                Time.timeScale = 0f;
            }
            else
            {
                SetTimeScale(timescale_state);
            }
            _AfterSetPause();
        }

        public void SwitchPause()
        {
            SetPause(!isPause);
        }

        protected virtual void _AfterSetTimeScale()
        {

        }

        protected virtual void _AfterSetPause()
        {

        }
    }
}