using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set;}

    Stack<CardData> stack = new Stack<CardData>();
    [SerializeField, ReadOnly]
    List<GameObject> hand = new List<GameObject>();
    [SerializeField, ReadOnly]
    List<CardData> discardPile = new List<CardData>();

    public List<GameObject> enemies = new List<GameObject>();
    public List<EnemyAction> enemyActions = new List<EnemyAction>();
    List<CardData> deck;

    [SerializeField]
    StageData stageData;

    public TurnManager turnManager {get; private set;}

    public bool inBattle {get; private set;}
    int currentCardCount = 0;

    public int N;

    [SerializeField]
    GameObject pref_card;

    [SerializeField]
    GameObject pref_damageUI;

    [SerializeField]
    GameObject pref_enemy;

    [SerializeField]
    GameObject gameManager;

    public GameObject player;

    [SerializeField]
    Button turnEndButton;

    [SerializeField]
    float mana_MAX;

    [SerializeField]
    float interval;

    [SerializeField]
    Text manaLabel;

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    Transform handArea;

    [SerializeField]
    Transform pileArea;

    [SerializeField]
    Transform discardPileArea;

    [SerializeField]
    Transform damageUIArea;

    [SerializeField]
    Transform enemyArea;

    [SerializeField]
    RectTransform[] spawnSlot = new RectTransform[4];
    GameObject[] occupyingEnemies = new GameObject[4];

    [HideInInspector]
    public float mana {get; private set;}

    [SerializeField]
    float waitTime;

    [SerializeField]
    int max_enemy;

    // 戦闘中に表示するCanvas
    [SerializeField]
    GameObject battleCanvas;

    CardCTR cardCTR;

    PlayerCTR playerCTR;

    bool handOutCardFlag = false;

    void Awake(){
        if(Instance == null){
            Instance = this;
            turnManager = this.GetComponent<TurnManager>();
        }
        else{
            Destroy(gameObject);
        }
        inBattle = false;
    }

    void Start(){
        currentCardCount = 0;
        battleCanvas.SetActive(false);
        mana = mana_MAX;
    }

