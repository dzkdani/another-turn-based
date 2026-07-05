using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "ScriptableObjects/Unit/Character", order = 1)]
public class UnitSO : ScriptableObject
{
    [Header("Identity & Visuals")]
    public string Name;
    // Prefab 3D model (Karakter dummy Mixamo Anda) yang akan di-spawn ke arena
    public GameObject VisualPrefab; 

    [Header("Base Stats")]
    public int MaxHP;
    public int Attack;
    public int Defense;
    public int Speed; // Krusial untuk Action Value HSR di TurnManager
    public float CritChance = 0.05f;

    [Header("Status")]
    public Team Team;

    [Header("AI & Move Sets")]
    public AIBehaviorSO AIBehavior;
    public List<BattleActionSO> Skills;
}