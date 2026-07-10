using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Collider))]
public class BattleUnitClickHandler : MonoBehaviour
{
    [SerializeField] private BattleUnit battleUnit;
    public BattleUnit BattleUnit => battleUnit;

    public static event Action<BattleUnit> OnUnitClicked;

    private void Awake()
    {
        if (battleUnit == null)
            battleUnit = GetComponentInParent<BattleUnit>();
        
        if (battleUnit == null)
            return;
    }
    
    public void Click()
    {
        if (battleUnit == null || battleUnit.IsDead)
            return;

        Debug.Log($"Clicked {battleUnit.Data.Name}");

        OnUnitClicked?.Invoke(battleUnit);
    }
}