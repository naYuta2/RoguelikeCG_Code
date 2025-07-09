using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Events/Card Reward Event")]
public class CardRewardEvent : FloorEvent
{
    [SerializeField] List<CardData> rewardCards;
    public override void Execute(PlayerCTR player, System.Action onComplete)
    {
        /*
        EventUI.Instance.ShowEventWithoutButton(storyData, () =>
        {
            EventUI.Instance.ShowChoiceUI(rewardCards, (selectedCard) =>
            {
                GameManager.Instance.AddCard(selectedCard);
                onComplete?.Invoke();
            });
        });
        */
        onComplete?.Invoke();
    }
}
