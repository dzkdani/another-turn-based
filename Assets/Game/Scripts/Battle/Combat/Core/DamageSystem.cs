using UnityEngine;

public static class DamageSystem
{
    public static float CalculateDamage(
        BattleUnit attacker,
        BattleUnit defender,
        float multiplier)
    {
        float attack =
            attacker.Data.CurrentAtk;

        float defense =
            defender.Data.CurrentDef;

        float damage =
            (attack - defense) * multiplier;

        bool crit = Random.value < attacker.Data.CurrentCritChance;

        if(crit)
            damage *= 2;

        return damage;
    }
}

public struct DamageResult
{
    public int Damage;
    public bool IsCrit;
    public bool IsMiss;
}