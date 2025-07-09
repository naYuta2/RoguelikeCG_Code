using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


[System.Serializable]
public class EnemyAction{
    public ActionType actionType;
    public int value;
    public GameObject enemy;
    public EnemyStats summonTarget;

    public enum ActionType{
        Attack,
        Defend,
        Summon
    }
}

[CreateAssetMenu(menuName = "Stats/Enemy", fileName = "NewEnemy")]
public class EnemyStats : ScriptableObject
{
    public string enemyName; //名前
    public int maxHP; //HP
    public List<EnemyAction> actions;
    [Header ("行動パターン")]
    public ActionPattern pattern;
    [Header ("敵の見た目(テクスチャ)")]
    public Sprite sprite;
    [Header ("ボスかどうか")]
    public bool isBoss; //ボスかどうか

    public enum ActionPattern{
        Random,
        Sequence,
        Conditional
    };
}