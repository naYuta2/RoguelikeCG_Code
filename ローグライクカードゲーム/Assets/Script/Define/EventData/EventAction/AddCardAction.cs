using UnityEngine;

[CreateAssetMenu(menuName = "EventActions/AddCardAction")]
public class AddCardAction : EventAction{
    [SerializeField] CardData cardData;

    public override void Execute(PlayerCTR player){
        GameManager.Instance.AddCard(cardData);
    }

    public override string GetResultMessage(){
        return $"カード「{cardData.name}」を獲得";
    }
}