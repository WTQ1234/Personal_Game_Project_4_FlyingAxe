using System.Linq;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities;
using Sirenix.OdinInspector;
using HRL;

public enum CardType
{
    Axe,
    Spear,
    Sword,
}

public enum CardCounterType
{
    Advantage,
    Disadvantage,
    Normal,
}

[SerializeField]
public class CardInfo : BasicInfo
{
    public string CardName;

    public string CardDesc;

    public CardType CardType;
    [ColorPalette]
    public Color color;

    [PreviewField(70, ObjectFieldAlignment.Center)]
    public Sprite Icon;

    public int level = 1;
    public int attr_health = 3;
    public int attr_damage = 1;
    public int attr_defense = 0;

    public float size = 1.5f;

    private static float AdvantageDamageScale = 1.5f;
    private static float DisadvantageDamageScale = 1f;
    private static float NormalDamageScale = 1f;

    [Title("Test")]
    public bool test_disable = false;

    public float GetDamageScale(CardInfo otherinfo, out CardCounterType counterType)
    {
        switch(CardType)
        {
            case CardType.Axe:
                switch(otherinfo.CardType)
                {
                    case CardType.Sword:
                        counterType = CardCounterType.Advantage;
                        return AdvantageDamageScale;
                    case CardType.Spear:
                        counterType = CardCounterType.Disadvantage;
                        return DisadvantageDamageScale;
                    default:
                        counterType = CardCounterType.Normal;
                        return NormalDamageScale;
                }
            case CardType.Sword:
                switch (otherinfo.CardType)
                {
                    case CardType.Axe:
                        counterType = CardCounterType.Disadvantage;
                        return DisadvantageDamageScale;
                    case CardType.Spear:
                        counterType = CardCounterType.Advantage;
                        return AdvantageDamageScale;
                    default:
                        counterType = CardCounterType.Normal;
                        return NormalDamageScale;
                }
            case CardType.Spear:
                switch (otherinfo.CardType)
                {
                    case CardType.Axe:
                        counterType = CardCounterType.Advantage;
                        return AdvantageDamageScale;
                    case CardType.Sword:
                        counterType = CardCounterType.Disadvantage;
                        return DisadvantageDamageScale;
                    default:
                        counterType = CardCounterType.Normal;
                        return NormalDamageScale;
                }
            default:
                counterType = CardCounterType.Normal;
                return NormalDamageScale;
        }
    }    
}
