using UnityEngine;
using System.Collections.Generic;

public class BattleUIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private UnitHUD playerHUDPrefab;
    [SerializeField] private UnitHUD enemyHUDPrefab;

    [SerializeField] private Transform playerHUDRoot;
    [SerializeField] private Transform enemyHUDRoot;

    // Menghapus 'readonly' agar list bisa di-clear atau di-reset dengan aman jika dibutuhkan
    private List<UnitHUD> playerHUDs = new();
    private List<UnitHUD> enemyHUDs = new();

    public void Initialize(List<BattleUnit> players, List<BattleUnit> enemies)
    {
        // PENTING: Bersihkan HUD lama sebelum membuat yang baru
        ClearHUDs();

        CreatePlayerHUDs(players);
        CreateEnemyHUDs(enemies);
    }

    private void CreatePlayerHUDs(List<BattleUnit> players)
    {
        foreach(var player in players)
        {
            if (playerHUDPrefab == null) continue;

            UnitHUD hud = Instantiate(playerHUDPrefab, playerHUDRoot);
            hud.Setup(player);
            playerHUDs.Add(hud);
        }
    }

    private void CreateEnemyHUDs(List<BattleUnit> enemies)
    {
        foreach(var enemy in enemies)
        {
            if (enemyHUDPrefab == null) continue;

            UnitHUD hud = Instantiate(enemyHUDPrefab, enemyHUDRoot);
            hud.Setup(enemy);
            enemyHUDs.Add(hud);
        }
    }

    /// <summary>
    /// Menghancurkan semua objek HUD yang ada di scene dan mengosongkan list penampung.
    /// </summary>
    public void ClearHUDs()
    {
        // Hancurkan Game Object Player HUD
        foreach (var hud in playerHUDs)
        {
            if (hud != null) Destroy(hud.gameObject);
        }
        playerHUDs.Clear();

        // Hancurkan Game Object Enemy HUD
        foreach (var hud in enemyHUDs)
        {
            if (hud != null) Destroy(hud.gameObject);
        }
        enemyHUDs.Clear();
    }

    // Pastikan pembersihan juga terjadi saat objek ini dihancurkan dari memory
    private void OnDestroy()
    {
        ClearHUDs();
    }
}