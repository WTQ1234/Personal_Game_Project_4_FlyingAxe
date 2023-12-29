using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HRL;

public class CardAI: MobAIBase<Card>
{
    public override void Tick()
    {
        LogHelper.Info("AI", $"Tick {transform.name} {current_state}");
        ticked_this_turn = true;
        if (Owner == null)
        {
            current_state = MobAIState.Error;
            return;
        }
        if (current_state == MobAIState.Error)
        {
            return;
        }
        if (current_state == MobAIState.Disable)
        {
            return;
        }
        if (Owner._next)
        {
            if (!Owner.teamController.DetectTeam(Owner._next.teamController.teamId))
            {
                // Ŀǰ��Player�ȹ����������ܴ���Player����������
                if (Owner.teamController.teamId == (int)CardTeam.Player)
                {
                    Owner.Attack(Owner._next);
                    current_state = MobAIState.Running;
                    // ���Ź���������AI������ͣ`
                    // ���������ŷ���������
                    
                    // ����������Ƿ����������������������ŷ�������
                    return;
                }
            }
        }
        if (Owner._prev)
        {
            if (!Owner.teamController.DetectTeam(Owner._prev.teamController.teamId))
            {
                if (Owner.teamController.teamId == (int)CardTeam.Player)
                {
                    Owner.Attack(Owner._prev);
                    current_state = MobAIState.Running;
                    // ����ͬ��
                    return;
                }
            }
        }
        current_state = MobAIState.Success;
        return;
    }

    public void OnAnimDone()
    {
        current_state = MobAIState.Success;
    }
}
