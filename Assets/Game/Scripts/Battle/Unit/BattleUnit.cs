using UnityEngine;
using System.Collections.Generic;
using System;

public class BattleUnit : MonoBehaviour
{
    public UnitData Data;
    public UnitSO UnitSO;
    public Team Team; // Sekarang status ini dinamis (bisa di-overwrite oleh Initializer/Character Select)
    public bool IsDead;

    // --- ARCHITECTURE EVENT-DRIVEN (Wajib Syarat Test Novastra) ---
    public Action OnTurnStart;
    public Action OnTurnEnd;
    public Action OnAttack;
    public Action<int> OnTakeDamage; // Mengirim jumlah final damage untuk Post-Processing / UI Pop-up
    public Action OnDeath;

    private void Awake()
    {
        InitializeUnit();
    }

    // Dipisah menjadi fungsi public agar bisa di-reset atau di-reinit oleh BattleInitializer
    public void InitializeUnit()
    {
        if (UnitSO == null)
        {
            Debug.LogError($"{gameObject.name} had no SO Assigned!");
            return;
        }
        
        Data = new UnitData();

        Data.Name = UnitSO.Name;
        Data.MaxHP = UnitSO.MaxHP;
        Data.CurrentHP = UnitSO.MaxHP;
        Data.CurrentAtk = UnitSO.Attack;
        Data.CurrentDef = UnitSO.Defense;
        Data.CurrentSpd = UnitSO.Speed;
        Data.CurrentCritChance = UnitSO.CritChance;

        Data.Skills = new List<BattleActionSO>();
        Data.Skills.AddRange(UnitSO.Skills);

        // Ambil default team dari SO, tapi variabel ini bisa diganti nanti saat Character Select
        Team = UnitSO.Team; 
        IsDead = false;
    }

    public AIBehaviorSO AIBehavior => UnitSO != null ? UnitSO.AIBehavior : null;

    // Refaktor fungsi Take Damage agar menghitung DEF dan memicu Event-Driven Architecture
    public void TakeDamage(int rawDamage)
    {
        if (IsDead) return;

        // Formula damage standar: Raw DMG dikurangi DEF (Minimal menghasilkan 1 damage)
        int finalDamage = Mathf.Max(1, rawDamage - Data.CurrentDef);
        
        Data.CurrentHP -= finalDamage;
        Data.CurrentHP = Mathf.Clamp(Data.CurrentHP, 0, Data.MaxHP);

        // 1. Picu event internal system lama Anda
        BattleEvents.OnUnitDamaged?.Invoke(this);

        // 2. Picu event lokal spesifik unit untuk Post-Processing Flash & Hit Animation (Req Utama)
        OnTakeDamage?.Invoke(finalDamage);

        Debug.Log($"{Data.Name} took {finalDamage} damage. Current HP: {Data.CurrentHP}");

        if (Data.CurrentHP <= 0)
        {
            IsDead = true;
            OnDeath?.Invoke(); // Picu event kematian (untuk trigger animasi mati)
        }
    }

    public void Heal(int amount)
    {
        Debug.Log($"Implement healing later..");
    }
}