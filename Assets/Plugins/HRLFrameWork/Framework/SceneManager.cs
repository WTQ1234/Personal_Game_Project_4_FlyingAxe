using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Sirenix.OdinInspector;

// TODO SceneManager ����
namespace HRL
{
    public class SceneHelper : MonoSingleton<SceneHelper>
    {
        protected override void Init()
        {
            base.Init();
            isClosing = false;
            SceneManager.sceneLoaded += _OnSceneLoaded;
            SceneManager.sceneUnloaded += _OnSceneUnloaded;
        }

        protected override void OnDestroy()
        {
            SceneManager.sceneLoaded -= _OnSceneLoaded;
            SceneManager.sceneUnloaded -= _OnSceneUnloaded;
            base.OnDestroy();
        }

        public static bool isClosing = false;
        private void _OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            Debug.Log($"��������: {scene.name} {loadSceneMode}");
            isClosing = false;
            // Do sth.
        }
        private void _OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"�������ر�: {scene.name}");
            isClosing = true;
            // Do sth.
        }



        public static void OnClickRestart()
        {
            Time.timeScale = 1;
            // ��ȡ��ǰ����������
            string currentSceneName = SceneManager.GetActiveScene().name;
            // ���¼��ص�ǰ����
            SceneManager.LoadScene(currentSceneName);
        }
    }
}