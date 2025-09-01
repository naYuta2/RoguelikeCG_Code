using UnityEngine;

[CreateAssetMenu(menuName = "EventActions/HealAction")]
public class HealAction : EventAction
{
    [SerializeField] int value;

    public override void Execute(PlayerCTR player){
        player.Heal(value);
    }

    public override string GetResultMessage(){
        return $"{value}のHPを回復した";
    }
}