using UnityEngine;

public abstract class FloorEvent : ScriptableObject
{
    public string eventID;
    public string eventName;
    public StoryData storyData;
    public int minFloor;
    public int maxFloor;
    public int weight = 1;

    public abstract void Execute(PlayerCTR player, System.Action onComplete);
}
