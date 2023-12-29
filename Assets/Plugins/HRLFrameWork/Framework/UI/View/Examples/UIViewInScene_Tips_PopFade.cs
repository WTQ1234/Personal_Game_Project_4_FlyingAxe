using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HRL
{
    public class UIViewInScene_Tips_PopFade : UIViewInScene
    {
        public Text text;
        public Image image_bg;
        public string content;

        public Animator animator;

        public override void OnShown()
        {
            base.OnShown();
            animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("UIShown");
            }
            content = (string)this.UIInfo.GetParam("content", typeof(string));
            float scale = (float)this.UIInfo.GetParam("scale", typeof(float));
            if (scale > 1)
            {
                image_bg.gameObject.SetActive(true);
            }
            else
            {
                image_bg.gameObject.SetActive(false);
            }
            text.text = content;
        }
    }
}
