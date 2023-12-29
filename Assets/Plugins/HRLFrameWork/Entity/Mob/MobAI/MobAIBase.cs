using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HRL
{
    public enum MobAITickType
    {
        ByTime,
        ByTurn,
    }

    public enum MobAIState
    {
        Disable,
        Enable,
        Running,
        Success,
        Fail,
        Error = 999,
    }

    public class MobAIBase<T> : MonoBehaviour where T : Entity
    {
        public T Owner;

        [DetailedInfoBox("Mob AI", "通过设置MobAITickType来设置如何触发AI的Tick，暂不支持行为树")]
        public MobAITickType tickType;

        [ReadOnly]
        public MobAIState current_state = MobAIState.Disable;

        [Title("By Time")]
        [ShowIf("@tickType == MobAITickType.ByTime")]
        public float tick_time = 0.5f;
        [ShowIf("@tickType == MobAITickType.ByTime")]
        private float tick_current_time = 0f;

        [Title("By Turn")]
        [ShowIf("@tickType == MobAITickType.ByTurn")]
        public bool ticked_this_turn = false;

        public virtual void Tick()
        {
            // 子类需要设置 current_state
            Debug.LogError("未复写AI的Tick函数, 需要设置 current_state");
            return;
        }

        protected virtual void Update()
        {
            if (tickType == MobAITickType.ByTime)
            {
                tick_current_time += Time.deltaTime;
                if (tick_current_time > tick_time)
                {
                    tick_current_time = 0;
                    Tick();
                }
            }
        }

        public virtual void SetAIEnable()
        {
            if (current_state == MobAIState.Disable)
            {
            }
            current_state = MobAIState.Enable;
        }

        public virtual void SetAIDisable()
        {
            current_state = MobAIState.Disable;
        }
    }
}