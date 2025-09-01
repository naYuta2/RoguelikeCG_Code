using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CardCTR : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Vector2 initialPosition;
    private Vector2 mousePosition;

    List<GameObject> target = new List<GameObject>();
    GameObject[] enemies;

    [SerializeField]
    GameObject player;

    int bonusAttack;

    [SerializeField]
    Text nameLabel;

    [SerializeField]
    Text effectLabel;

    [SerializeField]
    Text costLabel;

    [SerializeField]
    float waitTime;

    [SerializeField]
    float a;

    public CardData cardData;

    Character playerStats;

    BattleManager battleManager => BattleManager.Instance;

    bool pos_reset = false;
    bool isDragged = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        // カード名を表示
        nameLabel.text = cardData.cardName;

        // カード効果テキストを生成
        string effectText = null;
        foreach (var cardEffect in cardData.effectList) {
            string add = CardEffectDefine.Dic_EffectName_JP[cardEffect.effectType];
            add = string.Format(add, cardEffect.value);
            effectText = effectText + "〇" + add + "\n";
        }
        // カード効果テキストを表示　～Debug.Logを添えて～
        Debug.Log(effectText);
        effectLabel.text = effectText;

        // カードコストを表示
        costLabel.text = cardData.cardCost.ToString();
    }

    public void OnPointerDown(PointerEventData eventData){
        Debug.Log("カードがタップされました");
        /* カードの詳細情報を表示する処理をここに追加 */
        /* 例えば、カードの効果やコストを表示するUIを表示するなど */
    }

    public void OnPointerUp(PointerEventData eventData){
        Debug.Log("カードへのタップを終了しました");
        /* カードの詳細情報を非表示にする処理をここに追加 */
    }

    public void OnPointerEnter(PointerEventData eventData){
        if(isDragged) return; // ドラッグ中は処理しない
        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y+20f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData){
        if(isDragged) return; // ドラッグ中は処理しない
        rectTransform.DOAnchorPos(initialPosition, 0.2f);
    }

    public void OnBeginDrag(PointerEventData eventData){
        transform.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out mousePosition
        );
        mousePosition -= rectTransform.anchoredPosition;

        canvasGroup.alpha = a;
        canvasGroup.blocksRaycasts = false;
        isDragged = true;
    }


    public void OnDrag(PointerEventData eventData){
        if (canvas == null){
            return;
        }

        // ドラッグ中のカードの位置を更新
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out position
        );
        rectTransform.anchoredPosition = position - mousePosition;

        //enemiesがnullの場合、全ての敵オブジェクトを取得
        if (enemies == null){
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }

        // 使用可能ならターゲットを取得
        if(battleManager.mana >= cardData.cardCost && rectTransform.anchoredPosition.y >= 180){
            GameObject enemy = null;
            foreach (var effect in cardData.effectList){
                switch(effect.targetType){
                    case CardEffectDefine.TargetType.SingleEnemy:
                        if(enemy != GetDropTarget(eventData)){
                            enemy = GetDropTarget(eventData);
                        }
                        // enemyがnullの場合、pos_resetをtrueにする
                        if(enemy == null) {
                            pos_reset = true;
                            target.Clear();
                        }
                        else {
                            pos_reset = false;
                            //targetにenemyが含まれていない場合、targetにenemyを追加する
                            if(!target.Contains(enemy)) target.Add(enemy);
                        }
                        break;

                    case CardEffectDefine.TargetType.AllEnemies:
                        foreach(GameObject e in enemies){
                            //targetにenemyが含まれていない場合、targetにenemyを追加する
                            if(!target.Contains(e)) target.Add(e);
                        }
                        pos_reset = false;
                        break;
                    
                    case CardEffectDefine.TargetType.RandomEnemy:
                        foreach(GameObject e in enemies){
                            //targetにenemyが含まれていない場合、targetにenemyを追加する
                            if(!target.Contains(e)) target.Add(e);
                        }
                        pos_reset = false;
                        break;
                    
                    case CardEffectDefine.TargetType.Self:
                        if(!target.Contains(player)) target.Add(player);
                        pos_reset = false;
                        break;
                }
                /*
                if(effect.targetType == CardEffectDefine.TargetType.SingleEnemy){
                    if(target == null){
                        target = GetDropTarget(eventData);
                    }
                    if(target == null){
                        pos_reset = true;
                    }
                }
                */
                if(!pos_reset && effect.targetType != CardEffectDefine.TargetType.Self) {
                    foreach(GameObject t in target){
                        //targetのマテリアルのフラグを変更
                        EnemyCTR enemyCTR = t.GetComponent<EnemyCTR>();
                        enemyCTR.OutlineOn();
                    }
                }
                //target以外のオブジェクトのマテリアルのフラグを元に戻す
                foreach(GameObject t in enemies){
                    if(!target.Contains(t)){
                        EnemyCTR enemyCTR = t.GetComponent<EnemyCTR>();
                        enemyCTR.OutlineOff();
                    }
                }
            }
        }
        else {
            pos_reset = true;
            target.Clear();
            foreach(GameObject t in enemies){
                EnemyCTR enemyCTR = t.GetComponent<EnemyCTR>();
                enemyCTR.OutlineOff();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData){
        isDragged = false;
        Debug.Log("ドラッグ終了");
        Debug.Log($"pos_reset: {pos_reset}");
        /*
        bool pos_reset = false;
        // 使用可能ならカードを使用
        if (rectTransform.anchoredPosition.y < 180){
            pos_reset = true;
        }
        if(battleManager.mana >= cardData.cardCost && !pos_reset){
            // カードを使用する処理をここに追加
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject enemy;
            foreach (var effect in cardData.effectList){
                switch(effect.targetType){
                    case CardEffectDefine.TargetType.SingleEnemy:
                        enemy = GetDropTarget(eventData);
                        if(enemy == null) pos_reset = true;
                        else target.Add(enemy);
                        break;

                    case CardEffectDefine.TargetType.AllEnemies:
                        foreach(GameObject e in enemies){
                            target.Add(e);
                        }
                        break;
                    
                    case CardEffectDefine.TargetType.RandomEnemy:
                        enemy = enemies[Random.Range(0,enemies.Length)];
                        target.Add(enemy);
                        break;
                    
                    case CardEffectDefine.TargetType.Self:
                        target.Add(this.gameObject);
                        break;
                }
                if(!pos_reset) battleManager.CardUsing(this.gameObject, target);
            }
        }
        else pos_reset = true;
        */

        // フラグが立っていない場合、カードを使用する
        if(!pos_reset) {
            battleManager.CardUsing(this.gameObject, target);
        }
        // そうでないなら、カードを元の位置に戻す
        else{
            rectTransform.anchoredPosition = initialPosition;
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true;
            pos_reset = false;
        }

        target.Clear();

        // 敵オブジェクトを改めて取得
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 敵オブジェクトのマテリアルのフラグを元に戻す
        foreach(GameObject e in enemies){
            EnemyCTR enemyCTR = e.GetComponent<EnemyCTR>();
            enemyCTR.OutlineOff();
        }

        enemies = null;
    }

    public void DestroyThisCard(){
        Destroy(this.gameObject);
    }

    // ドロップターゲットを取得するメソッド
    // PointerEventDataを引数にとり、ドロップターゲットのGameObjectを返す
    GameObject GetDropTarget(PointerEventData eventData){
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = eventData.position };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // 取得したRaycastResultの中から、"Enemy"タグを持つGameObjectを探す
        // もし見つかったら、そのGameObjectを返す
        foreach (var result in results){
            if (result.gameObject.CompareTag("Enemy")){
                return result.gameObject;
            }
        }
        // 見つからなかった場合はnullを返す
        return null;
    }

    void ApplyCardEffect(GameObject target){
        Debug.Log($"カードを {target.name} に使用！");
    }
}
