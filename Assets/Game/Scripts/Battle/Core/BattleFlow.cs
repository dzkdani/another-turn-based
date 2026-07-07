using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class BattleFlow
{
    private readonly TurnManager turnManager;
    private readonly ActionSystem actionSystem; // Sistem lama Anda untuk kalkulasi damage/aksi
    private readonly BattleAISystem aiSystem;
    private readonly TargetSystem targetSystem;

    private BattleActionSO currentAction;
    private BattleUnit currentTarget;

    public BattleState CurrentState { get; private set; }
    public BattleUnit CurrentUnit { get; private set; }
    
    public event Action EnemyTurnStarted;

    public BattleFlow(
        TurnManager turnManager,
        ActionSystem actionSystem,
        BattleAISystem aiSystem,
        TargetSystem targetSystem)
    {
        this.turnManager = turnManager;
        this.actionSystem = actionSystem;
        this.aiSystem = aiSystem;
        this.targetSystem = targetSystem;
    }

    public void StartBattle(List<BattleUnit> players, List<BattleUnit> enemies)
    {
        CurrentState = BattleState.Setup;

        List<BattleUnit> allUnits = new List<BattleUnit>();
        allUnits.AddRange(players);
        allUnits.AddRange(enemies);

        // TurnManager sekarang menggunakan inisialisasi HSR Action Value
        turnManager.Initialize(allUnits);
        StartTurn();
    }

    private void StartTurn()
    {
        CurrentUnit = turnManager.GetCurrentUnit();
        currentAction = null;
        currentTarget = null;
        targetSystem.ClearSelectedTarget();
        BattleEvents.OnTargetSelected?.Invoke(null);

        if (CurrentUnit == null)
        {
            Debug.Log("No current turn unit.");
            return;
        }

        Debug.Log($"{CurrentUnit.Data.Name} Turn");

        // MEMUHI REQ: Memanggil Event OnTurnStart bawaan unit
        CurrentUnit.OnTurnStart?.Invoke();

        // REFAKTOR KUNCI: Cek faksi dinamis, bukan enum tim statis dari SO
        if (CurrentUnit.Team == Team.Player)
        {
            CurrentState = BattleState.WaitingForCommand;
            BattleEvents.OnPlayerTurn?.Invoke();
            return;
        }

        // Jika faksi adalah Enemy, serahkan ke AI
        CurrentState = BattleState.Executing;
        BattleEvents.OnEnemyTurn?.Invoke();
        EnemyTurnStarted?.Invoke();
    }

    public IEnumerator PerformEnemyTurn(
        List<BattleUnit> players,
        List<BattleUnit> enemies)
    {
        if (CurrentUnit == null)
            yield break;

        yield return new WaitForSeconds(1f);

        // AI memilih action
        BattleActionSO action =
            aiSystem.ChooseAction(CurrentUnit);

        if (action == null)
        {
            EndCurrentTurn(players, enemies);
            yield break;
        }

        // AI memilih target berdasarkan action
        BattleUnit target =
            aiSystem.ChooseTarget(
                CurrentUnit,
                action,
                players,
                enemies);

        if (target == null)
        {
            EndCurrentTurn(players, enemies);
            yield break;
        }

        List<BattleUnit> targets = new()
        {
            target
        };

        CurrentUnit.OnAttack?.Invoke();

        yield return actionSystem.ExecuteAction(
            CurrentUnit,
            targets,
            action);

        EndCurrentTurn(players, enemies);
    }

    public void OnPlayerActionSelected(BattleActionSO action)
    {
        if (CurrentState != BattleState.WaitingForCommand) return;

        currentAction = action;

        if (action.TargetRequirement == TargetRequirement.None)
        {
            CurrentState = BattleState.Executing;
            BattleEvents.OnImmediateActionSelected?.Invoke(action);
            return;
        }

        CurrentState = BattleState.WaitingForTarget;
        Debug.Log($"Select target for {action.ActionName}");
    }

    public IEnumerator HandleTargetSelected(BattleUnit target, List<BattleUnit> players, List<BattleUnit> enemies)
    {
        if (CurrentState != BattleState.WaitingForTarget) yield break;

        currentTarget = target;
        targetSystem.SetSelectedTarget(target);

        if (currentAction == null)
        {
            CurrentState = BattleState.WaitingForCommand;
            yield break;
        }

        CurrentState = BattleState.Executing;

        List<BattleUnit> targets = actionSystem.ResolveTargets(
            CurrentUnit, currentAction, players, enemies, currentTarget, targetSystem);

        if (targets.Count == 0)
        {
            CurrentState = BattleState.WaitingForCommand;
            yield break;
        }

        // Memenuhi REQ: Panggil event OnAttack milik Player
        CurrentUnit.OnAttack?.Invoke();

        yield return actionSystem.ExecuteAction(CurrentUnit, targets, currentAction);
        EndCurrentTurn(players, enemies);
    }

    public void EndCurrentTurn(List<BattleUnit> players, List<BattleUnit> enemies)
    {
        // REFAKTOR KUNCI: Cek kondisi menang/kalah berdasarkan sisa unit yang hidup di list dinamis
        if (AreAllEnemiesDead(enemies))
        {
            CurrentState = BattleState.Victory;
            BattleEvents.OnVictory?.Invoke();
            return;
        }

        if (AreAllPlayersDead(players))
        {
            CurrentState = BattleState.Defeat;
            BattleEvents.OnDefeat?.Invoke();
            return;
        }

        // Maju ke antrean AV berikutnya di TurnManager HSR
        turnManager.AdvanceTurn();
        StartTurn();
    }

    private bool AreAllEnemiesDead(List<BattleUnit> enemies)
    {
        return enemies.All(x => x == null || x.IsDead);
    }

    private bool AreAllPlayersDead(List<BattleUnit> players)
    {
        return players.All(x => x == null || x.IsDead);
    }
}