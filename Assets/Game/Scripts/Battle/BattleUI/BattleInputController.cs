using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class BattleInputController : MonoBehaviour
{
    [SerializeField] private Camera battleCamera;

    private TargetSystem targetSystem;

    public void Initialize(TargetSystem targetSystem)
    {
        this.targetSystem = targetSystem;
    }

    private void Awake()
    {
        if (battleCamera == null)
            battleCamera = Camera.main;
    }

    private void Update()
    {
        if (targetSystem == null)
            return;

        if (!targetSystem.IsSelecting)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = battleCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        BattleUnitClickHandler clickHandler =
            hit.collider.GetComponent<BattleUnitClickHandler>();

        if (clickHandler == null)
            return;

        clickHandler.Click();

        BattleUnit unit = clickHandler.BattleUnit;

        if (unit == null || unit.IsDead)
            return;

    }
}