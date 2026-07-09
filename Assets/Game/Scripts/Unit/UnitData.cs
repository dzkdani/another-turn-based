public class UnitData
{
    public string Name;

    public int MaxHP;
    public int CurrentHP;

    public int CurrentAtk;
    public int CurrentDef;
    public int CurrentSpd;

    public float CurrentCritChance;

    public float HPPercent =>
        MaxHP <= 0 ? 0 : (float)CurrentHP / MaxHP;
}