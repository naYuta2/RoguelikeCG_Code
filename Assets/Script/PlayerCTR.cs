using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PlayerCTR : MonoBehaviour, IDamageable
{
    public static PlayerCTR Instance {get; private set;}
    public Slider slider;

    RectTransform rectTransform;

    [SerializeField]
    Text hp_text;
    
    [SerializeField]
    Text guard_text;

    // 後でpublic消そうね☆
    public CharacterStats status;

    BattleManager battleManager => BattleManager.Instance;

    int _currentHP;
    int maxHP;
    int _guardValue;

    public int guardValue{
        get { return _guardValue;}
        set {
            Debug.Log($"PlayerGuard set: {value}");
            if (value < 0) _guardValue = 0;
            else _guardValue = value;
            ChangeGuard();
        }
    }

    public int currentHP{
        get { return _currentHP; }
        set {
            _currentHP = Mathf.Clamp(value, 0, maxHP);
            if (currentHP <= 0){
                Death();
            }
            ChangeHP();
        }
    }

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        slider.value = 1;
        maxHP = status.maxHP;
        currentHP = maxHP;
        guardValue = 0;
    }

    // Update is called once per frame
    public void Damage(int damage){
        battleManager.DamageUI(damage, this.rectTransform.anchoredPosition);
        if (guardValue  > 0){
            if (guardValue >= damage){
                guardValue -= damage;
                return;
            } 
            else {
                damage -= guardValue;
                guardValue = 0;
            }
        }
        currentHP -= damage;
        Debug.Log("After currentHP : " + currentHP);

        slider.value = (float)currentHP / (float)maxHP;
        Debug.Log("slider.value : " + slider.value);
    }

    public void MagicDmg(int damage){
        battleManager.DamageUI(damage, this.rectTransform.anchoredPosition);
        if(guardValue > 0){
            if(guardValue >= damage){
                guardValue -= damage;
                return;
            } 
            else {
                damage -= guardValue;
                guardValue = 0;
            }
        }
        currentHP -= damage;
        Debug.Log("After currentHP : " + currentHP);

        slider.value = (float)currentHP / (float)maxHP;
        Debug.Log("slider.value : " + slider.value);
    }

    public void Heal(int healValue){
        currentHP += healValue;
        if (currentHP > maxHP){
            currentHP = maxHP;
        }
        Debug.Log("After currentHP : " + currentHP);

        slider.value = (float)currentHP / (float)maxHP;
        Debug.Log("slider.value : " + slider.value);
    }

    public void Guard(int value){
        guardValue += value;
    }

    public void Death(){
        Debug.Log("Player is dead");
        battleManager.GameOver();
    }

    public void TurnStart(){
        guardValue = 0;
    }

    void ChangeHP(){
        hp_text.text = $"{currentHP} / {maxHP}";
    }

    void ChangeGuard(){
        guard_text.text = $"{guardValue}";
    }
}
