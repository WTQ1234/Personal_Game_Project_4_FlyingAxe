using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HRL;
using Sirenix.OdinInspector;

public class UI_Comp_PlayerSlider : UI_Comp_Base
{
    public Text text_times;

    public Image img_slider;

    public void RefreshSliderShow(float cur, float times)
    {
        img_slider.fillAmount = cur;

        text_times.text = times.ToString();
    }
}
