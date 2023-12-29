using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HRL;
using Sirenix.OdinInspector;
using PathCreation;

public enum CardState
{
    Attack,
    MoveBack,
    Move,
    StartMove,
}

public class CardManager : MonoSingleton<CardManager>
{
    public Card prefab_card;
    public List<Card> cards;

    public List<List<Card>> card_groups;

    public Vector3 card_pos_init;

    public CardState cardState;

    public int init_card_num = 5;
    public int turn_card_num = 1;
    public float each_turn_move_progress = 0.1f;
    public CardDetector_Create cardDetector_Create;

    [Title("scale time")]
    public float cur_time = 0f;
    public float maxStartTime = 10f;
    public float timeScale = 10f;

    [Title("Test")]
    public bool AddAttrWhenClick = false;

    protected override void Awake()
    {
        base.Awake();
        cur_time = 0f;
        set_time_scale = true;
        cardState = CardState.StartMove;
        card_pos_init = GetPointInLine(0);
        step_current = 0;
        card_last_one = null;
        _InitMove();
        _InitCardPool();
        //print(pathCreator.path.length);

        //NextTurn();
    }

    // ����ʱ��һ�����⣬��һ�����������ĺ����ع����£����¶����һ�Σ���Ҫ��ȥ��
    private bool set_time_scale = true;
    private void FixedUpdate()
    {
        if (set_time_scale)
        {
            cur_time += Time.fixedDeltaTime;
            if (cur_time > maxStartTime)
            {
                Time.timeScale = 1f;
                set_time_scale = false;
                if (cardState == CardState.StartMove)
                {
                    cardState = CardState.Move;
                }
            }
            else
            {
                Time.timeScale = Mathf.Lerp(timeScale, 1f, cur_time / maxStartTime);
                if (cur_time > maxStartTime * 0.8f)
                {
                    if (cardState == CardState.StartMove)
                    {
                        cardState = CardState.Move;
                    }
                }
            }
        }
        if (cardDetector_Create.CanCreate)
        {
            _SummonCard(turn_card_num);
            NextTurn();
        }
        if (cardState == CardState.MoveBack)
        {
            if (current_back_card != null)
            {
                _MoveCard(current_back_card, 3f * Time.fixedDeltaTime * each_turn_move_progress, false);
            }
            else
            {
                MoveCardBackFinish(current_back_card);
            }
        }
        else if (cardState == CardState.Move || cardState == CardState.StartMove)
        {
            if (card_last_one != null)
            {
                _MoveCard(card_last_one, Time.fixedDeltaTime * each_turn_move_progress, true);
            }
        }
    }

