using UnityEngine;
using System.Collections.Generic;

public abstract class EventAction : ScriptableObject
{
    [TextArea(1, 3)]
    public string description;
    public abstract void Execute(PlayerCTR player);
    public virtual bool CanExecute(PlayerCTR player) => true;
    public virtual string GetResultMessage() => "";
}