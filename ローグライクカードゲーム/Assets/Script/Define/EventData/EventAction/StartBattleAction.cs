using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "EventActions/StartBattleAction")]
public class StartBattleAction : EventAction{
    [SerializeField] List<EnemyStats> enemy;

    public override void Execute(PlayerCTR player){
        GameManager.Instance.StartEventBattle(enemy);
    }

    public override string GetResultMessage(){
        return "バトル開始";
    }
}