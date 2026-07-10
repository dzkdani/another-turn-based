using System.Collections.Generic;
using UnityEngine;

public class BattleSetup : MonoBehaviour
{
    public static BattleSetup Instance { get; private set; }

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

    public int GetTeamCount(Team team)
    {
        int count = 0;

        foreach (var participant in participants)
        {
            if (participant.Team == team)
                count++;
        }

        return count;
    }

    public bool HasValidSelection()
    {
        return GetTeamCount(Team.Player) == 2 &&
               GetTeamCount(Team.Enemy) == 2;
    }

    public void CreateBattle(
        IReadOnlyList<UnitSO> players,
        EncounterDataSO encounter,
        CharacterDatabase database)
    {
        Clear();

        foreach (UnitSO player in players)
        {
            AddParticipant(player, Team.Player);
        }

        foreach (UnitSO enemy in encounter.GenerateEnemies(database))
        {
            AddParticipant(enemy, Team.Enemy);
        }
    }
}

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