using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleVFXManager battleVFXManager;
    [SerializeField] private BattleAudioManager battleAudioManager;

    public BattlePresentationContext Presentation { get; private set; }
    public TurnManager TurnManager { get; private set; }
    public BattleState CurrentState => battleFlow.CurrentState;
    public BattleUnit CurrentUnit => battleFlow.CurrentUnit;

    public List<BattleUnit> PlayerUnits;
    public List<BattleUnit> EnemyUnits;

    private TargetSystem targetSystem;
    private ActionSystem actionSystem;
    private BattleAISystem aiSystem;
    private BattleFlow battleFlow;

    private void OnEnable()
    {
        BattleEvents.OnAttackPressed += OnAttackPressed;
        BattleEvents.OnRunPressed += OnRunPressed;
        BattleEvents.OnTargetSelected += OnTargetSelected;
        BattleEvents.OnActionSelected += OnActionSelected;
        BattleEvents.OnImmediateActionSelected += OnImmediateActionSelected;
    }

    private void OnDisable()
    {
        BattleEvents.OnAttackPressed -= OnAttackPressed;
        BattleEvents.OnRunPressed -= OnRunPressed;
        BattleEvents.OnTargetSelected -= OnTargetSelected;
        BattleEvents.OnActionSelected -= OnActionSelected;
        BattleEvents.OnImmediateActionSelected -= OnImmediateActionSelected;
        
        if (battleFlow != null)
            battleFlow.EnemyTurnStarted -= OnEnemyTurnStarted;
    }

    private void Awake()
    {
        TurnManager = new TurnManager();
        targetSystem = new TargetSystem();
        // actionSystem = new ActionSystem();
        aiSystem = new BattleAISystem();
        Presentation = new BattlePresentationContext(
            battleVFXManager,
            battleAudioManager);
        actionSystem = new ActionSystem(Presentation);
        
        battleFlow = new BattleFlow(TurnManager, actionSystem, aiSystem, targetSystem);
        battleFlow.EnemyTurnStarted += OnEnemyTurnStarted;
    }

    public void InitializeBattle(List<BattleUnit> players, List<BattleUnit> enemies)
    {
        PlayerUnits = players;
        EnemyUnits = enemies;

        battleFlow.StartBattle(PlayerUnits, EnemyUnits);
    }

    private void OnAttackPressed()
    {
        if(CurrentState != BattleState.WaitingForCommand) return;

        // Ambil skill attack dari data unit runtime
        BattleActionSO attackAction = CurrentUnit?.Data.Skills.FirstOrDefault(a => a.ActionType == BattleActionType.Attack);
        BattleEvents.OnActionSelected?.Invoke(attackAction);
    }

    private void OnRunPressed()
    {
        if(CurrentState != BattleState.WaitingForCommand) return;

        BattleActionSO runAction = CurrentUnit?.Data.Skills.FirstOrDefault(a => a.ActionType == BattleActionType.Run);
        BattleEvents.OnActionSelected?.Invoke(runAction);
    }

    private void OnActionSelected(BattleActionSO action)
    {
        battleFlow.OnPlayerActionSelected(action);
    }

    private void OnTargetSelected(BattleUnit target)
    {
        StartCoroutine(battleFlow.HandleTargetSelected(target, PlayerUnits, EnemyUnits));
    }

    private void OnEnemyTurnStarted()
    {
        StartCoroutine(battleFlow.PerformEnemyTurn(PlayerUnits, EnemyUnits));
    }

    private void OnImmediateActionSelected(BattleActionSO action)
    {
        StartCoroutine(ExecuteImmediateAction(action));
    }

    private IEnumerator ExecuteImmediateAction(BattleActionSO action)
    {
        List<BattleUnit> targets = actionSystem.ResolveTargets(
            CurrentUnit, action, PlayerUnits, EnemyUnits, null, targetSystem);

        CurrentUnit.OnAttack?.Invoke(); // Picu event serang

        yield return actionSystem.ExecuteAction(CurrentUnit, targets, action);

        battleFlow.EndCurrentTurn(PlayerUnits, EnemyUnits);
    }
}