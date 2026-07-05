using UnityEngine;
using System.Collections.Generic;

public class BattleInitializer : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;

    [Header("Base Battle Object Prefab")]
    // Sediakan 1 prefab kosong yang memiliki komponen `BattleUnit` di dalamnya
    [SerializeField] private GameObject baseBattleUnitPrefab; 

    [Header("Spawn Anchors (Sediakan minimal 2 slot per tim)")]
    [SerializeField] private Transform[] playerSpawnAnchors;
    [SerializeField] private Transform[] enemySpawnAnchors;

    private readonly List<BattleUnit> spawnedPlayers = new();
    private readonly List<BattleUnit> spawnedEnemies = new();

    private void Start()
    {
        // Jalankan validasi pengaman fallback jika player bypass UI select langsung ke scene ini
        ValidateAndFallback();

        SpawnCombatants();

        // Teruskan data dinamis ke komponen manager Anda yang lain
        battleManager.InitializeBattle(spawnedPlayers, spawnedEnemies);
    }

    private void ValidateAndFallback()
    {
        if (CharacterSelectionManager.Instance == null || !CharacterSelectionManager.Instance.IsSelectionValid())
        {
            Debug.LogWarning("CharacterSelectionManager data invalid or missing! Menggunakan data fallback.");
            // Di sini Anda bisa menulis kode manual memasukkan data ke CharacterSelectionManager 
            // agar saat Anda menekan 'Play' langsung di Battle Scene untuk testing, game tidak error.
        }
    }

    private void SpawnCombatants()
    {
        var selectedUnits = CharacterSelectionManager.Instance.SelectedUnits;

        int playerIndex = 0;
        int enemyIndex = 0;

        foreach (var config in selectedUnits)
        {
            Transform currentAnchor = null;

            // Tentukan posisi spawn berdasarkan Faksi Dinamis hasil pilihan user
            if (config.assignedTeam == Team.Player)
            {
                if (playerIndex >= playerSpawnAnchors.Length) continue;
                currentAnchor = playerSpawnAnchors[playerIndex];
                playerIndex++;
            }
            else
            {
                if (enemyIndex >= enemySpawnAnchors.Length) continue;
                currentAnchor = enemySpawnAnchors[enemyIndex];
                enemyIndex++;
            }

            // 1. Spawn wadah utama (objek ber-komponen BattleUnit)
            GameObject unitObj = Instantiate(baseBattleUnitPrefab, currentAnchor.position, currentAnchor.rotation);
            BattleUnit battleUnit = unitObj.GetComponent<BattleUnit>();

            // 2. Suntikkan Data-Driven ScriptableObject yang dipilih ke dalam BattleUnit runtime tersebut
            battleUnit.UnitSO = config.unitSO;
            battleUnit.InitializeUnit(); // Panggil fungsi setup internal data (HP, ATK, SPD, dll)
            
            // 3. OVERWRITE faksi tim aslinya agar mengikuti pilihan dinamis user! (Kunci Utama Test)
            battleUnit.Team = config.assignedTeam;

            // 4. Spawn Visual Grafis (Karakter dummy Mixamo Anda) di bawah objek wadah ini
            if (config.unitSO.VisualPrefab != null)
            {
                GameObject visualObj = Instantiate(config.unitSO.VisualPrefab, unitObj.transform);
                visualObj.transform.localPosition = Vector3.zero;
                visualObj.transform.localRotation = Quaternion.identity;
            }

            // 5. Masukkan ke dalam list faksi pertempuran
            if (battleUnit.Team == Team.Player)
            {
                spawnedPlayers.Add(battleUnit);
            }
            else
            {
                spawnedEnemies.Add(battleUnit);
            }
        }
    }
}