using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Events/Heal Event")]
public class HealEvent : FloorEvent
{
    public int healAmount;

    public override void Execute(PlayerCTR player, System.Action onComplete)
    {

        EventUI.Instance.ShowStory(storyData, onComplete);
    }

    public void Heal()
    {
        PlayerCTR.Instance.Heal(healAmount);
        Debug.Log($"プレイヤーのHPが{healAmount}回復");
    }
}
