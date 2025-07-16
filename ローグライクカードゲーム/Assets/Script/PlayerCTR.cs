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

    BattleManager battleManager;

    int currentHP;
    int maxHP;
    int guardValue = 0;

    public int HP{
        get { return currentHP; }
        set {
            currentHP = Mathf.Clamp(value, 0, maxHP);
            if (currentHP <= 0){
                Death();
            }
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        slider.value = 1;
        maxHP = status.maxHP;
        currentHP = maxHP;
        guardValue = 0;
    }

    void Update(){
        string text = $"{HP} / {maxHP}";
        hp_text.text = text;

        text = $"{guardValue}";
        guard_text.text = text;
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
        HP -= damage;
        Debug.Log("After currentHP : " + HP);

        slider.value = (float)HP / (float)maxHP;
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
        HP -= damage;
        Debug.Log("After currentHP : " + HP);

        slider.value = (float)HP / (float)maxHP;
        Debug.Log("slider.value : " + slider.value);
    }

    public void Heal(int healValue){
        HP += healValue;
        if (HP > maxHP){
            HP = maxHP;
        }
        Debug.Log("After currentHP : " + HP);

        slider.value = (float)HP / (float)maxHP;
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
}
