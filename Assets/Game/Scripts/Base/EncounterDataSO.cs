using System.Collections.Generic;
using UnityEngine;

public enum EncounterType
{
    Random,
    Fixed
}

[CreateAssetMenu(
    fileName = "New Encounter",
    menuName = "Battle/Encounter Data")]
public class EncounterDataSO : ScriptableObject
{
    [Header("General")]
    [SerializeField]
    private EncounterType encounterType = EncounterType.Random;

    [SerializeField]
    private int enemyCount = 2;

    [Header("Fixed Encounter")]
    [SerializeField]
    private List<UnitSO> fixedEnemies = new();

    public List<UnitSO> PlayerUnits = new();
    public List<UnitSO> EnemyUnits = new();

    public IReadOnlyList<UnitSO> GenerateEnemies(CharacterDatabase database)
    {
        List<UnitSO> enemies = new();

        switch (encounterType)
        {
            case EncounterType.Random:

                for (int i = 0; i < enemyCount; i++)
                {
                    UnitSO randomEnemy = GetRandomCharacter(database);

                    if (randomEnemy != null)
                        enemies.Add(randomEnemy);
                }

                break;

            case EncounterType.Fixed:

                enemies.AddRange(fixedEnemies);

                break;
        }

        return enemies;
    }

    public UnitSO GetRandomCharacter(CharacterDatabase database)
    {
        if (database == null)
        {
            Debug.LogError("EncounterData : Character Database is null.");
            return null;
        }

        if (database.Count == 0)
        {
            Debug.LogWarning("Character Database contains no characters.");
            return null;
        }

        int index = Random.Range(0, database.Count);

        return database.GetCharacter(index);
    }
}