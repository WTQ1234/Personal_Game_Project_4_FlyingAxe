using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDetector_Card : MonoBehaviour
{
    public Card Owner;

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        _CheckConnectOtherCard(collider2D);
    }

    private void OnTriggerExit2D(Collider2D collider2D)
    {
        _CheckDisconnectOtherCard(collider2D);
    }

    private void _CheckConnectOtherCard(Collider2D collider2D)
    {
        if (collider2D == null)
        {
            return;
        }
        if (!collider2D.CompareTag("CardDetector"))
        {
            return;
        }
        CardDetector_Card cardDetector = collider2D.GetComponent<CardDetector_Card>();
        if (cardDetector == null)
        {
            return;
        }
        if (cardDetector.Owner == null)
        {
            return;
        }
        // ��һ�ſ�ƬҲ�ᴥ��һ�Σ��������迼��+1������ж�����Է�ֹ����ڲ�����Ԥ�ڵ���ײ
        if (cardDetector.Owner.step == Owner.step - 1)
        {
            // ��ʱ�������ƶ�����ײ�赲���⣬����ɿ���
            //if (card.teamController.DetectTeam(teamController.teamId))
            //{
            //}
            // ͬһ���飬���һ����
            cardDetector.Owner._prev_move = this.Owner;
            this.Owner._next_move = cardDetector.Owner;

            if (cardDetector.Owner.isMovingBack)
            {
                if (this.Owner.isEnemy(cardDetector.Owner))
                {
                    // ����Ҫ����������fight_backΪtrue, ����Ҫ�ٶ���������Ƭ������is_move_backΪtrue
                    cardDetector.Owner.Attack(this.Owner, true, true);
                }
                else
                {
                    CardManager.Instance.MoveCardBackFinish(cardDetector.Owner);
                }
            }
            if (this.Owner.isMovingBack)
            {
                if (!this.Owner.isEnemy(cardDetector.Owner))
                {
                    // ����Ҫ����������fight_backΪtrue, ����Ҫ�ٶ���������Ƭ������is_move_backΪtrue
                    this.Owner.Attack(cardDetector.Owner, true, true);
                }
                else
                {
                    CardManager.Instance.MoveCardBackFinish(this.Owner);
                }
            }
        }
    }

    private void _CheckDisconnectOtherCard(Collider2D collider2D)
    {
        if (collider2D == null)
        {
            return;
        }
        if (!collider2D.CompareTag("CardDetector"))
        {
            return;
        }
        CardDetector_Card cardDetector = collider2D.GetComponent<CardDetector_Card>();
        if (cardDetector == null)
        {
            return;
        }
        if (cardDetector.Owner == null)
        {
            return;
        }
        _DisconnectOtherCard(cardDetector.Owner);
    }

    private void _DisconnectOtherCard(Card other_card)
    {
        if (this.Owner._next_move == other_card)
        {
            this.Owner._next_move = null;
        }
        if (this.Owner._prev_move == other_card)
        {
            this.Owner._prev_move = null;
        }
        if (other_card._next_move == this.Owner)
        {
            other_card._next_move = null;
        }
        if (other_card._prev_move == this.Owner)
        {
            other_card._prev_move = null;
        }
    }

    public void _DisconnectOtherCardOnDeath()
    {
        if (this.Owner._next_move != null)
        {
            Card otherCard = this.Owner._next_move;
            if (otherCard._next_move == this.Owner)
            {
                otherCard._next_move = null;
            }
            if (otherCard._prev_move == this.Owner)
            {
                otherCard._prev_move = null;
            }
            this.Owner._next_move = null;
        }
        if (this.Owner._prev_move != null)
        {
            Card otherCard = this.Owner._prev_move;
            if (otherCard._next_move == this.Owner)
            {
                otherCard._next_move = null;
            }
            if (otherCard._prev_move == this.Owner)
            {
                otherCard._prev_move = null;
            }
            this.Owner._prev_move = null;
        }
    }
}
