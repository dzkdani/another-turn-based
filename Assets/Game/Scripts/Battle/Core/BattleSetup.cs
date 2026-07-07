using System.Collections.Generic;
using UnityEngine;

public class BattleSetup : MonoBehaviour
{
    public static BattleSetup Instance { get; private set; }

    [System.Serializable]
    public class BattleParticipant
    {
        public UnitSO UnitSO;
        public Team Team;

        public BattleParticipant(UnitSO unitSO, Team team)
        {
            UnitSO = unitSO;
            Team = team;
        }
    }

    public IReadOnlyList<BattleParticipant> Participants => participants;

    private readonly List<BattleParticipant> participants = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Clear()
    {
        participants.Clear();
    }

    public void AddParticipant(UnitSO unitSO, Team team)
    {
        if (unitSO == null)
        {
            Debug.LogWarning("Trying to add a null UnitSO.");
            return;
        }

        participants.Add(new BattleParticipant(unitSO, team));
    }

    /// <summary>
    /// Creates a simple 1 vs 1 battle.
    /// </summary>
    public void CreateBattle(UnitSO player, CharacterDatabase characterDatabase)
    {
        Clear();

        AddParticipant(player, Team.Player);

        UnitSO enemy = characterDatabase.GetRandomCharacter(player);
        
        AddParticipant(enemy, Team.Enemy);
    }

    public bool HasValidSelection()
    {
        if (participants.Count == 0)
            return false;

        bool hasPlayer = false;
        bool hasEnemy = false;

        foreach (var participant in participants)
        {
            switch (participant.Team)
            {
                case Team.Player:
                    hasPlayer = true;
                    break;

                case Team.Enemy:
                    hasEnemy = true;
                    break;
            }
        }

        return hasPlayer && hasEnemy;
    }
}