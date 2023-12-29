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
                // 目前是Player先攻，后续可能存在Player被冻结的情况
                if (Owner.teamController.teamId == (int)CardTeam.Player)
                {
                    Owner.Attack(Owner._next);
                    current_state = MobAIState.Running;
                    // 播放攻击动画，AI队列暂停`
                    // 接下来播放反击动画？
                    
                    // 接下来检测是否死亡，若死亡，继续播放反击动画
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
                    // 处理同上
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
