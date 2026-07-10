using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "ScriptableObjects/Unit/Character", order = 1)]
public class UnitSO : ScriptableObject
{
    [Header("Identity")]
    public string Name;

    [TextArea]
    public string Description;

    [Header("Visuals")]
    public Sprite Portrait;              // Character Select UI
    public Sprite FullArtwork;           // Optional large artwork
    public GameObject VisualPrefab;      // Battle prefab

    [Header("Base Stats")]
    public int MaxHP;
    public int Attack;
    public int Defense;
    public int Speed;
    [Range(0f, 1f)]
    public float CritChance = 0.05f;

    [Header("Battle")]
    public BattleActionSO BasicAttack;
    public List<BattleActionSO> Actions;

    [Header("AI")]
    public AIBehaviorSO AIBehavior;
}