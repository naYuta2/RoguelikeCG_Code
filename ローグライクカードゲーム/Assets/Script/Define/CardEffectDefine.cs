using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CardEffectDefine
{
    [Header ("効果の種類")]
    public CardEffect effectType;
    [Header ("効果値")]
    public int value;
    [Header ("ターゲット")]
    public TargetType targetType;

    #region 効果の種類定義部

    public enum CardEffect{
        Damage,
        MagicDmg,
        Heal,
        ManaHeal,
        Burn,
        Bleeding,
        Poison,
        Guard,
        AttackUp,
        AttackDown,

        _MAX,
    };

    public enum TargetType{
        Self,
        SingleEnemy,
        AllEnemies,
        RandomEnemy
    };

    readonly public static Dictionary<CardEffect, string> Dic_EffectName_JP = new Dictionary<CardEffect, string>(){
        {CardEffect.Damage,
            "物理ダメージ {0}"},
        {CardEffect.MagicDmg,
            "魔法ダメージ {0}"},
        {CardEffect.Heal,
            "回復 {0}"},
        {CardEffect.ManaHeal,
            "マナ増加 {0}"},
        {CardEffect.Burn,
            "火傷 {0}"},
        {CardEffect.Bleeding,
            "出血 {0}"},
        {CardEffect.Poison,
            "毒 {0}"},
        {CardEffect.Guard,
            "ガード {0}"},
        {CardEffect.AttackUp,
            "攻撃力上昇 {0}"},
        {CardEffect.AttackDown,
            "攻撃力減少 {0}"},
    };
    readonly public static Dictionary<CardEffect, string> Dic_EffectExplain_JP = new Dictionary<CardEffect, string>(){
        {CardEffect.Damage,
            "相手に {0} の物理ダメージを与える"},
        {CardEffect.MagicDmg,
            "相手に {0} の魔法ダメージを与える"},
        {CardEffect.Heal,
            "自分のHPを {0} 回復する"},
        {CardEffect.ManaHeal,
            "このターンに使用可能なマナを {0} 増やす"},
        {CardEffect.Burn,
            "ターン終了時に {0} だけダメージを受けて消滅する"},
        {CardEffect.Bleeding,
            "このターンの間、攻撃する毎に {0} ダメージを受ける\nターン終了時に消滅する"},
        {CardEffect.Guard,
            "ダメージを {0} だけ肩代わりする\n敵の次のターン終了時に消滅する"},
        {CardEffect.AttackUp,
            "攻撃力が {0} 上昇する\nターン終了時に消滅する"},
        {CardEffect.AttackDown,
            "攻撃力が {0} 減少する\nターン終了時に消滅する"},
    };
    #endregion
}