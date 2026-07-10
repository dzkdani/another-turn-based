using UnityEngine;
using System.Collections.Generic;

public class BattleInitializer : MonoBehaviour
{
    [SerializeField] private EncounterDataSO testEncounter;

    [SerializeField] private BattleManager battleManager;

    [Header("Base Battle Unit Prefab")]
    [SerializeField] private GameObject baseBattleUnitPrefab;

    [Header("Spawn Root")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform enemyRoot;

    [Header("Spawn Anchors")]
    [SerializeField] private Transform[] playerSpawnAnchors;
    [SerializeField] private Transform[] enemySpawnAnchors;

    private readonly List<BattleUnit> spawnedPlayers = new();
    private readonly List<BattleUnit> spawnedEnemies = new();
    private List<BattleParticipant> participants;

    private void Start()
    {
        InitializeBattle();
    }

    public void InitializeBattle()
    {
        if (!ValidateBattleSetup())
            return;

        spawnedPlayers.Clear();
        spawnedEnemies.Clear();

        if (testEncounter != null)
        {
            participants = GetParticipants();
        }
        else
        {
            participants = new List<BattleParticipant>(BattleSetup.Instance.Participants);
        }

        SpawnCombatants(participants);

        battleManager.InitializeBattle(
            spawnedPlayers,
            spawnedEnemies);
    }

    private bool ValidateBattleSetup()
    {
        if (BattleSetup.Instance != null &&
            BattleSetup.Instance.HasValidSelection()) 
                return true;

        if(testEncounter != null) return true;

        if(battleManager != null) return true;

        Debug.LogError("Battle components missing");
        return false;
    }

    private void SpawnCombatants(List<BattleParticipant> participants)
    {
        int playerIndex = 0;
        int enemyIndex = 0;

        foreach (var participant in participants)
        {
            Transform anchor = null;

            if (participant.Team == Team.Player)
            {
                if (playerIndex >= playerSpawnAnchors.Length)
                {
                    Debug.LogWarning("Not enough Player Spawn Anchors.");
                    continue;
                }

                anchor = playerSpawnAnchors[playerIndex++];
            }
            else
            {
                if (enemyIndex >= enemySpawnAnchors.Length)
                {
                    Debug.LogWarning("Not enough Enemy Spawn Anchors.");
                    continue;
                }

                anchor = enemySpawnAnchors[enemyIndex++];
            }

            Transform parent = participant.Team == Team.Player
                ? playerRoot
                : enemyRoot;

            GameObject obj = Instantiate(
                baseBattleUnitPrefab,
                anchor.position,
                anchor.rotation,
                parent);

            BattleUnit battleUnit = obj.GetComponent<BattleUnit>();

            if (battleUnit == null)
            {
                Debug.LogError("Base prefab has no BattleUnit component.");
                Destroy(obj);
                continue;
            }

            Transform targetRoot = battleUnit.Team == Team.Player
                ? enemyRoot
                : playerRoot;

            battleUnit.Visual.FaceTarget(targetRoot);

            battleUnit.Setup(participant.UnitSO, participant.Team);

            if (participant.Team == Team.Player)
                spawnedPlayers.Add(battleUnit);
            else
                spawnedEnemies.Add(battleUnit);
        }
    }

    private List<BattleParticipant> GetParticipants()
    {
        if (BattleSetup.Instance != null)
        {
            return (List<BattleParticipant>)BattleSetup.Instance.Participants;
        }

        if (testEncounter == null)
        {
            Debug.LogError("No BattleSetup found and no Test Encounter assigned.");
            return new List<BattleParticipant>();
        }

        List<BattleParticipant> participants = new();

        foreach (UnitSO player in testEncounter.PlayerUnits)
        {
            if (player != null)
                participants.Add(new BattleParticipant(player, Team.Player));
        }

        foreach (UnitSO enemy in testEncounter.EnemyUnits)
        {
            if (enemy != null)
                participants.Add(new BattleParticipant(enemy, Team.Enemy));
        }

        return participants;
    }
}