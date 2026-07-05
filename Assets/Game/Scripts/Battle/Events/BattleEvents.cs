using System;

public static class BattleEvents
{
    public static Action<BattleUnit> OnUnitDamaged;
    // public static Action<string> OnBattleLog;
    public static Action<BattleActionSO> OnActionSelected;
    public static Action<BattleActionSO> OnImmediateActionSelected;
    public static Action<BattleUnit> OnTargetSelected;
    public static Action OnAttackPressed;
    public static Action OnRunPressed;
    public static Action OnPlayerTurn;
    public static Action OnEnemyTurn;
    public static Action OnVictory;
    public static Action OnDefeat;
}