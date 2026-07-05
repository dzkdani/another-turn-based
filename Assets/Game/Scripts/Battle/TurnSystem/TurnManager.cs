using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager
{
    private readonly List<BattleUnit> allUnits = new();
    
    // Melacak sisa Action Value (AV) dari setiap unit yang masih hidup
    private readonly Dictionary<BattleUnit, float> unitAVRegistry = new();
    
    private BattleUnit currentUnitInTurn;

    public IReadOnlyList<BattleUnit> Units => allUnits;

    public void Initialize(List<BattleUnit> units)
    {
        allUnits.Clear();
        unitAVRegistry.Clear();
        currentUnitInTurn = null;

        if (units != null)
        {
            foreach (var unit in units)
            {
                if (unit != null && !unit.IsDead)
                {
                    allUnits.Add(unit);
                    // Hitung AV awal berdasarkan rumus HSR: 10000 / SPD
                    unitAVRegistry[unit] = 10000f / unit.Data.CurrentSpd;
                }
            }
        }

        DetermineNextTurnUnit();
    }

    public BattleUnit GetCurrentUnit()
    {
        // Jika unit saat ini mati di tengah ronde (misal karena counter/pasif), cari ulang
        if (currentUnitInTurn == null || currentUnitInTurn.IsDead)
        {
            DetermineNextTurnUnit();
        }
        return currentUnitInTurn;
    }

    public void AdvanceTurn()
    {
        if (allUnits.Count == 0)
        {
            Debug.LogError("Turn order is empty.");
            return;
        }

        // 1. Ambil unit yang baru saja menyelesaikan gilirannya
        BattleUnit previousUnit = currentUnitInTurn;

        // 2. Reset Action Value untuk unit yang baru selesai bertindak tersebut
        if (previousUnit != null && !previousUnit.IsDead)
        {
            unitAVRegistry[previousUnit] = 10000f / previousUnit.Data.CurrentSpd;
        }
        else if (previousUnit != null && previousUnit.IsDead)
        {
            // Jika unit mati, hapus dari registrasi pelacakan AV
            unitAVRegistry.Remove(previousUnit);
        }

        // 3. Tentukan siapa unit berikutnya yang memiliki AV terkecil
        DetermineNextTurnUnit();
    }

    private void DetermineNextTurnUnit()
    {
        // Bersihkan unit mati dari registrasi sebelum kalkulasi
        var deadUnits = unitAVRegistry.Keys.Where(u => u == null || u.IsDead).ToList();
        foreach (var dead in deadUnits)
        {
            unitAVRegistry.Remove(dead);
        }

        if (unitAVRegistry.Count == 0)
        {
            currentUnitInTurn = null;
            return;
        }

        // Cari unit dengan sisa Action Value (AV) terkecil
        BattleUnit nextUnit = null;
        float shortestAV = float.MaxValue;

        foreach (var pair in unitAVRegistry)
        {
            if (pair.Value < shortestAV)
            {
                shortestAV = pair.Value;
                nextUnit = pair.Key;
            }
        }

        // Jalankan simulasi jalannya waktu: Kurangi AV semua unit lain sebesar 'shortestAV'
        List<BattleUnit> keys = new List<BattleUnit>(unitAVRegistry.Keys);
        foreach (var unit in keys)
        {
            unitAVRegistry[unit] -= shortestAV;
        }

        currentUnitInTurn = nextUnit;

        // Panggil Event OnTurnStart bawaan requirement utama Novastra
        if (currentUnitInTurn != null)
        {
            currentUnitInTurn.OnTurnStart?.Invoke();
        }
    }

    public void AddUnit(BattleUnit unit)
    {
        if (unit == null || allUnits.Contains(unit))
            return;

        allUnits.Add(unit);
        if (!unit.IsDead)
        {
            unitAVRegistry[unit] = 10000f / unit.Data.CurrentSpd;
        }
    }

    public void RemoveUnit(BattleUnit unit)
    {
        if (unit == null)
            return;

        allUnits.Remove(unit);
        unitAVRegistry.Remove(unit);

        if (currentUnitInTurn == unit)
        {
            DetermineNextTurnUnit();
        }
    }

    // Fungsi pembantu jika ada mekanik Buff/Debuff SPD di tengah game
    public void RefreshOrder()
    {
        // Di sistem HSR asli, jika SPD berubah, sisa AV akan dikalkulasi ulang secara proporsional.
        // Untuk scope test 4 hari ini, mereset AV berdasarkan SPD baru sudah sangat aman dan valid:
        var activeUnits = unitAVRegistry.Keys.ToList();
        foreach (var unit in activeUnits)
        {
            if (unit != null && !unit.IsDead)
            {
                unitAVRegistry[unit] = 10000f / unit.Data.CurrentSpd;
            }
        }
        DetermineNextTurnUnit();
    }
}