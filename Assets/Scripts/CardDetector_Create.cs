using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDetector_Create : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
    }

    public bool CanCreate
    {
        get
        {
            return cur_card == null;
        }
    }

    public Card cur_card = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision == null)
        //{
        //    return;
        //}
        //if (!collision.CompareTag("Card"))
        //{
        //    return;
        //}
        //Card card = collision.GetComponent<Card>();
        //if (card == null)
        //{
        //    return;
        //}
        //print("enter");
        //cur_card_num++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }
        if (!collision.CompareTag("CardDetector"))
        {
            return;
        }
        CardDetector_Card cardDetector = collision.GetComponent<CardDetector_Card>();
        if (cardDetector == null)
        {
            return;
        }
        if (cardDetector.Owner == cur_card)
        {
            cur_card = null;
        }
        // 这里不判断队伍感觉也行
        if ((CardManager.Instance.cardState == CardState.MoveBack) && (cardDetector.Owner._teamController.teamId == (int)CardTeam.Player))
        {
            CardManager.Instance.MoveCardBackFinish(cardDetector.Owner);
        }
    }
}
