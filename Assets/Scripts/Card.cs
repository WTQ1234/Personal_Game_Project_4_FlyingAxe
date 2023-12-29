using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HRL;
using Sirenix.OdinInspector;
using DG.Tweening;

public enum CardTeam
{
    Enemy = 1,
    Player = 2,
}

public class Card : Entity
{
    public CardAI ai;

    public CardInfo card_info;
    public CardDetector_Card cardDetector_Card;

    [Title("Outline")]
    public SpriteRenderer outline;
    public Color outline_green;
    public Color outline_red;

    public SpriteRenderer CardSprite;
    public SpriteRenderer CardBgSprite;

    public static string damage = "Damage";
    public static string defense = "Defense";
    public static string health = "Health";

    protected override void Awake()
    {
        base.Awake();
        _InitAttr();
    }

    private void Start()
    {
        CardBgSprite.color = card_info.color;
    }

    protected void LateUpdate()
    {
        //_CheckMove();
    }

    #region Move
    [Title("Line")]
    public int step = 0;    // step 表示当前的序列
    public Card _next;
    public Card _prev;
    // 检测范围较小，用于推动卡片移动
    public Card _next_move;
    public Card _prev_move;
    public float curr_pos_in_line = 0;
    public float target_pos_in_line = 0;
    public float movement;
    public bool isMovingBack = false;

    public void SetStep(int _step)
    {
        step = _step;
        transform.name = $"card_{step}";
    }

    public void SetTargetPos(float target_pos_in_line)
    {
        this.target_pos_in_line = target_pos_in_line;
        Vector2 pos = CardManager.Instance.GetPointInLine(target_pos_in_line);
        transform.position = pos;
    }

    public float GetTargetPos()
    {
        return target_pos_in_line;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //_CheckMoveToNextPos(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //_CheckMoveToNextPos(collision);

        _CheckConnectOtherCard(collision);
    }

    private void OnTriggerExit2D(Collider2D collider2D)
    {
        _CheckDisconnectOtherCard(collider2D);
    }