/*
    void Update(){
        if(inBattle){
            if(player_turn){
                if (player_turn_start){
                    player.GetComponent<IDamageable>().TurnStart();
                    Debug.Log("ターンスタート");
                    mana = mana_MAX;
                    StartCoroutine(HandOutCardsCoroutine(N));
                    DeterminesEnemyActions();
                    turnCount += 1;
                    player_turn_start = false;
                }

                if (turnEnd){
                    currentCardCount = 0;
                    player_turn = false;
                    StartCoroutine(DiscardHandCoroutine(() =>{
                        enemy_turn = true;
                        enemy_turn_start = true;
                        turnEnd = false;
                    }));
                    foreach(GameObject enemy in enemies.ToList()){
                        enemy.GetComponent<IDamageable>().TurnStart();
                    }
                }
            }
            else if (enemy_turn){
                if (enemy_turn_start){
                    Debug.Log("敵ターン開始");
                    enemy_turn_start = false;
                    StartCoroutine(EnemyTurnCoroutine());
                }
            }

            manaLabel.text = $"{mana} / {mana_MAX}";
            if(handOutCardFlag){
                turnEndButton.interactable = false;
            }
            else if(player_turn){
                turnEndButton.interactable = (turnEnd) ? false : true;
            }
            else{
                turnEndButton.interactable = false;
            }
        }
    }
*/



    public void StartBattle(bool Ambush, List<EnemyStats> enemyList = null){
        inBattle = true;
        GameManager.Instance.StartBattle();
        Debug.Log("バトル開始！");
        currentCardCount = 0;

        EventUI.Instance.ShowBattleUI();

        List<CardData> Deck = gameManager.GetComponent<GameManager>().Deck;
        deck = new List<CardData>(Deck);
        Debug.Log($"Deck Count: {deck.Count}");

        int RandomInt = Random.Range(1, stageData.startEnemy+1);
        int enemyNumber = (stageData.isFixed) ? RandomInt : stageData.startEnemy;
        SummonEnemies(enemyNumber, enemyList);

        mana = mana_MAX;
        ShuffleList(deck);
        InsertDeck(deck);
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy")){
            enemies.Add(e);
        }
        playerCTR = player.GetComponent<PlayerCTR>();
        if (turnManager != null){
            turnManager.StartBattle();
        }
        else{
            Debug.LogError("TurnManagerが見つかりません！");
        }
    }

    public void RestoreMana(){
        mana = mana_MAX;
        manaLabel.text = $"{mana} / {mana_MAX}";
    }

    public void EnablePlayerActions(){
        turnEndButton.interactable = true;
    }

    public void DisablePlayerActions(){
        turnEndButton.interactable = false;
    }

    public void EndBattle(){
        Debug.Log("バトル終了！");
        currentCardCount = 0;
        StartCoroutine(DiscardHandCoroutine(()=>{
            StartCoroutine(Delay(() =>{
                mana = mana_MAX;
                GameManager.Instance.EndBattle();
                battleCanvas.SetActive(false);
            }));
        }));
    }

    public void InsertDeck(List<CardData> list){
        stack = new Stack<CardData>(list);
        foreach (CardData card in stack){
            Debug.Log($"{card.cardName}");
        }
    }

    void ShuffleList<T>(List<T> list){
        System.Random rng = new System.Random();
        int n = list.Count;

        while(n > 1){
            n--;
            int k = rng.Next(n+1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void ShuffleBackIntoDeck(){
        List<CardData> list = new List<CardData>();
        foreach (CardData card in discardPile){
            list.Add(card);
        }
        discardPile.Clear();
        ShuffleList(list);
        InsertDeck(list);
        list = null;
    }

    public IEnumerator HandOutCardsCoroutine(int n){
        if (handOutCardFlag){
            yield break;
        }
        handOutCardFlag = true;
        for (int i=0; i < n; i++){
            DrawSingleCard();
            yield return new WaitForSeconds(waitTime);
        }
        handOutCardFlag = false;
        yield return null;
    }

    void DrawSingleCard(){
        currentCardCount += 1;

        if (stack.Count == 0){
            ShuffleBackIntoDeck();
        }
        GameObject card = Instantiate(pref_card, pileArea.transform);
        CardData data = stack.Pop();
        card.GetComponent<CardCTR>().cardData = Instantiate(data);
        hand.Add(card);

        // CardMove(card);
        RectTransform cardRect = card.GetComponent<RectTransform>();
        float cardWidth = cardRect.sizeDelta.x;
        // cardRect.anchoredPosition = new Vector2(currentCardCount * (cardWidth + interval), 0);
        cardRect.DOAnchorPos(new Vector2(currentCardCount * (cardWidth + interval), 0), 0.5f);
        card.GetComponent<CardCTR>().initialPosition = new Vector2(currentCardCount * (cardWidth + interval), 0);
    }

    public void CardUsing(GameObject card, List<GameObject> target){
        cardCTR = card.GetComponent<CardCTR>();
        CardData cardData = cardCTR.cardData;
        DiscardCard(cardData);
        hand.Remove(card);
        currentCardCount -= 1;
        cardCTR.DestroyThisCard();
        if(cardData.cardCost <= mana){
            mana -= cardData.cardCost;
            foreach(var cardEffect in cardData.effectList){
                // ターゲットタイプがランダムな敵の場合、ターゲットをランダムに選択
                if(cardEffect.targetType == CardEffectDefine.TargetType.RandomEnemy){
                    int rnd = Random.Range(0, target.Count);
                    GameObject randomTarget = target[rnd];
                    target.Clear();
                    target.Add(randomTarget);
                }
                switch(cardEffect.effectType){
                    case CardEffectDefine.CardEffect.Damage:
                        Debug.Log($"物理ダメージ: {cardEffect.value}");
                        foreach(GameObject t in target){
                            t.GetComponent<IDamageable>().Damage(cardEffect.value);
                        }
                        break;
                    case CardEffectDefine.CardEffect.MagicDmg:
                        Debug.Log($"魔法ダメージ: {cardEffect.value}");
                        foreach(GameObject t in target){
                            t.GetComponent<IDamageable>().Damage(cardEffect.value);
                        }
                        break;
                    case CardEffectDefine.CardEffect.Heal:
                        Debug.Log($"回復: {cardEffect.value}");
                        foreach(GameObject t in target){
                            t.GetComponent<IDamageable>().Heal(cardEffect.value);
                        }
                        break;
                    case CardEffectDefine.CardEffect.ManaHeal:
                        Debug.Log($"マナ回復: {cardEffect.value}");
                        mana += cardEffect.value;
                        break;
                    /*
                    case CardEffectDefine.CardEffect.Burn:
                        Debug.Log($"物理ダメージ: {cardEffect.value}");
                        break;
                    case CardEffectDefine.CardEffect.Bleeding:
                        Debug.Log($"物理ダメージ: {cardEffect.value}");
                        break;
                    */
                    case CardEffectDefine.CardEffect.Guard:
                        Debug.Log($"ガード: {cardEffect.value}");
                        foreach(GameObject t in target){
                            t.GetComponent<IDamageable>().Guard(cardEffect.value);
                        }
                        break;
                    
                    case CardEffectDefine.CardEffect.Poison:
                        Debug.Log($"毒: {cardEffect.value}");
                        foreach(GameObject t in target){
                            t.GetComponent<IBuffable>().AddBuff(new Buff(BuffType.Poison, cardEffect.value, -1));
                        }
                        break;
                }
            }

            manaLabel.text = $"{mana} / {mana_MAX}";

            bool frag = true;
            foreach(GameObject handCard in hand){
                if(handCard.GetComponent<CardCTR>().cardData.cardCost <= mana){
                    frag = false;
                }
            }
            if(frag){
                turnManager.PlayerEndTurn();
            }
        }
        else return;
    }

    public void DamageUI(int damage, Vector2 position){
        GameObject damageUI = Instantiate(pref_damageUI, damageUIArea.transform);
        damageUI.gameObject.name = "DamageUI";
        RectTransform damageUIRect = damageUI.GetComponent<RectTransform>();
        position.x += Random.Range(-50, 50);
        position.y += Random.Range(-50, 50);
        damageUIRect.anchoredPosition = position;
        Debug.Log($"DamageUI Position: {position}");
        Transform transform = damageUI.transform.Find("DamageText");
        Text damageText = transform.GetComponent<Text>();
        damageText.text = $"{damage}";
        Image image = damageUI.GetComponent<Image>();
        StartCoroutine(FadeOutCoroutine(image, damageText, damageUI));
    }

    public void DiscardCard(CardData cardData){
        discardPile.Add(cardData);
    }

    IEnumerator Delay(System.Action onComplete){
        Debug.Log("Delay Start");
        yield return new WaitForSeconds(1f);
        Debug.Log("Delay End");
        onComplete?.Invoke();
    }

    IEnumerator FadeOutCoroutine(Image image, Text text, GameObject target){
        float fadeTime = 1.0f;
        float fadeOutTime = 1.0f;
        float fadeInTime = 0.5f;

        image.DOFade(0, fadeOutTime).OnComplete(() => {
            Destroy(target);
        });
        text.DOFade(0, fadeOutTime).OnComplete(() => {
            Destroy(text.gameObject);
        });

        yield return new WaitForSeconds(fadeTime);
    }

    public IEnumerator DiscardHandCoroutine(System.Action onComplete){
        foreach (GameObject card in hand){
            CardData cardData = card.GetComponent<CardCTR>().cardData;
            // カードの親をDiscarPileに変更
            card.transform.SetParent(discardPileArea.transform);
            DiscardCard(cardData);
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.DOAnchorPosX(0f, 0.2f).OnComplete(() => {
                Debug.Log("Discard Card");
                card.GetComponent<CardCTR>().DestroyThisCard();
            });
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        hand.Clear();
        currentCardCount = 0;
        yield return null;
        onComplete?.Invoke();
    }

    public void EnemyDead(GameObject e){
        enemies.Remove(e);
        enemyActions.RemoveAll(action => action.enemy == e);
        // 敵が居ないなら戦闘終了
        if (enemies.Count == 0){
            Debug.Log("戦闘終了");
            turnManager.EndBattle();
            return;
        }

        // 倒した敵がボスなら他にボスが居ないか確認 → 居たら戦闘続行
        foreach(GameObject enemy in enemies.ToList()){
            if (enemy.GetComponent<EnemyCTR>().status.isBoss){
                Debug.Log("ボスが居る！戦闘続行！");
                return;
            }
        }
        // ボスが居ないなら戦闘終了
        if (e.GetComponent<EnemyCTR>().status.isBoss){
            Debug.Log("ボスを倒した！戦闘終了！");
            occupyingEnemies = new GameObject[4];;
            List<GameObject> deathEnemies = new List<GameObject>(enemies);
            foreach(GameObject enemy in deathEnemies){
                enemy.GetComponent<IDamageable>().Death();
            }
            enemies.Clear();
            enemyActions.Clear();
            deathEnemies.Clear();
            turnManager.EndBattle();
        }
    }

    void SummonEnemies(int n, List<EnemyStats> enemyList = null){
        float totalWidth = 200f * (n - 1);
        float startX = enemyArea.GetComponent<RectTransform>().anchoredPosition.x - totalWidth / 2f;

        if(enemyList != null){
            for(int i = 0; i < n; i++){
                GameObject newEnemy = Instantiate(pref_enemy, enemyArea);
                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX + i * 200f, 0f);
                newEnemy.GetComponent<EnemyCTR>().status = enemyList[Random.Range(0, enemyList.Count)];
                enemies.Add(newEnemy);
            }
        }
        else{
            for(int i = 0; i < n; i++){
                GameObject newEnemy = Instantiate(pref_enemy, enemyArea);
                newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX + i * 200f, 0f);
                newEnemy.GetComponent<EnemyCTR>().status = stageData.enemiesInThisStage[Random.Range(0, stageData.enemiesInThisStage.Count)];
            }
        }
    }

    void SpawnEnemy(EnemyStats e, RectTransform parentEnemyTransform){
        if(enemies.Count >= max_enemy) return;
        GameObject newEnemy = Instantiate(pref_enemy, enemyArea);
        newEnemy.GetComponent<EnemyCTR>().status = e;
        float posX = 200f;
        // 召喚によって呼び出された敵は基本的に召喚元の敵の隣に配置される
        for(int i = 0; i < max_enemy; i++){
            if(occupyingEnemies[i] == null){
                occupyingEnemies[i] = newEnemy;
                posX = spawnSlot[i].anchoredPosition.x;
                break;
            }
        }
        newEnemy.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0f);
        enemies.Add(newEnemy);
    }

    public void DoEnemyAction(EnemyAction action){
        Debug.Log("敵の行動！");
        switch(action.actionType){
            case EnemyAction.ActionType.Attack:
                Debug.Log($"攻撃！プレイヤーに{action.value}のダメージ！");
                playerCTR.Damage(action.value);
                break;
            case EnemyAction.ActionType.Defend:
                Debug.Log($"敵はシールドを{action.value}だけ生成した！");
                action.enemy.GetComponent<EnemyCTR>().Guard(action.value);
                break;
            case EnemyAction.ActionType.Summon:
                Debug.Log($"敵は新たな敵を呼び出した！");
                for(int i = 0; i < action.value; i++){
                    SpawnEnemy(action.summonTarget, action.enemy.GetComponent<RectTransform>());
                }
                break;
        }
        Debug.Log("行動終了");
    }

    public void DeterminesEnemyActions(){
        EnemyAction action;
        foreach(GameObject e in enemies){
            enemyActions.Add(e.GetComponent<EnemyCTR>().DetermineEnemyAction());
        }
    }

    public void TurnEndButton(){
        if(turnManager.CanPlayerAct() && !handOutCardFlag){
            turnManager.PlayerEndTurn();
        }
    }

    public void GameOver(){
        Debug.Log("GameOver");
    }
}