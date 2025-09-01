using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour{
    [System.Serializable]
    public enum TurnPhase{
        BattleStart,
        PlayerTurnStart,
        PlayerTurnMain,
        PlayerTurnEnd,
        EnemyTurnStart,
        EnemyTurnMain,
        EnemyTurnEnd,
        BattleEnd,
        _MAX,
    }

    [Header("ターン管理")]
    TurnPhase _currentPhase;
    [ReadOnly] TurnPhase currentPhase{ 
        get { return _currentPhase;}
        set {
            _currentPhase = value;
            if(phaseLabel) phaseLabel.text = $"Phase: {_currentPhase}";
        }
    }
    int _turnCount;
    [ReadOnly] int turnCount{
        get {return _turnCount;}
        set {
            _turnCount = value;
            if(turnCountLabel) turnCountLabel.text = $"Turn: {_turnCount}";
        }
    }
    [ReadOnly] bool isTransitioning;
    [ReadOnly] bool isBattleEnded;

    public static event Action<TurnPhase> OnPhaseChanged;
    public static event Action<int> OnTurnCountChanged;

    BattleManager bm => BattleManager.Instance;

    // Debug用のUI
    [Header("Debug用UI")]
    [SerializeField]
    Text turnCountLabel;
    [SerializeField]
    Text phaseLabel;

    public void StartBattle(){
        turnCount = 0;
        isBattleEnded = false;
        ChangePhase(TurnPhase.BattleStart);
    }

    public void ChangePhase(TurnPhase newPhase){
        if (isTransitioning) return;

        Debug.Log($"フェーズ変更: {currentPhase} -> {newPhase}");
        currentPhase = newPhase;
        OnPhaseChanged?.Invoke(newPhase);

        StartCoroutine(ExecutePhase(newPhase));
    }

    IEnumerator ExecutePhase(TurnPhase phase){
        isTransitioning = true;

        switch(phase){
            case TurnPhase.BattleStart:
                yield return StartCoroutine(BattleStartPhase());
                if(!isBattleEnded){
                    isTransitioning = false;
                    ChangePhase(TurnPhase.PlayerTurnStart);
                }
                break;
            
            case TurnPhase.PlayerTurnStart:
                yield return StartCoroutine(PlayerTurnStartPhase());
                if(!isBattleEnded){
                    isTransitioning = false;
                    ChangePhase(TurnPhase.PlayerTurnMain);
                }
                break;
            
            case TurnPhase.PlayerTurnMain:
                yield return StartCoroutine(PlayerTurnMainPhase());
                if(!isBattleEnded){
                    isTransitioning = false;
                }
                break;
            
            case TurnPhase.PlayerTurnEnd:
                yield return StartCoroutine(PlayerTurnEndPhase());
                if(!isBattleEnded){
                    isTransitioning = false;
                    ChangePhase(TurnPhase.EnemyTurnStart);
                }
                break;
            
            case TurnPhase.EnemyTurnStart:
                yield return StartCoroutine(EnemyTurnStartPhase());
                if(!isBattleEnded){
                    isTransitioning = false;
                    ChangePhase(TurnPhase.EnemyTurnMain);
                }
                break;
            
            case TurnPhase.EnemyTurnMain:
                yield return StartCoroutine(EnemyTurnMainPhase());
                if(!isBattleEnded){
                    isTransitioning = false;
                    ChangePhase(TurnPhase.EnemyTurnEnd);
                }
                break;
            
            case TurnPhase.EnemyTurnEnd:
                yield return StartCoroutine(EnemyTurnEndPhase());
                if(!isBattleEnded){
                    isTransitioning = false;
                    ChangePhase(TurnPhase.PlayerTurnStart);
                }
                break;
            
            case TurnPhase.BattleEnd:
                yield return StartCoroutine(BattleEndPhase());
                isTransitioning = false;
                break;
        }
    }

    IEnumerator BattleStartPhase(){
        Debug.Log("=== バトル開始フェーズ ===");
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator PlayerTurnStartPhase(){
        Debug.Log("=== プレイヤー開始フェーズ ===");

        turnCount++;
        OnTurnCountChanged?.Invoke(turnCount);
        bm.player.GetComponent<IDamageable>().TurnStart();
        bm.RestoreMana();

        yield return StartCoroutine(bm.HandOutCardsCoroutine(bm.N));
        bm.DeterminesEnemyActions();

        Debug.Log($"プレイヤーターン開始完了");
    }

    IEnumerator PlayerTurnMainPhase(){
        Debug.Log("=== プレイヤーメインフェーズ ===");
        bm.EnablePlayerActions();
        yield return null;
    }

    IEnumerator PlayerTurnEndPhase(){
        Debug.Log("=== プレイヤーターン終了フェーズ ===");
        bm.DisablePlayerActions();
        yield return StartCoroutine(bm.DiscardHandCoroutine(() => {}));

        Debug.Log("プレイヤーターン終了完了");
    }

    IEnumerator EnemyTurnStartPhase(){
        Debug.Log("=== 敵ターン開始フェーズ ===");
        foreach (GameObject enemy in bm.enemies.ToList()){
            enemy.GetComponent<IDamageable>().TurnStart();
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator EnemyTurnMainPhase(){
        Debug.Log("=== 敵メインフェーズ ===");
        foreach (EnemyAction action in bm.enemyActions.ToList()){
            bm.DoEnemyAction(action);
            yield return new WaitForSeconds(0.4f);
        }

        bm.enemyActions.Clear();
    }

    IEnumerator EnemyTurnEndPhase(){
        Debug.Log("=== 敵ターン終了フェーズ ===");
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator BattleEndPhase(){
        Debug.Log("=== バトル終了フェーズ ===");

        isTransitioning = true;
        bm.EndBattle();
        yield return null;
    }

    public void PlayerEndTurn(){
        if (currentPhase == TurnPhase.PlayerTurnMain){
            ChangePhase(TurnPhase.PlayerTurnEnd);
        }
    }

    public void EndBattle(){
        Debug.Log("バトル終了！");
        isBattleEnded = true;
        StopAllCoroutines();

        isTransitioning = false;
        ChangePhase(TurnPhase.BattleEnd);
    }

    public bool IsPlayerTurn(){
        return currentPhase == TurnPhase.PlayerTurnMain;
    }

    public bool IsEnemyTurn(){
        return currentPhase == TurnPhase.EnemyTurnMain;
    }

    public bool CanPlayerAct(){
        return currentPhase == TurnPhase.PlayerTurnMain && !isTransitioning;
    }

    public TurnPhase CurrentPhase => currentPhase;
    public int CurrentTurnCount => turnCount;
    public bool IsTransitioning => isTransitioning;
}