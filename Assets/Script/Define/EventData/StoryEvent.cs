using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Events/Story Event")]
public class StoryEvent : FloorEvent
{

    public override void Execute(PlayerCTR player, System.Action onComplete)
    {

        EventUI.Instance.ShowStory(storyData, onComplete);
    }
}