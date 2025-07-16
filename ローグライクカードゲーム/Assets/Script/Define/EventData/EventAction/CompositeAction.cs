using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "EventActions/CompositeAction")]
public class CompositeAction : EventAction{
    [SerializeField] List<EventAction> actions;

    public override void Execute(PlayerCTR player){
        foreach (var action in actions){
            if(action.CanExecute(player)){
                action.Execute(player);
            }
        }
    }

    public override string GetResultMessage(){
        string result = "";
        foreach (var action in actions){
            var message = action.GetResultMessage();
            if (!string.IsNullOrEmpty(message)){
                result += message + "\n";
            }
        }
        return result.TrimEnd('\n');
    }
}