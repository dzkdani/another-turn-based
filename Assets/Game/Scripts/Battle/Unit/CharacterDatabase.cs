using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "ScriptableObjects/Database/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField]
    private List<UnitSO> characters = new();

    public IReadOnlyList<UnitSO> Characters => characters;

    public int Count => characters.Count;

    public UnitSO GetCharacter(int index)
    {
        if (index < 0 || index >= characters.Count)
            return null;

        return characters[index];
    }

    public UnitSO GetRandomCharacter(UnitSO exclude = null)
    {
        if (characters.Count == 0)
            return null;

        List<UnitSO> candidates = new();

        foreach (var character in characters)
        {
            if (character != null && character != exclude)
                candidates.Add(character);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }
}