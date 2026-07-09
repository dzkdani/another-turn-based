using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    [Header("Presentation")]
    [SerializeField] private BattleVFXManager battleVFXManager;
    [SerializeField] private BattleAudioManager battleAudioManager;
    [SerializeField] private PostProcessController postProcessManager;
    [SerializeField] private ScreenDistortionController screenDistortionManager;

    public BattlePresentationContext Presentation { get; private set; }
    public BattleExecutionContext Execution { get; private set; }

    public TurnManager TurnManager { get; private set; }

    public BattleState CurrentState => battleFlow.CurrentState;
    public BattleUnit CurrentUnit => battleFlow.CurrentUnit;

    public IReadOnlyList<BattleUnit> PlayerUnits => playerUnits;
    public IReadOnlyList<BattleUnit> EnemyUnits => enemyUnits;

    private readonly List<BattleUnit> playerUnits = new();
    private readonly List<BattleUnit> enemyUnits = new();

    private TargetSystem targetSystem;
    private ActionSystem actionSystem;
    private BattleAISystem aiSystem;
    private BattleFlow battleFlow;

    #region Unity

    private void Awake()
    {

        TurnManager = new TurnManager();

        targetSystem = new TargetSystem();
        aiSystem = new BattleAISystem();

        if (battleVFXManager ==  null || 
            battleAudioManager == null || 
            postProcessManager == null || 
            screenDistortionManager == null)
        {
            Debug.LogWarning("Missing Inspector Components");
            return;
        }

        Presentation = new BattlePresentationContext(
            battleVFXManager,
            battleAudioManager,
            postProcessManager,
            screenDistortionManager);

        Execution = new BattleExecutionContext(
            TurnManager,
            targetSystem,
            Presentation);

        actionSystem = new ActionSystem(Execution);

        battleFlow = new BattleFlow(
            TurnManager,
            actionSystem,
            aiSystem,
            targetSystem);

        battleFlow.EnemyTurnStarted += HandleEnemyTurnStarted;
    }

    private void OnEnable()
    {
        BattleEvents.OnActionSelected += HandleActionSelected;
        BattleEvents.OnImmediateActionSelected += HandleImmediateActionSelected;
        BattleEvents.OnTargetSelected += HandleTargetSelected;
    }

    private void OnDisable()
    {
        BattleEvents.OnActionSelected -= HandleActionSelected;
        BattleEvents.OnImmediateActionSelected -= HandleImmediateActionSelected;
        BattleEvents.OnTargetSelected -= HandleTargetSelected;

        if (battleFlow != null)
            battleFlow.EnemyTurnStarted -= HandleEnemyTurnStarted;
    }

    #endregion

    public void InitializeBattle(
        List<BattleUnit> players,
        List<BattleUnit> enemies)
    {
        playerUnits.Clear();
        enemyUnits.Clear();

        playerUnits.AddRange(players);
        enemyUnits.AddRange(enemies);

        battleFlow.StartBattle(playerUnits, enemyUnits);
    }

    #region Event Handlers

    private void HandleActionSelected(BattleActionSO action)
    {
        if (CurrentState != BattleState.WaitingForCommand)
            return;

        if (action == null)
            return;

        battleFlow.OnPlayerActionSelected(action);
    }

    private void HandleTargetSelected(BattleUnit target)
    {
        if (CurrentState != BattleState.WaitingForTarget)
            return;

        StartCoroutine(
            battleFlow.HandleTargetSelected(
                target,
                playerUnits,
                enemyUnits));
    }

    private void HandleEnemyTurnStarted()
    {
        StartCoroutine(
            battleFlow.PerformEnemyTurn(
                playerUnits,
                enemyUnits));
    }

    private void HandleImmediateActionSelected(BattleActionSO action)
    {
        if (action == null)
            return;

        StartCoroutine(
            ExecuteImmediateAction(action));
    }

    #endregion

    private IEnumerator ExecuteImmediateAction(BattleActionSO action)
    {
        List<BattleUnit> targets =
            actionSystem.ResolveTargets(
                CurrentUnit,
                action,
                playerUnits,
                enemyUnits,
                null,
                targetSystem);

        if (targets.Count == 0)
        {
            battleFlow.EndCurrentTurn(playerUnits, enemyUnits);
            yield break;
        }

        CurrentUnit.Attack();

        yield return actionSystem.ExecuteAction(
            CurrentUnit,
            targets,
            action);

        battleFlow.EndCurrentTurn(
            playerUnits,
            enemyUnits);
    }
}