    #region Get Card
    public Card GetCardByStep(int step)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            if (card.step == step)
            {
                return card;
            }
        }
        return null;
    }

    public Card GetFirstCard()
    {
        return GetCardByStep(1);
    }

    public Card GetLastCard()
    {
        return card_last_one;
    }
    #endregion

    #region Summon Card
    [Title("SummonCard"), ReadOnly]
    public Card card_last_one = null;
    [ReadOnly]
    public int step_current = 0;
    public int count_current = 0;
    public int count_max = 60;
    public void _SummonCard(int summon_card_num)
    {
        if (count_max < count_current)
        {
            return;
        }
        float current_level = 0.5f + count_current / 20f;
        LogHelper.Info("Card", $"summon card, level {current_level}");
        float current_value = current_level - (int)current_level;
        CardInfo info = null;
        LogHelper.Info("Card", $"summon card, current_value {current_value}");

        if (Random.Range(0f, 1f) < current_value)
        {
            info = GetCurCardPool((int)current_level);
        }
        if (info == null)
        {
            info = GetCurCardPool((int)current_level + 1);
        }
        if (info == null)
        {
            for (int i = (int)current_level; i > 0; i--)
            {
                info = GetCurCardPool(i);
                if (info != null)
                {
                    break;
                }
            }
        }
        if (info == null)
        {
            Debug.LogError("Can not get info");
            return;
        }
        Card card = Instantiate(prefab_card);
        card.transform.position = card_pos_init;
        card.SetEntityInfo(info);
        cards.Add(card);


        count_current++;
        step_current++;
        card.SetStep(step_current);
        card.teamController.SetTeam((int)CardTeam.Enemy);
        cardDetector_Create.cur_card = card;
        // ��󱻴����������ƶ���ǰ
        card_last_one = card;
    }

    private Dictionary<int, CardInfo> dict_card;
    public Dictionary<int, List<CardInfo>> cardInfos;
    private void _InitCardPool()
    {
        cardInfos = new Dictionary<int, List<CardInfo>>();
        dict_card = ConfigManager.Instance.GetAllInfo<CardInfo>();
        foreach(var kv in dict_card)
        {
            if (kv.Value.test_disable)
            {
                continue;
            }
            int level = kv.Value.level;
            if (!cardInfos.ContainsKey(level))
            {
                cardInfos.Add(level, new List<CardInfo>());
            }
            cardInfos[level].Add(kv.Value);
        }
    }

    private CardInfo GetCurCardPool(int level)
    {
        if (!cardInfos.ContainsKey(level))
        {
            return null;
        }
        CardInfo info = cardInfos[level][Random.Range(0, cardInfos[level].Count)];
        return info;
    }
    #endregion

    #region Fight
    [Button("NextTurn")]
    public void NextTurn()
    {
        if (cardState == CardState.Attack)
        {
            return;
        }
        if (cardState == CardState.StartMove)
        {
            return;
        }
        cardState = CardState.Attack;
        // ��������CardΪEnable
        EnableCardAI();

        // ���Э�̣�����״̬����ս���������ټ��һ�Σ���μ��
        DetectCardFight();
    }

    public void DetectCardFight()
    {
        //print("Start DetectCardFight");
        // ���ս��
        for (int i = 0; i < cards.Count; i++)
        {
            // 0 �������
            Card cur_card = cards[i];
            //print($"try detect {cur_card.name} {cur_card.ai.current_state}");
            switch (cur_card.ai.current_state)
            {
                case MobAIState.Disable:
                case MobAIState.Success:
                case MobAIState.Fail:
                case MobAIState.Error:
                    // �˿�Ƭ�Ѿ�����������
                    break;
                case MobAIState.Running:
                    // �˴�Running��ʾ����ִ�ж��������صȴ�
                    return;
                case MobAIState.Enable:
                    // ������⣬Tick������ı�״̬
                    cur_card.ai.Tick();
                    switch (cur_card.ai.current_state)
                    {
                        case MobAIState.Disable:
                        case MobAIState.Success:
                        case MobAIState.Fail:
                            // ����ս�����򱻶����
                            break;
                        // �˴�Running��ʾ��ִ��ս�����������صȴ�
                        case MobAIState.Running:
                            return;
                        // ����
                        case MobAIState.Error:
                            Debug.LogError("AI is error");
                            return;
                    }
                    break;
            }
            //print($"after detect {cur_card.name} {cur_card.ai.current_state}");
        }
        AfterAllCardFight();
    }

    private void AfterAllCardFight()
    {
        LogHelper.Info("Attack", $"AfterAllCardFight");

        cardState = CardState.Move;
        _DetectSuccess();
    }

    public void EnableCardAI()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            card.ai.SetAIEnable();
        }
    }

    public void RefreshCardStep()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            card.SetStep(i);
            step_current = i;
        }
    }

    public void OnCardDie(Card card)
    {
        print($"on card die {cardState} {card} {current_back_card}");
        if ((cardState == CardState.MoveBack) && (card == current_back_card))
        {
            MoveCardBackFinish(card);
        }
        cards.Remove(card);
        RefreshCardStep();
    }

    private void _DetectSuccess()
    {
        if (count_max < count_current)
        {
            if (cards.Count == 0)
            {
                UIManager.Instance.PushScreen<UIScreen_Win>(new UIInfo());
            }
            else
            {
                foreach (Card card in cards)
                {
                    if (card != null)
                    {
                        if (card.teamController.teamId == (int)CardTeam.Enemy)
                        {
                            return;
                        }
                    }
                }
                UIManager.Instance.PushScreen<UIScreen_Win>(new UIInfo());
            }
        }
    }
    #endregion

    #region Move
    public Card current_back_card;
    public int curMovingBackCardStep;
    public float curMovingBackTimes;

    private void _InitMove()
    {
        current_back_card = null;
        curMovingBackCardStep = 0;
        curMovingBackTimes = 1.3f;
    }

    public void _MoveCard(Card start_card, float speed, bool isForward = true)
    {
        Card last_card = null;
        if (isForward)
        {
            for (int i = start_card.step; i >= 0; i--)
            {
                if (i >= cards.Count || i < 0)
                {
                    continue;
                }
                Card card = cards[i];
                if (last_card == null)
                {
                    card.SetTargetPos(card.target_pos_in_line + speed);
                }
                else
                {
                    if (last_card._next_move == card)
                    {
                        card.SetTargetPos(card.target_pos_in_line + speed);
                    }
                }
                last_card = card;
            }
        }
        else
        {
            //print($"push back {start_card} {start_card.step - 1}");
            // �ж������Ѿ����ɵĿ�Ƭ��������
            for (int i = start_card.step; i >= 0; i--)
            {
                Card card = GetCardByStep(i);
                if (card == null)
                {
                    continue;
                }
                if (last_card == null)
                {
                    //print($"push back 1 {card}");
                    card.SetTargetPos(card.target_pos_in_line + speed * -1 * curMovingBackTimes);
                }
                else
                {
                    if (last_card != null)
                    {
                        if (last_card._next_move == card)
                        {
                            //print($"push back 2 {card}");
                            card.SetTargetPos(card.target_pos_in_line + speed * -1 * curMovingBackTimes);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (card.target_pos_in_line < 0)
                {
                    card.target_pos_in_line = 0;
                    break;
                }
                last_card = card;
            }
            // �ж��ƶ������ɵĿ�Ƭ��������
            last_card = start_card;
            for (int i = start_card.step + 1; i < cards.Count; i++)
            {
                Card card = GetCardByStep(i);
                if (card == null)
                {
                    continue;
                }
                // ������ܹ��ƶ��ģ���ô�������
                if (last_card != null)
                {
                    if (last_card._prev_move == card)
                    {
                        //print($"push back 3 {card}");
                        card.SetTargetPos(card.target_pos_in_line + speed * -1 * curMovingBackTimes);
                    }
                    else if (last_card._prev_move == null)
                    {
                        // ����м��ϣ���ôbreak
                        break;
                    }
                }
                if (card.target_pos_in_line < 0)
                {
                    card.target_pos_in_line = 0;
                    break;
                }
                last_card = card;
            }
        }
    }

    public void MoveCardBack(Card card)
    {
        card.isMovingBack = true;
        cardState = CardState.MoveBack;
        curMovingBackCardStep = card.step;
        current_back_card = card;
        // ���ƻ��˵��ٶ�
        curMovingBackTimes *= 2f;
    }

    public void MoveCardBackFinish(Card card)
    {
        if (card != null)
        {
            card.isMovingBack = false;
        }
        current_back_card = null;
        curMovingBackCardStep = 0;
        cardState = CardState.Move;
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            Card cur_card = cards[i];
            if (cur_card._teamController.teamId == (int)CardTeam.Player)
            {
                if ((card != null) && (card._next_move != cur_card) && (cur_card._prev_move == null))
                {
                    MoveCardBack(cur_card);
                    return;
                }
            }
            cur_card.isMovingBack = false;
        }
        curMovingBackTimes = 1.3f;
        DetectCardFight();
    }

    [Button("TestMovingBack")]
    public void TestMovingBack()
    {
        MoveCardBack(GetFirstCard());
    }
    #endregion

    #region Line
    public int card_count_max = 20;
    public Vector2 card_pos_offset;

    public PathCreator pathCreator;

    public Vector2 GetPointInLine(float t)
    {
        return pathCreator.path.GetPointAtDistance(pathCreator.path.length - t, EndOfPathInstruction.Stop);
    }

    public int max_i = 200;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // ���û�����ɫΪ��ɫ  
        //var poses = GetBeadPositions();
        //for(int i = 0; i < poses.Length; i++)
        //{
        //    Gizmos.DrawSphere(poses[i], 0.5f);
        //}
        for (float i = 0; i < max_i; i++)
        {
            Vector2 vec = GetPointInLine(i);
            //print($"{vec} {i}");
            Gizmos.DrawSphere(vec, 0.4f); // ���������ϵĵ㣬���Ը�����Ҫ��������뾶  
        }
    }
    #endregion
}