using System;

public static class BattleEvents
{
    // ===========================
    // Battle Lifecycle
    // ===========================

    /// <summary>
    /// Fired when a player's turn begins.
    /// </summary>
    public static Action<BattleUnit> OnPlayerTurnStarted;

    /// <summary>
    /// Fired when an enemy's turn begins.
    /// </summary>
    public static Action<BattleUnit> OnEnemyTurnStarted;

    public static Action OnVictory;
    public static Action OnDefeat;

    // ===========================
    // Player Input
    // ===========================

    /// <summary>
    /// UI submits the selected battle action.
    /// </summary>
    public static Action<BattleActionSO> OnActionSelected;

    /// <summary>
    /// Used when an action doesn't require manual target selection.
    /// (Self Buff, All Enemies, etc.)
    /// </summary>
    public static Action<BattleActionSO> OnImmediateActionSelected;

    /// <summary>
    /// UI submits the selected target.
    /// </summary>
    public static Action<BattleUnit> OnTargetSelected;

    // ===========================
    // Combat
    // ===========================

    /// <summary>
    /// Fired after damage has been successfully applied.
    /// </summary>
    public static Action<BattleUnit, BattleUnit, float> OnUnitDamaged;

    /// <summary>
    /// Fired whenever HP changes
    /// (Damage, Heal, Revive, etc.)
    /// </summary>
    public static Action<BattleUnit> OnUnitHPChanged;

    /// <summary>
    /// Fired when a unit dies.
    /// </summary>
    public static Action<BattleUnit> OnUnitDied;
}