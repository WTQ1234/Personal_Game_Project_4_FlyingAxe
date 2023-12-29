using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HRL;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class UIScreen_Win : UIScreen
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
        Time.timeScale = 1;
        // 获取当前场景的名称
        string currentSceneName = SceneManager.GetActiveScene().name;
        // 重新加载当前场景
        SceneManager.LoadScene(currentSceneName);
        UIManager.Instance.PopScreen(this);
    }
}
