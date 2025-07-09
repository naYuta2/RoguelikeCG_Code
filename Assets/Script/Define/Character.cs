using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour, IDamageable
{
    public CharacterStats stats;

    int maxHP;
    int hp;
    int guardValue = 0;

    [ReadOnly]
    List<Buff> buffs = new List<Buff>();

    public int HP{
        get { return hp; }
        set {
            hp = Mathf.Clamp(value, 0, maxHP);
            if (hp <= 0){
                Death();
            }
        }
    }

    public virtual void ApplyEffectsEachTurn(){
        foreach (var buff in buffs){
            if(buff.duration <= 0){
                buffs.Remove(buff);
            }
        }
    }

    protected virtual void Start(){
        maxHP = stats.maxHP;
        hp = maxHP;
        Debug.Log("Start currentHP : " + hp);
    }

    public virtual void Damage(int value){
        if(guardValue > 0){
            if(guardValue >= value){
                guardValue -= value;
                return;
            } 
            else {
                value -= guardValue;
                guardValue = 0;
            }
        }
        hp -= value;
    }

    public virtual void MagicDmg(int value){
        hp -= value;
    }

    public void Heal(int value){
        hp += value;
    }

    public virtual void Guard(int value){
        guardValue += value;
    }

    public virtual void Death()
    {
        if(this.gameObject.tag == "Player"){
            // Handle player death (e.g., show game over screen)
            Debug.Log("Player has died.");
        } 
        else {
            gameObject.tag = "EnemyDead";
        }
    }

    public virtual void TurnStart(){
        guardValue = 0;
    }
}