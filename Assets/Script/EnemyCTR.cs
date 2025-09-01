using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyCTR : MonoBehaviour, IDamageable, IBuffable
{
    public Slider slider;

    [SerializeField]
    float fadeDuration = 1.0f;

    [SerializeField]
    public EnemyStats status;

    [SerializeField]
    Text hpText;
    [SerializeField]
    Text guardText;
    [SerializeField]
    Text actionText;

    int _maxHP;
    int maxHP{
        get {return _maxHP;}
        set{
            _maxHP = value;
            if (_maxHP < 1) _maxHP = 1;
            ChangeHP();
        }
    }
    int _currentHP;
    int _guardValue;
    public int guardValue{
        get { return _guardValue;}
        set {
            Debug.Log($"EnemyGuard set: {value}");
            if (value < 0) _guardValue = 0;
            else _guardValue = value;
            ChangeGuard();
        }
    }

    public int currentHP{
        get { return _currentHP; }
        set {
            Debug.Log($"EnemyHP set: {value}");
            _currentHP = Mathf.Clamp(value, 0, maxHP);
            if (_currentHP <= 0){
                Death();
            }
            ChangeHP();
        }
    }

    int turnCount;

    [SerializeField]
    Image actionIcon;
    [SerializeField]
    Sprite attackIcon;
    [SerializeField]
    Sprite defendIcon;
    [SerializeField]
    Sprite summonIcon;

    Image image;

    Material outlineMaterial;

    BattleManager battleManager;

    RectTransform rectTransform;

    EnemyAction action = new EnemyAction();

    [ReadOnly]
    List<Buff> buffs = new List<Buff>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actionIcon.gameObject.SetActive(false);
        turnCount = 0;
        status = Instantiate(status);
        maxHP = status.maxHP;
        currentHP = maxHP;
        guardValue = 0;

        slider.value = 1;
        Debug.Log("Start currentHP : " + currentHP);

        image = GetComponent<Image>();

        Material m = image.material;
        outlineMaterial = Instantiate(m);

        image.sprite = status.sprite;
        image.material = outlineMaterial;

        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Update()
    {
        // hpText.text = $"{currentHP} / {maxHP}";
        guardText.text = $"{guardValue}";
    }

    // Update is called once per frame
    public void Damage(int damage)
    {
        battleManager.DamageUI(damage, rectTransform.anchoredPosition);
        Debug.Log($"Damage: {damage}");
        if (guardValue > 0)
        {
            if (guardValue >= damage)
            {
                guardValue -= damage;
                return;
            }
            else
            {
                damage -= guardValue;
                guardValue = 0;
            }
        }
        currentHP -= damage;
        Debug.Log("After currentHP : " + currentHP);

        slider.value = (float)currentHP / (float)maxHP;
        Debug.Log("slider.value : " + slider.value);
    }

    public void MagicDmg(int damage)
    {
        battleManager.DamageUI(damage, rectTransform.anchoredPosition);
        currentHP -= damage;
        Debug.Log("After currentHP : " + currentHP);

        slider.value = (float)currentHP / (float)maxHP;
        Debug.Log("slider.value : " + slider.value);
        if (currentHP <= 0)
        {
            StartCoroutine(FadeOutCoroutine());
            // タグを変更する
            gameObject.tag = "EnemyDead";
        }
    }

    public virtual void Guard(int value)
    {
        guardValue += value;
    }

    IEnumerator FadeOutCoroutine()
    {
        float time = 0.0f;
        Color color = image.color;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(1.0f, 0.0f, time / fadeDuration);
            image.color = color;
            yield return null;
        }
        Destroy(gameObject);
    }

    public void OutlineOn()
    {
        image.material.SetFloat("_isTarget", 1.0f);
    }

    public void OutlineOff()
    {
        image.material.SetFloat("_isTarget", 0.0f);
    }

    public virtual void Heal(int value)
    {
        currentHP += value;
    }

    public virtual void Death()
    {
        OutlineOff();
        battleManager.EnemyDead(this.gameObject);
        // タグを変更する
        gameObject.tag = "EnemyDead";
        StartCoroutine(FadeOutCoroutine());
    }

    public virtual void TurnStart()
    {
        guardValue = 0;
        turnCount += 1;
        ApplyBuffEffectsEachTurn();
    }

    public EnemyAction DetermineEnemyAction()
    {
        switch (status.pattern)
        {
            case EnemyStats.ActionPattern.Random:
                action = status.actions[UnityEngine.Random.Range(0, status.actions.Count)];
                action.enemy = this.gameObject;
                break;
            case EnemyStats.ActionPattern.Sequence:
                action = status.actions[turnCount % status.actions.Count];
                action.enemy = this.gameObject;
                break;
            case EnemyStats.ActionPattern.Conditional:
                // ここに敵特有のアクションを追加
                // EnemyCTRでは特に追加しない
                break;
            default:
                break;
        }
        BehaviorPrediction(action);
        return action;
    }

    void BehaviorPrediction(EnemyAction action)
    {
        // 敵の行動予測を表示する処理を追加
        switch (action.actionType)
        {
            case EnemyAction.ActionType.Attack:
                actionIcon.sprite = attackIcon;
                break;
            case EnemyAction.ActionType.Defend:
                actionIcon.sprite = defendIcon;
                break;
            case EnemyAction.ActionType.Summon:
                actionIcon.sprite = summonIcon;
                break;
            default:
                break;
        }
        actionIcon.gameObject.SetActive(true);
        actionText.text = action.value.ToString();
    }

    public void AddBuff(Buff buff){
        // 既に同じバフが存在しているか確認
        var existingBuff = buffs.Find(b => b.buffType == buff.buffType);
        // もし存在していれば、バフの値と持続時間を更新
        if (existingBuff != null){
            Debug.Log($"敵のバフを更新します");
            existingBuff.value += buff.value;
            if (buff.duration != -1){
                existingBuff.duration += buff.duration;
            }
            Debug.Log($"敵のバフを更新: type={existingBuff.buffType}, value={existingBuff.value}, duration={existingBuff.duration}");
        }
        // 存在していなければバフを新たに追加
        else{
            Debug.Log($"敵のバフを追加します");
            buffs.Add(buff);
            Debug.Log($"敵のバフを追加: type={buff.buffType}, value={buff.value}, duration={buff.duration}");
        }
        UpdateBuffText(buffs[buffs.Count - 1]);
    }

    public void RemoveBuff(BuffType type){
        buffs.RemoveAll(b => b.buffType == type);
        Debug.Log($"敵のバフを削除: type={type}");
    }

    public void ApplyBuffEffectsEachTurn(){
        foreach (var buff in buffs.ToList()){
            ApplyBuffEffect(buff);
            switch (buff.behaviorType){
                case BuffBehaviorType.ValueDecreasing:
                    buff.value--;
                    if (buff.value <= 0){
                        buffs.Remove(buff);
                        Debug.Log($"バフを削除: {buff.name}");
                    }
                    UpdateBuffText(buff);
                    break;
                case BuffBehaviorType.DurationBased:
                    buff.duration--;
                    if (buff.duration <= 0){
                        buffs.Remove(buff);
                        Debug.Log($"バフを削除: {buff.name}");
                    }
                    break;
                case BuffBehaviorType.Instant:
                    buffs.Remove(buff);
                    Debug.Log($"バフを削除: {buff.name}");
                    break;
                
                case BuffBehaviorType.Permanent:
                    // 永続効果は普通効果が消えないので何もしない
                    break;

                case BuffBehaviorType.ActionBased:
                    // アクションベースのバフはターン開始時に何もしない…よな？
                    break;
                
                default:
                    Debug.LogWarning($"未知のバフタイプが検出されました: {buff.behaviorType}");
                    break;
            }
        }
    }

    public void ApplyBuffEffect(Buff buff){
        switch (buff.buffType){
            case BuffType.Poison:
                Damage(buff.value);
                Debug.Log($"敵に毒ダメージを与えました: {buff.value}");
                break;
            default:
                Debug.LogWarning($"未知のバフタイプが検出されました: {buff.buffType}");
                break;
        }
    }

    void UpdateBuffText(Buff buff){
        Debug.Log("バフのテキストを更新");
        switch (buff.buffType){
            case BuffType.Poison:
                hpText.text = $"{currentHP} / {maxHP} (Poison: {buff.value})";
                break;
            default:
                Debug.LogWarning($"未知のバフタイプが検出されました: {buff.buffType}");
                break;
        }
    }

    void ChangeHP(){
        hpText.text = $"{currentHP} / {maxHP}";
    }

    void ChangeGuard(){
        guardText.text = $"{guardValue}";
    }
}
