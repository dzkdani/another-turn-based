using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class InterruptAction : BattleAction
{
    public override IEnumerator Execute(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData, BattlePresentationContext presentation)
    {
        yield return new WaitForSeconds(0.4f);

        foreach (var target in targets)
        {
            if (target == null || target.IsDead) continue;

            // 1. Berikan damage utama terlebih dahulu
            int damage = DamageSystem.CalculateDamage(attacker, target, actionData.DamageMultiplier);
            target.TakeDamage(damage);

            if (actionData.FX.HitVFXPrefab != null)
            {
                Object.Instantiate(actionData.FX.HitVFXPrefab, target.transform.position, Quaternion.identity);
            }

            // 2. KUNCI INTERRUPT: Manipulasi Turn Order HSR
            // Kita asumsikan Anda bisa mengakses TurnManager yang aktif via BattleManager (atau modifikasi sesuai arsitektur Anda)
            // Di TurnManager HSR yang kita refaktor kemarin, kita bisa menambahkan fungsi untuk memanipulasi registry AV.
            
            Debug.Log($"INTERRUPT! Giliran {target.Data.Name} ditunda!");
            
            // Contoh implementasi logika manipulasi jika TurnManager di-passing atau bisa diakses:
            // float currentAV = TurnManager.GetUnitAV(target);
            // TurnManager.SetUnitAV(target, currentAV + 50f); // Menambah AV = memperlama giliran jalan
        }

        yield return new WaitForSeconds(0.8f);
    }
}