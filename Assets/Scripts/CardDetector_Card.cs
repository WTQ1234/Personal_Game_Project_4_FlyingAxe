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
        // 另一张卡片也会触发一次，所以无需考虑+1情况，判断这个以防止轨道内产生非预期的碰撞
        if (cardDetector.Owner.step == Owner.step - 1)
        {
            // 暂时不考虑推动和碰撞阻挡问题，都组成卡组
            //if (card.teamController.DetectTeam(teamController.teamId))
            //{
            //}
            // 同一队伍，组成一个组
            cardDetector.Owner._prev_move = this.Owner;
            this.Owner._next_move = cardDetector.Owner;

            if (cardDetector.Owner.isMovingBack)
            {
                if (this.Owner.isEnemy(cardDetector.Owner))
                {
                    // 不需要反击，所以fight_back为true, 不需要再多检测其他卡片，所以is_move_back为true
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
                    // 不需要反击，所以fight_back为true, 不需要再多检测其他卡片，所以is_move_back为true
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
