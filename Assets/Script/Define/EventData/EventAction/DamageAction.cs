using UnityEngine;

[CreateAssetMenu(menuName = "EventActions/DamageAction")]
public class DamageAction : EventAction{
    [SerializeField] int value;

    public override void Execute(PlayerCTR player){
        player.Damage(value);
    }

    public override string GetResultMessage(){
        return $"{value}のダメージを受けた";
    }
}