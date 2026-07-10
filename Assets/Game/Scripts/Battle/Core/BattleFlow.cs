    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using UnityEngine;

    public class BattleFlow
    {
        private readonly TurnManager turnManager;
        private readonly ActionSystem actionSystem; 
        private readonly BattleAISystem aiSystem;
        private readonly TargetSystem targetSystem;
        private readonly BattleUIController battleUI;

        private BattleActionSO currentAction;
        private BattleUnit currentTarget;

        public BattleState CurrentState { get; private set; }
        public BattleUnit CurrentUnit { get; private set; }
        
        public event Action EnemyTurnStarted;

        public BattleFlow(
            TurnManager turnManager,
            ActionSystem actionSystem,
            BattleAISystem aiSystem,
            TargetSystem targetSystem,
            BattleUIController battleUIController)
        {
            this.turnManager = turnManager;
            this.actionSystem = actionSystem;
            this.aiSystem = aiSystem;
            this.targetSystem = targetSystem;
            this.battleUI = battleUIController;
        }

        public void SetState(BattleState state)
        {
            CurrentState = state;
            Debug.Log($"Battle State -> {state}");
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
            Debug.Log($"Current Turn : {CurrentUnit.Data.Name}");
            currentAction = null;
            currentTarget = null;
            targetSystem.ClearSelectedTarget();
            BattleEvents.OnTargetSelected?.Invoke(null);

            if (CurrentUnit == null)
            {
                Debug.Log("No current turn unit.");
                return;
            }

            CurrentUnit.BeginTurn();

            // REFAKTOR KUNCI: Cek faksi dinamis, bukan enum tim statis dari SO
            if (CurrentUnit.Team == Team.Player)
            {
                CurrentState = BattleState.WaitingForCommand;
                BattleEvents.OnPlayerTurnStarted?.Invoke(CurrentUnit);
                battleUI.ShowCommands(CurrentUnit);
                return;
            }

            // Jika faksi adalah Enemy, serahkan ke AI
            CurrentState = BattleState.Executing;
            BattleEvents.OnEnemyTurnStarted?.Invoke(currentTarget);
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

            CurrentUnit.Attack();

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
        }

        public IEnumerator HandleTargetSelected(BattleUnit target, List<BattleUnit> players, List<BattleUnit> enemies)
        {
            Debug.Log($"Entered HandleTargetSelected. State = {CurrentState}");
            
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

            battleUI.HideCommands();
            battleUI.DisableTargetSelection();

            CurrentUnit.Attack();

            yield return actionSystem.ExecuteAction(CurrentUnit, targets, currentAction);
            EndCurrentTurn(players, enemies);
        }

        public void EndCurrentTurn(List<BattleUnit> players, List<BattleUnit> enemies)
        {
            Debug.Log($"{CurrentUnit.Data.Name} ending turn");
            CurrentUnit.EndTurn();

            CleanupDeadUnits(players, enemies);

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

        private void CleanupDeadUnits(List<BattleUnit> players, List<BattleUnit> enemies)
        {
            CleanupFaction(players);
            CleanupFaction(enemies);
        }

        private void CleanupFaction(List<BattleUnit> units)
        {
            for (int i = units.Count - 1; i >= 0; i--)
            {
                BattleUnit unit = units[i];

                if (!unit.IsDead)
                    continue;

                turnManager.RemoveUnit(unit);

                battleUI.RemoveHUD(unit);

                targetSystem.RemoveTarget(unit);

                units.RemoveAt(i);
            }
        }
    }