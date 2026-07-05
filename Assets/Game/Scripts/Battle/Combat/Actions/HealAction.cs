using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class HealAction : BattleAction
{
    public override IEnumerator Execute(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData)
    {
        if (actionData.SFX != null) AudioSource.PlayClipAtPoint(actionData.SFX, attacker.transform.position);
        yield return new WaitForSeconds(0.3f);

        foreach (var target in targets)
        {
            if (target == null || target.IsDead) continue;

            // Membaca variabel 'BaseHealValue' dari SO spesifik tersebut
            target.Heal(actionData.HealAmount);

            if (actionData.HitVFXPrefab != null)
            {
                Object.Instantiate(actionData.HitVFXPrefab, target.transform.position, Quaternion.identity);
            }
        }
        yield return new WaitForSeconds(0.8f);
    }
}