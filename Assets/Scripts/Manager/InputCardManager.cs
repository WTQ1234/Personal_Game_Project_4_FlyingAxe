using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCardManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CardManager.Instance.cardState == CardState.Move)
            {
                if (PlayerManager.Instance.CanUseMagic())
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("CardDetector"))
                        {
                            CardDetector_Card cardDetector_Card = hit.collider.GetComponent<CardDetector_Card>();
                            if (cardDetector_Card != null)
                            {
                                if (PlayerManager.Instance.CostMagic())
                                {
                                    cardDetector_Card.Owner.OnClick();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
