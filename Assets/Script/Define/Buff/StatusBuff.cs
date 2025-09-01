using UnityEngine;
using System.Collections.Generic;

public enum BuffTiming{
    TurnEnd,
    TurnStart,
    BeforeAttack,
    BeforeDefense,
    _Max,
}

public enum BuffType{
    // 攻撃力系
    AttackUp,
    AttackDown,

    // ガード系
    GuardUp,
    GuardDown,

    // ダメージ量系
    DamageUp,
    DamageDown,

    // 状態異常系
    Poison,
    Burned,
    Bloodied,

    // 回復系
    Regeneration,

    _Max,
}

public enum BuffBehaviorType{
    ValueDecreasing, // 継続時間が無く、ターン毎に効果値が減少するタイプ
    DurationBased, // ターン毎に効果値は変わらず、継続時間が減少するタイプ
    Instant, // 即時効果で、ターン終了時に消えるタイプ
    Permanent, // バトル中永続的な効果で、ターン終了時に消えないタイプ
    ActionBased, // 特定のアクションに基づいて効果が発動するタイプ
                 // 例えば攻撃を行うと効果が発動する、とか
                 // 実装むずそう…
    _Max, // 一応最大値を定義 (これを超えるとエラーが出るよ☆)
}

[System.Serializable]
public class Buff
{
    public BuffType buffType;
    public BuffBehaviorType behaviorType;
    public int value;
    public int duration;
    public string name;
    public string description;
    public Sprite icon;

    public Buff(BuffType type, int value, int duration){
        this.buffType = type;
        this.value = value;
        this.duration = duration;
    }

    static BuffBehaviorType GetBuffBehavior(BuffType type){
        switch(type){
            // 効果値減少タイプ
            case BuffType.Poison:
                return BuffBehaviorType.ValueDecreasing;
            
            // 継続時間減少タイプ(デフォルトでもこのタイプになるよう設定してるが念のため)
            case BuffType.AttackUp:
            case BuffType.AttackDown:
            case BuffType.GuardUp:
            case BuffType.GuardDown:
            case BuffType.DamageUp:
            case BuffType.DamageDown:
            case BuffType.Burned:
            case BuffType.Regeneration:
                return BuffBehaviorType.DurationBased;
            
            // 即時効果タイプ(いまのところはナシ)

            // 永続効果タイプ(いまのところはナシ)

            // アクションベースタイプ
            case BuffType.Bloodied:
                return BuffBehaviorType.ActionBased;
            
            default:
                return BuffBehaviorType.DurationBased; // デフォルトとして継続時間タイプを設定

        }
    }

    #region バフの種類定義部

    readonly public static Dictionary<BuffType, string> Dic_BuffName_JP = new Dictionary<BuffType, string>(){
        {BuffType.AttackUp, 
            "与ダメージ量増加 {0}"},
        {BuffType.AttackDown, 
            "与ダメージ量減少 {0}"},
        {BuffType.GuardUp, 
            "ガード付与量増加 {0}"},
        {BuffType.GuardDown,
            "ガード付与量減少 {0}"},
        {BuffType.DamageUp, 
            "被ダメージ量増加 {0}"},
        {BuffType.DamageDown,
            "被ダメージ量減少 {0}"},
        {BuffType.Poison,
            "毒 {0}"},
        {BuffType.Burned,
            "火傷 {0}"},
        {BuffType.Regeneration,
            "再生 {0}"}
    };

    readonly public static Dictionary<BuffType, string> Dic_BuffExplain_JP = new Dictionary<BuffType, string>(){
        {BuffType.AttackUp, 
            "攻撃によって与えるダメージが {0} 増加する"},
        {BuffType.AttackDown, 
            "攻撃によって与えるダメージが {0} 減少する"},
        {BuffType.GuardUp, 
            "カードによって得るガードの量が {0} 増加する"},
        {BuffType.GuardDown,
            "カードによって得るガードの量が {0} 減少する"},
        {BuffType.DamageUp, 
            "敵からの攻撃によって受けるダメージが {0} 増加する"},
        {BuffType.DamageDown,
            "敵からの攻撃によって受けるダメージが {0} 減少する"},
        {BuffType.Poison,
            "ターン開始時にHPが {0} 減少し、このデバフの数値が1減る"},
        {BuffType.Burned,
            "ターン終了時に {0} ダメージを受ける"},
        {BuffType.Regeneration,
            "ターン終了時に {0} だけHPを回復する"}
    };

    #endregion
}