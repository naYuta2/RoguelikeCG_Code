using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StoryData", menuName = "StoryData")]
public class StoryData : ScriptableObject
{
    public int ID;
    public List<Story> stories = new List<Story>();
}

[System.Serializable]
public class Story{
    public Sprite Background;
    [TextArea]
    public string StoryText;
    public string CharacterName;
    public List<Choice> Choices = new List<Choice>();
}

[System.Serializable]
public class Choice
{
    public string Text;
    public List<Story> NextStories = new List<Story>();
    [Header("実行するアクション")]
    public List<EventAction> actions = new List<EventAction>();

    public void ExecuteActions(PlayerCTR player){
        foreach (var action in actions){
            if (action.CanExecute(player)){
                action.Execute(player);
                Debug.Log(action.GetResultMessage());
            }
        }
    }
}