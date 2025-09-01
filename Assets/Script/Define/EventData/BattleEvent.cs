using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Events/Battle Event")]
public class BattleEvent : FloorEvent
{
    public override void Execute(PlayerCTR player, Action onComplete){
        BattleManager.Instance.StartBattle(false);
        onComplete?.Invoke();
    }
}