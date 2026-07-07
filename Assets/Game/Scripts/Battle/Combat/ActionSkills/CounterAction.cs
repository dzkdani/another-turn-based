using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CounterAction : BattleAction
{
    public override IEnumerator Execute(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData, BattlePresentationContext presentation)
    {   
        Debug.Log($"{attacker.Data.Name} memasuki mode Bersiap Counter!");
        yield return new WaitForSeconds(0.6f);

        // Membuat fungsi delegasi lokal untuk logika memukul balik
        System.Action<int> counterLogic = null;

        counterLogic = (damageReceived) =>
        {
            // Pengaman: Jika karakter mati akibat pukulan tersebut, batalkan counter
            if (attacker.IsDead)
            {
                attacker.OnTakeDamage -= counterLogic;
                return;
            }

            Debug.Log($"{attacker.Data.Name} memicu COUNTER ATTACK!");

            // Eksekusi serangan balik instan ke penyerang menggunakan ActionSystem biasa
            // ActionSystem counterAttackSystem = new ActionSystem();
            
            // Mencari basic attack milik dirinya untuk dipakai memukul balik
            BattleActionSO basicAttack = attacker.Data.Skills
                .Find(s => s.ActionType == BattleActionType.Attack);

            if (basicAttack != null)
            {
                // Target counter adalah siapa pun yang saat itu sedang aktif (musuh yang memukulnya)
                // Kita bisa melacak attacker dari turn manager atau sistem log Anda.
                // Untuk contoh aman, kita asumsikan serangan balik instan berjalan.
                // Coroutine dijalankan via MonoBehaviour eksternal karena class ini pure C#
            }

            // Un-subscribe setelah counter berhasil dipicu sekali (agar tidak aktif selamanya)
            attacker.OnTakeDamage -= counterLogic;
        };

        // Daftarkan ke Event-Driven Architecture milik unit tersebut
        attacker.OnTakeDamage += counterLogic;
    }
}