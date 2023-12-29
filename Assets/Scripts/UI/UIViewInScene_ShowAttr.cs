using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HRL;

public class UIViewInScene_ShowAttr : UIViewInScene
{
    public Entity Owner;

    public Text text_atk;
    public Text text_health;
    public Text text_defense;

    public Image img_defense;
    public Image Weapon_Axe;
    public Image Weapon_Spear;
    public Image Weapon_Sword;

    public void OnShowType(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Axe:
                Weapon_Axe.gameObject.SetActive(true);
                Weapon_Spear.gameObject.SetActive(false);
                Weapon_Sword.gameObject.SetActive(false);
                break;
            case CardType.Spear:
                Weapon_Axe.gameObject.SetActive(false);
                Weapon_Spear.gameObject.SetActive(true);
                Weapon_Sword.gameObject.SetActive(false);
                break;
            case CardType.Sword:
                Weapon_Axe.gameObject.SetActive(false);
                Weapon_Spear.gameObject.SetActive(false);
                Weapon_Sword.gameObject.SetActive(true);
                break;
        }
    }

    public void OnShowAtk(int atk)
    {
        text_atk.text = atk.ToString();
    }

    public void OnShowHealth(int health)
    {
        text_health.text = health.ToString();
    }

    public void OnShowDefense(int defense)
    {
        if (defense <= 0)
        {
            img_defense.gameObject.SetActive(false);
        }
        else
        {
            img_defense.gameObject.SetActive(true);
            text_defense.text = defense.ToString();
        }
    }
}
