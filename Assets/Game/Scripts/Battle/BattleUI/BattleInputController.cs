using System.Threading.Tasks;
using UnityEngine;

public class BattleInputController : MonoBehaviour
{
    private TaskCompletionSource<PlayerDecision> pendingDecision;

    /// <summary>
    /// Called by BattleFlow when a player turn begins.
    /// Waits until the UI submits a decision.
    /// </summary>
    public async Task<PlayerDecision> WaitForPlayerDecision(BattleUnit actingUnit)
    {
        if (pendingDecision != null)
        {
            Debug.LogWarning("Already waiting for player input.");
            return null;
        }

        pendingDecision = new TaskCompletionSource<PlayerDecision>();

        return await pendingDecision.Task;
    }

    /// <summary>
    /// Called by the UI when the player has finished choosing.
    /// </summary>
    public void SubmitDecision(PlayerDecision decision)
    {
        if (pendingDecision == null)
            return;

        pendingDecision.TrySetResult(decision);
        pendingDecision = null;
    }

    /// <summary>
    /// Clears any pending decision.
    /// Useful if the battle ends while waiting for input.
    /// </summary>
    public void CancelWaiting()
    {
        if (pendingDecision == null)
            return;

        pendingDecision.TrySetCanceled();
        pendingDecision = null;
    }

    public bool IsWaitingForInput => pendingDecision != null;
}