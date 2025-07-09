using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    CardData card;
    [SerializeField] Text nameBase;
    [SerializeField] Text costBase;
    [SerializeField] Text effectBase;

    public void Setup(CardData cardData){
        card = cardData;
        nameBase.text = cardData.cardName;
        costBase.text = cardData.cardCost.ToString();
        string effectText = null;
        foreach (var cardEffect in cardData.effectList) {
            string add = CardEffectDefine.Dic_EffectName_JP[cardEffect.effectType];
            add = string.Format(add, cardEffect.value);
            effectText = effectText + "〇" + add + "\n";
        }
        effectBase.text = effectText;
    }
}