    private void _CheckConnectOtherCard(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }
        if (!collision.CompareTag("Card"))
        {
            return;
        }
        Card card = collision.GetComponent<Card>();
        if (card == null)
        {
            return;
        }
        // 另一张卡片也会触发一次，所以无需考虑+1情况，判断这个以防止轨道内产生非预期的碰撞
        if (card.step == step - 1)
        {
            card._prev = this;
            this._next = card;
        }
    }

    private void _CheckDisconnectOtherCard(Collider2D collider2D)
    {
        if (collider2D == null)
        {
            return;
        }
        if (!collider2D.CompareTag("Card"))
        {
            return;
        }
        Card card = collider2D.GetComponent<Card>();
        if (card == null)
        {
            return;
        }
        _DisconnectOtherCard(card);
    }

    private void _DisconnectOtherCardOnDeath()
    {
        if (_next != null)
        {
            _DisconnectOtherCard(_next);
        }
        if (_prev != null)
        {
            _DisconnectOtherCard(_prev);
        }
    }

    private void _DisconnectOtherCard(Card other_card)
    {
        if (this._next == other_card)
        {
            this._next = null;
        }
        if (this._prev == other_card)
        {
            this._prev = null;
        }
        if (other_card._next == this)
        {
            other_card._next = null;
        }
        if (other_card._prev == this)
        {
            other_card._prev = null;
        }
    }

    public float maxDistance = 2.5f;

    // 每个卡片独立，第一个移动时会推动第二个，先生成的卡片step更小，更靠前，是其他卡片的next
    // 目前的问题是，一张卡可以推动，但是不能拉动，也就是向后回滚的时候，不能拉动前面的卡片
    // 当向后回滚的时候，这个球需要同时能将球推，并且拉，也就是说能区分

    // TODO: 还没有处理回滚的碰撞,不同阵营的也应该能够一起组成卡组，但是
    // 1.方案1 一起向前推动 主要依靠击杀来打断序列 先做这个
    // 2.方案2 能够阻挡，
    // 3，属性大于才能阻挡 感觉这个比较好
    private void _CheckMove()
    {
        if (curr_pos_in_line != target_pos_in_line)
        {
            float move_scale = movement * Time.deltaTime * Mathf.Clamp(Mathf.Abs(curr_pos_in_line - target_pos_in_line), 0.2f, 2f);
            if (curr_pos_in_line > target_pos_in_line)
            {
                curr_pos_in_line -= move_scale;
                if (curr_pos_in_line < target_pos_in_line)
                {
                    curr_pos_in_line = target_pos_in_line;
                }
            }
            else
            {
                curr_pos_in_line += move_scale;
                if (curr_pos_in_line > target_pos_in_line)
                {
                    curr_pos_in_line = target_pos_in_line;
                }
            }
            if (Mathf.Abs(curr_pos_in_line - target_pos_in_line) < 0.001f)
            {
                curr_pos_in_line = target_pos_in_line;
            }
            Vector2 pos = CardManager.Instance.GetPointInLine(curr_pos_in_line);
            //transform.position = Vector3.Lerp(transform.position, pos, 0.5f);
            transform.position = pos;
        }
    }
    #endregion

    #region Team
    public void OnClick()
    {
        if (teamController.teamId == (int)CardTeam.Enemy)
        {
            teamController.SetTeam((int)CardTeam.Player);
            CardManager.Instance.NextTurn();
        }
    }

    public override void _OnSetTeam()
    {
        base._OnSetTeam();
        _RefreshTeamShow();
        _OnTeamChange();
        // 方便测试用
        if (CardManager.Instance.AddAttrWhenClick && !isEnemy())
        {
            var attr = attrController.GetAttr<MobAttrBase>("Damage");
            attr.current_value += 10;
            _OnRefreshAttrShow();
        }
    }

    private void _RefreshTeamShow()
    {
        if (teamController.teamId == (int)CardTeam.Enemy)
        {
            outline.color = outline_red;
        }
        else if (teamController.teamId == (int)CardTeam.Player)
        {
            outline.color = outline_green;
        }
    }

    private void _OnTeamChange()
    {
        // 断链，不立刻战斗，而是操作后的回合进行战斗
        // 检测指定半径内是否有触发器
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useTriggers = true;

        int cardLayer = LayerMask.NameToLayer("Card"); // 通过层的名称获取层的整数序号
        contactFilter2D.layerMask = 1 << cardLayer; // 使用位运算将整数序号转换为LayerMask

        Collider2D[] colliders = new Collider2D[16];
        int num = Physics2D.GetContacts(rb2D, contactFilter2D, colliders);
        if (num > 0)
        {
            //for (int i = 0; i < num; i++)
            //{
            //    Collider2D collider = colliders[i];
            //    if (!collider.CompareTag("Card"))
            //    {
            //        continue;
            //    }
            //    Card other_card = collider.GetComponent<Card>();
            //    if (other_card == null)
            //    {
            //        continue;
            //    }
            //    if (other_card.step == step - 1)
            //    {
            //        if (other_card.teamController.DetectTeam(teamController.teamId))
            //        {
            //            // 同一队伍，组成一个组
            //            other_card._prev = this;
            //            this._next = other_card;
            //        }
            //        else
            //        {
            //            // 解绑
            //            _DisconnectOtherCard(other_card);
            //        }
            //    }
            //    else if (other_card.step == step + 1)
            //    {
            //        if (other_card.teamController.DetectTeam(teamController.teamId))
            //        {
            //            // 同一队伍，组成一个组
            //            other_card._next = this;
            //            this._prev = other_card;
            //        }
            //        else
            //        {
            //            // 解绑
            //            _DisconnectOtherCard(other_card);
            //        }
            //    }
            //}
            foreach (Collider2D collider in colliders)
            {

            }
        }
    }

    public bool isEnemy()
    {
        return teamController.teamId == (int)CardTeam.Enemy;
    }

    public bool isEnemy(Card card)
    {
        return teamController.teamId != card.teamController.teamId;
    }
    #endregion

    #region Fight
    [Title("Fight")]
    public UIViewInScene prefab_hurt;
    public GameObject prefab_cfx_hurt;
    [Button("Fight")]
    public void Attack(Card other_card, bool fight_back = false, bool is_move_back = false)
    {
        LogHelper.Info("Attack", $"{transform.name} - {teamController.teamId}  // {other_card.name} - {other_card.teamController.teamId} {fight_back}");
        //if (animator)
        //{
        //    animator.SetBool("Attack", true);
        //}
        float attack_time = 0.3f;
        Vector3 dir = other_card.transform.position - transform.position;
        if (is_move_back)
        {
            _Attack(other_card);
            // 撞击会掉1滴血，防止直接撞穿所有 TODO： 这样不太好
            Damage(1, false, this);
            _AfterAttackByMovingBack(other_card);
        }
        else
        {
            var tween = DOTween.Punch(() => transform.position, pos => transform.position = pos, dir * 0.5f, attack_time, 2, 0.2f);
            tween.onComplete = () =>
            {
                _Attack(other_card);
                // 如果这次攻击不是反击，也就是说需要让受攻击者反击一次。
                if (!fight_back)
                {
                    // 主动攻击结束需要重置AI
                    AfterAttack();
                    // 重置AI后再次检查
                    if (other_card.playerHealth.isDied)
                    {
                        // 此时已经走完了Killed，但并未销毁
                        // 如果是玩家阵营，需要回退
                        if (((other_card == this._prev_move) || (this._prev_move == null)) && (teamController.teamId == (int)CardTeam.Player))
                        {
                            isMovingBack = true;
                            CardManager.Instance.MoveCardBack(this);
                            //SetTargetPos(CardManager.Instance.GetCardByStep(step + 1).GetTargetPos());
                        }
                        else
                        {
                            CardManager.Instance.DetectCardFight();
                        }
                    }
                    else
                    {
                        other_card.Attack(this, true, is_move_back);
                    }
                }
                // 是反击，这次攻击后不需要在攻击了
                else
                {
                    // 如果不是回滚后的撞击，那么就再检测其他卡片的战斗
                    CardManager.Instance.DetectCardFight();
                }
            };
        }
    }

    private void _Attack(Card other_card)
    {
        this.abilityController.OnSetEntityTarget(other_card);
        this.abilityController.GetMobAbilityByName<PlayerAbility_Attack>("Attack")?.OnAbilityCall();
    }

    private void _AfterAttackByMovingBack(Card other_card)
    {
        if (other_card.playerHealth.isDied)
        {
            // 撞击杀死了敌方卡片，那么需要继续向回走
        }
        else
        {
            // 撞击没有杀死敌方卡片，那么就不用动了
            CardManager.Instance.MoveCardBackFinish(this);
        }
    }

    private void AfterAttack()
    {
        ai.OnAnimDone();
    }

    public override void Damage(int damage = 1, bool knock_back = false, Entity entity_damage_from = null)
    {
        CardInfo info = (CardInfo)entity_damage_from.GetEntityInfo();
        if (info == null)
        {
            Debug.LogError("can not get card info");
            return;
        }
        float damage_scale = card_info.GetDamageScale(info, out CardCounterType counterType);
        var res_damage = Mathf.RoundToInt(damage_scale * damage);
        if ((res_damage == damage) && (counterType == CardCounterType.Advantage))
        {
            damage++;
        }
        else
        {
            damage = res_damage;
        }
        UIInfo screen_ui_info = new UIInfo();
        screen_ui_info.RegisterParam("content", damage.ToString());
        screen_ui_info.RegisterParam("scale", damage_scale);
        UIManager.Instance.CreateSceneUI(prefab_hurt, transform.position, true, screen_ui_info);

        GameObject cfx_hurt = GameObject.Instantiate(prefab_cfx_hurt, transform.position, Quaternion.identity);
        cfx_hurt.transform.parent = transform;

        playerHealth.Damage(damage, knock_back, entity_damage_from);
        LogHelper.Info("Attack", $"Damage {damage} {playerHealth.current_value}, scale is {damage_scale}");
        view_show_attr.OnShowHealth(playerHealth.current_value);
    }

    public override void Killed()
    {
        LogHelper.Info("Attack", $"Killed");
        _DisconnectOtherCardOnDeath();
        cardDetector_Card._DisconnectOtherCardOnDeath();
        CardManager.Instance.OnCardDie(this);
        Destroy(gameObject);
    }

    public void OnKilledAnimDone()
    {

    }
    #endregion

    #region Attr
    [Title("Attr"), ReadOnly]
    public UIViewInScene_ShowAttr view_show_attr = null;

    private void _InitAttr()
    {
        Transform trans_attr = transform.Find("Logic/Attr");
        MobAttrBase[] attrs = trans_attr.GetComponents<MobAttrBase>();
        foreach (MobAttrBase attr in attrs)
        {
            attrController.SetAttr<MobAttrBase>(attr.Name, attr);
        }
        _OnRefreshAttrShow();
    }

    protected override void _AfterRegisterFollowUI(UIViewInScene uiView)
    {
        view_show_attr = (UIViewInScene_ShowAttr)uiView;
        view_show_attr.Owner = this;
        _OnRefreshAttrShow();
    }

    private void _OnRefreshAttrShow()
    {
        if (view_show_attr == null)
        {
            return;
        }
        if (card_info != null)
        {
            view_show_attr.OnShowType(card_info.CardType);
        }
        var attr_damage = attrController.GetAttr<MobAttrBase>(damage);
        if (attr_damage != null)
        {
            view_show_attr.OnShowAtk(attr_damage.current_value);
        }
        var attr_defense = attrController.GetAttr<MobAttrBase>(defense);
        if (attr_defense != null)
        {
            view_show_attr.OnShowDefense(attr_defense.current_value);
        }
        var attr_health = attrController.GetAttr<MobAttrBase>(health);
        if (attr_health != null)
        {
            view_show_attr.OnShowHealth(attr_health.current_value);
        }
    }
    #endregion

    #region Anim
    public void OnAttackAnimOver()
    {
        CardManager.Instance.DetectCardFight();
    }

    public void OnDeathAnimOver()
    {
        CardManager.Instance.DetectCardFight();
    }
    #endregion

    public override void SetEntityInfo(BasicInfo _card_info)
    {
        base.SetEntityInfo(_card_info);
        card_info = (CardInfo)_card_info;
        _OnRefreshCardInfo();
    }

    private void _OnRefreshCardInfo()
    {
        CardSprite.sprite = card_info.Icon;
        //Vector3 size = Vector3.one * card_info.size;
        //transform.localScale = size;
        // 属性显示UI在Awake中创建
        //if (view_show_attr != null)
        //{
        //    view_show_attr.transform.localScale = size;
        //}
        var attr_damage = attrController.GetAttr<MobAttrBase>(damage);
        if (attr_damage != null)
        {
            attr_damage.current_value = card_info.attr_damage;
        }
        var attr_defense = attrController.GetAttr<MobAttrBase>(defense);
        if (attr_defense != null)
        {
            attr_defense.current_value = card_info.attr_defense;
        }
        var attr_health = attrController.GetAttr<MobAttrBase>(health);
        if (attr_health != null)
        {
            attr_health.current_value = card_info.attr_health;
        }
        _OnRefreshAttrShow();
    }
}
