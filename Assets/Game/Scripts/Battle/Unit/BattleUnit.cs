using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class BattleUnit : MonoBehaviour
{
    public UnitData Data { get; private set; }

    public UnitSO UnitSO { get; private set; }

    public Team Team { get; private set; }

    public bool IsDead { get; private set; }

    public Action OnTurnStart;
    public Action OnTurnEnd;
    public Action OnAttack;
    public Action<int> OnTakeDamage;
    public Action OnDeath;

    private GameObject visualInstance;
    [SerializeField] private BattleUnitVisual visual;
    public BattleUnitVisual Visual => visual;
    [SerializeField] private BattleAnimationBridge animationBridge;
    public BattleAnimationBridge AnimationBridge => animationBridge;

    private void Awake()
    {
        if (visual == null)
            visual = GetComponent<BattleUnitVisual>();
        if (animationBridge == null)
            animationBridge = GetComponent<BattleAnimationBridge>();
    }

    public void Setup(UnitSO unitSO, Team team)
    {
        UnitSO = unitSO;
        Team = team;

        InitializeData();
        SpawnVisual();
    }

    private void InitializeData()
    {
        if (UnitSO == null)
        {
            Debug.LogError("BattleUnit has no UnitSO.");
            return;
        }

        Data = new UnitData
        {
            Name = UnitSO.Name,
            MaxHP = UnitSO.MaxHP,
            CurrentHP = UnitSO.MaxHP,
            CurrentAtk = UnitSO.Attack,
            CurrentDef = UnitSO.Defense,
            CurrentSpd = UnitSO.Speed,
            CurrentCritChance = UnitSO.CritChance,
            Skills = new List<BattleActionSO>(UnitSO.Skills)
        };

        IsDead = false;
    }

    private void SpawnVisual()
    {
        if (UnitSO.VisualPrefab == null)
            return;

        if (visualInstance != null)
            Destroy(visualInstance);

        visualInstance = Instantiate(
            UnitSO.VisualPrefab,
            transform);

        visualInstance.transform.localPosition = Vector3.zero;
        visualInstance.transform.localRotation = Quaternion.identity;
    }

    public BattleActionSO GetAction(BattleActionType type)
    {
        return Data.Skills.Find(x => x.ActionType == type);
    }

    public IReadOnlyList<BattleActionSO> GetAllActions()
    {
        return Data.Skills;
    }

    public bool HasAction(BattleActionType type)
    {
        return GetAction(type) != null;
    }

    public BattleActionSO GetDefaultAction()
    {
        return Data.Skills.FirstOrDefault();
    }

    public BattleActionSO GetRandomAction()
    {
        return Data.Skills[
            UnityEngine.Random.Range(0, Data.Skills.Count)];
    }

    public AIBehaviorSO AIBehavior =>
        UnitSO != null ? UnitSO.AIBehavior : null;

    public void TakeDamage(int rawDamage)
    {
        if (IsDead)
            return;

        int finalDamage =
            Mathf.Max(1, rawDamage - Data.CurrentDef);

        Data.CurrentHP -= finalDamage;
        Data.CurrentHP =
            Mathf.Clamp(Data.CurrentHP, 0, Data.MaxHP);

        BattleEvents.OnUnitDamaged?.Invoke(this);

        OnTakeDamage?.Invoke(finalDamage);

        Debug.Log($"{Data.Name} took {finalDamage} damage.");

        if (Data.CurrentHP <= 0)
        {
            IsDead = true;
            OnDeath?.Invoke();
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
    }
}