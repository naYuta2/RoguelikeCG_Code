using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu]
public class CardData : ScriptableObject
{
    [Header ("識別番号")]
    public int cardID;
    [Header ("名前")]
    public string cardName;
    [Header ("コスト")]
    public int cardCost;

    public List<CardEffectDefine> effectList = new List<CardEffectDefine>();

    [Header ("手札番号")]
    public int hand_ID;
    [Header ("レアリティ")]
    public int cardRarelity;
}