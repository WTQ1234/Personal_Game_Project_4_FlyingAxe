using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HRL;
using Sirenix.OdinInspector;

// 用于近战攻击，启用一个碰撞盒进行检测，碰撞则触发攻击
public class PlayerAbility_Attack : MobAbilityBase
{
    [HideInInspector]
    public Weapon weapon;

    public bool execute_by_anim = true;
    [ShowIf("execute_by_anim")]
    public Animator animator;
    [ShowIf("execute_by_anim")]
    public Collider2D collider2d;

    public int damage;

    protected override void _SetName()
    {
        Name = "Attack";
    }

    protected override void OnAbilityEnable()
    {
        base.OnAbilityEnable();
        if (execute_by_anim)
        {
            EndAttackDetect();
        }
    }

    protected override bool _Execute()
    {
        base._Execute();
        if (execute_by_anim)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            Entity target = AbilityController.OnGetEntityTarget();
            _CauseDamage(target);
        }
        return true;
    }

    #region Anim Attack
    public void StartAttackDetect()
    {
        if (!execute_by_anim)
        {
            LogHelper.Warning("Ability", "Player Ability Attack is not execute by anim, try change setting");
            return;
        }
        collider2d.enabled = true;
    }

    public void EndAttackDetect()
    {
        if (!execute_by_anim)
        {
            LogHelper.Warning("Ability", "Player Ability Attack is not execute by anim, try change setting");
            return;
        }
        collider2d.enabled = false;
        animator.ResetTrigger("Attack");
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!execute_by_anim)
        {
            return;
        }
        if (!collider2d.enabled) return;
        if (collision.tag == "SceneEntity")
        {
            SceneEntity sceneEntity = collision.GetComponent<SceneEntity>();
            // TODO: 其实SceneEntity也可以考虑统合到team里
            if (sceneEntity == null) return;
            _CauseDamage(sceneEntity);
        }
        else if (collision.tag == "Player")
        {
            Entity entity = collision.GetComponent<Entity>();
            if (entity == null) return;
            TeamController teamController = entity.teamController;
            int teamId = teamController.teamId;
            if (!Owner.teamController.DetectTeam(teamId))
            {
                _CauseDamage(entity);
            }
        }
    }
    #endregion

    protected void _CauseDamage(Entity target)
    {
        MobAttrBase attr_atk = Owner.attrController.GetAttr<MobAttrBase>("Damage");
        MobAttrBase attr_defense = Owner.attrController.GetAttr<MobAttrBase>("Defense");
        if (attr_atk != null)
        {
            if (attr_defense != null)
            {
                target.Damage(attr_atk.current_value - attr_defense.current_value, true, Owner);
            }
            else
            {
                target.Damage(attr_atk.current_value, true, Owner);
            }
        }
        else
        {
            target.Damage(damage, true, Owner);
        }
        weapon?.OnCostDurable(1);
    }
}
