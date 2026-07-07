public class BattleExecutionContext
{
    public BattlePresentationContext Presentation { get; }
    public TurnManager TurnManager { get; }
    public TargetSystem TargetSystem { get; }

    public BattleExecutionContext(
        TurnManager turnManager,
        TargetSystem targetSystem,
        BattlePresentationContext presentation)
    {
        TurnManager = turnManager;
        TargetSystem = targetSystem;
        Presentation = presentation;
    }
}