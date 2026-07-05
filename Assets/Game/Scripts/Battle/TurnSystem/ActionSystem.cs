using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ActionSystem
{
    private readonly Dictionary<BattleActionType, Type> actionClassCache = new();

    public List<BattleUnit> ResolveTargets(BattleUnit caster, BattleActionSO action, List<BattleUnit> players, List<BattleUnit> enemies, BattleUnit preferredTarget, TargetSystem targetSystem)
    {
        return targetSystem.GetTargets(caster, action.TargetType, players, enemies, preferredTarget);
    }

    public IEnumerator ExecuteAction(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData)
    {
        if (actionData == null) yield break;

        // Mencari Class Pelaksana berdasarkan kategori (ActionType)
        // Misal: BattleActionType.Attack -> otomatis mencari class "AttackAction"
        // Misal: BattleActionType.Heal   -> otomatis mencari class "HealAction"
        BattleAction actionExecutor = GetActionExecutor(actionData.ActionType);

        if (actionExecutor == null)
        {
            Debug.LogError($"Class pelaksana untuk kategori {actionData.ActionType}Action belum dibuat!");
            yield break;
        }

        // Jalankan eksekusi dengan melempar data spesifik dari ScriptableObject-nya!
        yield return actionExecutor.Execute(attacker, targets, actionData);
    }

    private BattleAction GetActionExecutor(BattleActionType type)
    {
        if (actionClassCache.TryGetValue(type, out Type cachedType))
        {
            return (BattleAction)Activator.CreateInstance(cachedType);
        }

        string className = $"{type}Action";
        Type actionType = Type.GetType(className);

        if (actionType != null)
        {
            actionClassCache[type] = actionType;
            return (BattleAction)Activator.CreateInstance(actionType);
        }

        return null;
    }
}