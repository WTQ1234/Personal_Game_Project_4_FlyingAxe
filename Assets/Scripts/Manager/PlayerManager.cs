using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HRL;
using Sirenix.OdinInspector;
using PathCreation;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    [Title("Times")]
    public float max_magic_time;
    public float cur_magic_time;
    [Title("CD")]
    public float cd_magic_time;
    public float cur_cd_magic_time;
    [Title("UI")]
    public UI_Comp_PlayerSlider slider;

    protected override void Init()
    {
        base.Init();
        cur_magic_time = 1;
        cur_cd_magic_time = 0;
    }

    private void LateUpdate()
    {
        if (CardManager.Instance.cardState == CardState.Move)
        {
            cur_cd_magic_time += Time.deltaTime;
            if (cur_cd_magic_time > cd_magic_time)
            {
                if (cur_magic_time >= max_magic_time)
                {
                    return;
                }
                cur_magic_time++;
                cur_cd_magic_time = 0f;
                if (cur_magic_time >= max_magic_time)
                {
                    cur_magic_time = max_magic_time;
                }
            }
            slider.RefreshSliderShow(cur_cd_magic_time / cd_magic_time, cur_magic_time);
        }
    }

    public bool CanUseMagic()
    {
        return cur_magic_time > 0;
    }

    public bool CostMagic()
    {
        if (cur_magic_time <= 0)
        {
            return false;
        }
        else
        {
            cur_magic_time--;
            slider.RefreshSliderShow(cur_cd_magic_time / cd_magic_time, cur_magic_time);
            return true;
        }
    }
}
