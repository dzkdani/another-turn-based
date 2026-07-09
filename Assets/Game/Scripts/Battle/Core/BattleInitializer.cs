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

        SpawnCombatants();

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

    private void SpawnCombatants()
    {
        int playerIndex = 0;
        int enemyIndex = 0;

        foreach (var participant in BattleSetup.Instance.Participants)
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

            Transform parent =
                participant.Team == Team.Player
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

            battleUnit.Setup(
                participant.UnitSO,
                participant.Team);

            if (participant.Team == Team.Player)
                spawnedPlayers.Add(battleUnit);
            else
                spawnedEnemies.Add(battleUnit);
        }
    }
}