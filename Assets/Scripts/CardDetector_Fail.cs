using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HRL;

public class CardDetector_Fail : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
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
        if (cardDetector.Owner == null)
        {
            return;
        }
        if (cardDetector.Owner.teamController.teamId == (int)CardTeam.Enemy)
        {
            GameFail();
        }
    }

    private void GameFail()
    {
        UIManager.Instance.PushScreen<UIScreen_Fail>(new UIInfo());
    }
}
