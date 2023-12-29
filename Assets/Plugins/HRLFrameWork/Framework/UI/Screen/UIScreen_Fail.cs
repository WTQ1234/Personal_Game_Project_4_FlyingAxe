using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HRL;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class UIScreen_Fail : UIScreen
{
    public Button btn;

    protected override void Init()
    {
        base.Init();
        btn.onClick.AddListener(OnClickRestart);
    }

    public override void OnShown()
    {
        base.OnShown();
        Time.timeScale = 0f;
    }

    private void OnClickRestart()
    {
        UIManager.Instance.PopScreen(this);
        SceneHelper.OnClickRestart();
    }
}
