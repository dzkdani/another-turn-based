using UnityEngine;
using System.Collections.Generic;

public class BattleInitializer : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;

    [Header("Base Battle Unit Prefab")]
    [SerializeField] private GameObject baseBattleUnitPrefab;

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
        if (BattleSetup.Instance == null)
        {
            Debug.LogError("BattleSetup not found.");
            return false;
        }

        if (!BattleSetup.Instance.HasValidSelection())
        {
            Debug.LogError("BattleSetup contains invalid battle data.");
            return false;
        }

        return true;
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

            GameObject obj = Instantiate(
                baseBattleUnitPrefab,
                anchor.position,
                anchor.rotation);

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