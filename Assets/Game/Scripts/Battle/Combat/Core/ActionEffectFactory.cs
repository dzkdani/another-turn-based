using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectFactory
{
    private static readonly Dictionary<BattleActionType, Type> effectTypeMap = new()
    {
        { BattleActionType.Attack, typeof(AttackEffect) },
        { BattleActionType.Heal, typeof(HealEffect) },
        { BattleActionType.Interrupt, typeof(InterruptEffect) },
        { BattleActionType.Counter, typeof(CounterEffect) },
        { BattleActionType.CopySkill, typeof(CopySkillEffect) }
    };

    public static IActionEffect CreateEffect(BattleActionType actionType)
    {
        if (effectTypeMap.TryGetValue(actionType, out Type effectType))
        {
            return (IActionEffect)Activator.CreateInstance(effectType);
        }

        Debug.LogWarning($"No effect found for action type: {actionType}");
        return null;
    }

    public static void RegisterEffect(BattleActionType actionType, Type effectType)
    {
        if (!typeof(IActionEffect).IsAssignableFrom(effectType))
        {
            Debug.LogError($"Type {effectType.Name} does not implement IActionEffect.");
            return;
        }

        effectTypeMap[actionType] = effectType;
    }
}
