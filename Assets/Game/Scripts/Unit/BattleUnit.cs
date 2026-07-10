using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class BattleUnit : MonoBehaviour
{
    public UnitData Data { get; private set; }
    public UnitSO Definition { get; private set; }
    public Team Team { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<BattleUnit, BattleUnit, float> OnTakeDamage;

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

        if (visual == null)
            Debug.LogWarning($"{Definition.Name} VisualPrefab has no BattleUnitVisual."); 
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
        
        if (animationBridge == null)
            animationBridge = GetComponentInChildren<BattleAnimationBridge>();
        if (animationBridge == null)
            Debug.LogWarning($"{Definition.Name} VisualPrefab has no BattleAnimationBridge.");
            

        if (visual != null)
            visual.Initialize();
        if (animationBridge != null)
            animationBridge.Initialize();

    }

    public BattleActionSO BasicAttack => Definition.BasicAttack;
    public IReadOnlyList<BattleActionSO> Actions => Definition.Actions;
    public AIBehaviorSO AIBehavior => Definition.AIBehavior;

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

        Debug.Log($"{Data.Name} took {rawDamage} damage from {attacker.Data.Name}");

        if (Data.CurrentHP <= 0)
        {
            StartCoroutine(Die());
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

    private IEnumerator Die()
    {
        if (IsDead)
            yield break;

        IsDead = true;

        yield return animationBridge.PlayDieUntilFinished();

        BattleEvents.OnUnitDied?.Invoke(this);

        gameObject.SetActive(false);
    }
}