public class PlayerDecision
{
    public BattleActionSO Action { get; }
    public BattleUnit Target { get; }

    public PlayerDecision(BattleActionSO action, BattleUnit target)
    {
        Action = action;
        Target = target;
    }
}