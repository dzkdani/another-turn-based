using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class CopySkillAction : BattleAction
{
    public override IEnumerator Execute(BattleUnit attacker, List<BattleUnit> targets, BattleActionSO actionData, BattlePresentationContext presentation)
    {
        if (targets == null || targets.Count == 0) yield break;
        BattleUnit target = targets[0];

        Debug.Log($"{attacker.Data.Name} menggunakan {actionData.ActionName} pada {target.Data.Name}!");
        yield return new WaitForSeconds(0.5f);

        // Ambil salah satu skill acak milik target yang valid (bukan tipe Run atau CopySkill lagi)
        BattleActionSO copiedSkillData = target.Data.Skills
            .FirstOrDefault(s => s.ActionType != BattleActionType.Run && s.ActionType != BattleActionType.CopySkill);

        if (copiedSkillData != null)
        {
            Debug.Log($"{attacker.Data.Name} berhasil meniru skill: {copiedSkillData.ActionName}!");
            yield return new WaitForSeconds(0.5f);

            // Eksekusi skill hasil copy menggunakan ActionSystem yang sama secara berantai
            ActionSystem internalSystem = new ActionSystem(presentation);
            
            // Tentukan target baru untuk skill hasil copy (misal: menyerang balik si target asal)
            List<BattleUnit> newTargets = new List<BattleUnit> { target };
            
            // Jalankan sekuens skill yang di-copy
            yield return internalSystem.ExecuteAction(attacker, newTargets, copiedSkillData);
        }
        else
        {
            Debug.Log($"{attacker.Data.Name} gagal meniru karena target tidak memiliki skill valid.");
            yield return new WaitForSeconds(0.8f);
        }
    }
}