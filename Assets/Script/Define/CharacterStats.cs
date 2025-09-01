using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Stats/Player",fileName = "NewCharacter")]
public class CharacterStats : ScriptableObject
{
    public string characterName; //名前
    public int maxHP; //HP
    public int attack; //攻撃力
    public int defense; //防御力
    public int MagicAttack; //魔法攻撃力
    public int MagicDefense; //魔法防御力
}