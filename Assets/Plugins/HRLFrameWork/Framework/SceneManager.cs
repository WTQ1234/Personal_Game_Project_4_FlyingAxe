using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Sirenix.OdinInspector;

// TODO SceneManager 重名
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
            Debug.Log($"场景被打开: {scene.name} {loadSceneMode}");
            isClosing = false;
            // Do sth.
        }
        private void _OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"场景被关闭: {scene.name}");
            isClosing = true;
            // Do sth.
        }



        public static void OnClickRestart()
        {
            Time.timeScale = 1;
            // 获取当前场景的名称
            string currentSceneName = SceneManager.GetActiveScene().name;
            // 重新加载当前场景
            SceneManager.LoadScene(currentSceneName);
        }
    }
}