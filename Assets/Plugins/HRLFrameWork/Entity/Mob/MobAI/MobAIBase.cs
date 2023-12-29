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

        [DetailedInfoBox("Mob AI", "ͨ������MobAITickType��������δ���AI��Tick���ݲ�֧����Ϊ��")]
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
            // ������Ҫ���� current_state
            Debug.LogError("δ��дAI��Tick����, ��Ҫ���� current_state");
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