using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AttackAction : BattleAction
{
    public override IEnumerator Execute(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData, BattlePresentationContext presentation)
    {
        presentation.Audio.Play(actionData.FX.CastSFX,attacker.Visual.CastPoint);

        attacker.AnimationBridge.PlayAttackAnimation();
        yield return new WaitForSeconds(0.4f); // Menunggu sekuens animasi memukul

        foreach (var target in targets)
        {
            if (target == null || target.IsDead) continue;

            // Membaca variabel 'Multiplier' yang ada di SO spesifik tersebut
            int damage = DamageSystem.CalculateDamage(attacker, target, actionData.DamageMultiplier);
            target.TakeDamage(damage);

            target.AnimationBridge.PlayHitAnimation();

            presentation.Audio.Play(
                actionData.FX.HitSFX,
                target.Visual.HitPoint);
        }
        yield return new WaitForSeconds(0.8f);
    }
}