using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;}
    [SerializeField]
    public List<CardData> Deck = new List<CardData>();
    [SerializeField]
    CharacterStats playerStats;
    [SerializeField]
    GameObject player;
    [SerializeField]
    FloorEvent[] events;
    [SerializeField]
    bool isEventRunning;
    public int floorCount  {get; private set;}
    bool inBattle;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        player.GetComponent<PlayerCTR>().status = Instantiate(playerStats);
        isEventRunning = false;
        floorCount = 0;
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        inBattle = false;
    }

    void Start(){
        
    }

    void Update(){
        if(isEventRunning || inBattle) return;
        isEventRunning = true;
        floorCount++;
        EventUI.Instance.ShowFloorCountUI(floorCount, () =>{
            // Execute a random event from the events array
            int randomIndex = Random.Range(0, events.Length);
            events[randomIndex].Execute(player.GetComponent<PlayerCTR>(), () => {
                Debug.Log("Event completed");
                isEventRunning = false;
            });
        });
    }

    public void AddCard(CardData cardData){
        Deck.Add(cardData);
    }

    public void StartBattle(){
        inBattle = true;
    }

    public void EndBattle(){
        inBattle = false;
    }

    public void StartEventBattle(List<EnemyStats> enemies){
        inBattle = true;
        BattleManager.Instance.StartBattle(false, enemies);
    }
}