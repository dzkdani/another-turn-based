using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    [Header("Presentation")]
    [SerializeField] private BattleUIController battleUIController;
    [SerializeField] private BattleVFXManager battleVFXManager;
    [SerializeField] private BattleAudioManager battleAudioManager;
    [SerializeField] private PostProcessController postProcessManager;
    [SerializeField] private ScreenDistortionController screenDistortionManager;
    [SerializeField] private BattleCameraManager battleCameraManager;
    [SerializeField] private BattleInputController battleInputController;

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

        battleInputController.Initialize(targetSystem);

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
            screenDistortionManager,
            battleCameraManager);

        Execution = new BattleExecutionContext(
            TurnManager,
            targetSystem,
            Presentation);

        actionSystem = new ActionSystem(Execution);

        battleFlow = new BattleFlow(
            TurnManager,
            actionSystem,
            aiSystem,
            targetSystem,
            battleUIController);

        battleFlow.EnemyTurnStarted += HandleEnemyTurnStarted;
    }

    private void OnEnable()
    {
        BattleEvents.OnActionSelected += HandleActionSelected;
        BattleEvents.OnImmediateActionSelected += HandleImmediateActionSelected;
        BattleUnitClickHandler.OnUnitClicked += HandleUnitClicked;
    }

    private void OnDisable()
    {
        BattleEvents.OnActionSelected -= HandleActionSelected;
        BattleEvents.OnImmediateActionSelected -= HandleImmediateActionSelected;
        BattleUnitClickHandler.OnUnitClicked -= HandleUnitClicked;

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
        battleUIController.Initialize(playerUnits, enemyUnits);
    }

    #region Event Handlers

    private void HandleActionSelected(BattleActionSO action)
    {
        if (CurrentState != BattleState.WaitingForCommand)
            return;

        if (action == null)
            return;

        battleFlow.OnPlayerActionSelected(action);

        targetSystem.BeginSelection(
            CurrentUnit,
            action,
            playerUnits,
            enemyUnits);
    }

    private void HandleUnitClicked(BattleUnit unit)
    {
        bool valid = targetSystem.SelectTarget(unit);
        
        if (!valid)
            return;

        StartCoroutine(
            battleFlow.HandleTargetSelected(
                unit,
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

        yield return actionSystem.ExecuteAction(
            CurrentUnit,
            targets,
            action);

        battleFlow.EndCurrentTurn(
            playerUnits,
            enemyUnits);
    }
}