using UnityEngine;

public enum BuffType{
    AttackUp,
    AttackDown,
    Poison,
    Burned
}

[System.Serializable]
public abstract class Buff
{
    public BuffType buffType;
    public int value;
    public int duration;

    public Buff(BuffType type, int value, int duration){
        this.buffType = type;
        this.value = value;
        this.duration = duration;
    }
}