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
        // ��ȡ��ǰ����������
        string currentSceneName = SceneManager.GetActiveScene().name;
        // ���¼��ص�ǰ����
        SceneManager.LoadScene(currentSceneName);
        UIManager.Instance.PopScreen(this);
    }
}
