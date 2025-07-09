using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage Data")]
public class StageData : ScriptableObject
{
    public string stageName_JP;
    public string stageName_EN;
    public List<EnemyStats> enemiesInThisStage;
    [Header ("敵の上限値")]
    public int startEnemy;
    [Header ("出現する敵は固定かどうか")]
    public bool isFixed;
}