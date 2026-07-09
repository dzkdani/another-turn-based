using UnityEngine;
using System;
using System.Collections.Generic;

public class BattleUnit : MonoBehaviour
{
    public UnitData Data { get; private set; }
    public UnitSO Definition { get; private set; }
    public Team Team { get; private set; }
    public bool IsDead { get; private set; }

    public event Action OnTurnStart;
    public event Action OnTurnEnd;
    public event Action OnAttack;
    public event Action<BattleUnit, BattleUnit, float> OnTakeDamage;
    public event Action OnDeath;

    public GameObject VisualInstance { get; private set; }

    [SerializeField]
    private BattleUnitVisual visual;
    public BattleUnitVisual Visual => visual;

    [SerializeField]
    private BattleAnimationBridge animationBridge;
    public BattleAnimationBridge AnimationBridge => animationBridge;


    private void Awake()
    {
        CacheComponents();
    }

    public void Setup(UnitSO definition, Team team)
    {
        Definition = definition;
        Team = team;

        InitializeData();
        SpawnVisual();
    }

    private void CacheComponents()
    {
        if (visual == null)
            visual = GetComponent<BattleUnitVisual>();

        if (animationBridge == null)
            animationBridge = GetComponent<BattleAnimationBridge>();
    }

    private void InitializeData()
    {
        if (Definition == null)
        {
            Debug.LogError($"{name} has no UnitSO assigned.");
            return;
        }

        Data = new UnitData
        {
            Name = Definition.Name,

            MaxHP = Definition.MaxHP,
            CurrentHP = Definition.MaxHP,

            CurrentAtk = Definition.Attack,
            CurrentDef = Definition.Defense,
            CurrentSpd = Definition.Speed,
            CurrentCritChance = Definition.CritChance
        };

        IsDead = false;
    }

    private void SpawnVisual()
    {
        if (Definition.VisualPrefab == null)
            return;

        if (VisualInstance != null)
            Destroy(VisualInstance);

        VisualInstance = Instantiate(
            Definition.VisualPrefab,
            transform);

        VisualInstance.transform.localPosition = Vector3.zero;
        VisualInstance.transform.localRotation = Quaternion.identity;

        visual = VisualInstance.GetComponentInChildren<BattleUnitVisual>();
        if (visual != null)
            visual.Initialize();
        animationBridge = VisualInstance.GetComponentInChildren<BattleAnimationBridge>();
        if (animationBridge != null)
            animationBridge.Initialize();

        if (visual == null)
            Debug.LogWarning($"{Definition.Name} VisualPrefab has no BattleUnitVisual.");

        if (animationBridge == null)
            Debug.LogWarning($"{Definition.Name} VisualPrefab has no BattleAnimationBridge.");
    }

    public BattleActionSO BasicAttack => Definition.BasicAttack;
    public IReadOnlyList<BattleActionSO> Skills => Definition.Skills;
    public AIBehaviorSO AIBehavior => Definition.AIBehavior;

    public void BeginTurn()
    {
        OnTurnStart?.Invoke();
    }

    public void EndTurn()
    {
        OnTurnEnd?.Invoke();
    }

    public void Attack()
    {
        OnAttack?.Invoke();
    }

    public void TakeDamage(BattleUnit attacker, int rawDamage)
    {
        if (IsDead)
            return;

        int finalDamage =
            (int)Mathf.Max(1f, rawDamage - Data.CurrentDef);

        Data.CurrentHP -= finalDamage;

        Data.CurrentHP = Mathf.Clamp(
            Data.CurrentHP,
            0,
            Data.MaxHP);

        BattleEvents.OnUnitDamaged?.Invoke(
            this,
            attacker,
            finalDamage);

        BattleEvents.OnUnitHPChanged?.Invoke(this);

        OnTakeDamage?.Invoke(
            attacker,
            this,
            finalDamage);

        if (Data.CurrentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead)
            return;

        Data.CurrentHP = 
            Mathf.Clamp(
                Data.CurrentHP + amount,
                0,
                Data.MaxHP);

        BattleEvents.OnUnitHPChanged?.Invoke(this);
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        OnDeath?.Invoke();

        BattleEvents.OnUnitDied?.Invoke(this);
    }
}