using UnityEngine;
using System.Collections.Generic;

// Struktur data sederhana untuk menampung setup per karakter
[System.Serializable]
public struct SelectedCharacterConfig
{
    public UnitSO unitSO;       // Mengambil template stat & visual prefab
    public Team assignedTeam;   // Player atau Enemy (Dinamis ditentukan di UI)
}

public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager Instance { get; private set; }

    // List yang akan menampung tepat 4 karakter hasil pilihan di UI
    public List<SelectedCharacterConfig> SelectedUnits = new List<SelectedCharacterConfig>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Agar data tidak hilang saat pindah ke Battle Scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Fungsi pembantu untuk validasi sebelum masuk battle (Harus 2 Player & 2 Enemy)
    public bool IsSelectionValid()
    {
        if (SelectedUnits.Count != 4) return false;

        int playerCount = 0;
        int enemyCount = 0;

        foreach (var config in SelectedUnits)
        {
            if (config.assignedTeam == Team.Player) playerCount++;
            if (config.assignedTeam == Team.Enemy) enemyCount++;
        }

        return playerCount == 2 && enemyCount == 2;
    }
}