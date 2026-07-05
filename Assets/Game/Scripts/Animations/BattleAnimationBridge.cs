using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(BattleUnit))]
public class BattleAnimationBridge : MonoBehaviour
{
    private BattleUnit battleUnit;
    private Animator animator;

    [Header("Animation Parameter Names")]
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int TakeDamageHash = Animator.StringToHash("TakeDamage");
    private static readonly int DieHash = Animator.StringToHash("IsDead");
    private static readonly int ActiveTurnHash = Animator.StringToHash("OnTurnStart");

    [SerializeField] private bool useTimeline = false;

    private void Awake()
    {
        battleUnit = GetComponent<BattleUnit>();
        animator = GetComponentInChildren<Animator>(); // Mengambil animator di child (model Mixamo)

        if (animator == null)
        {
            Debug.LogWarning($"Animator tidak ditemukan di {gameObject.name} atau child-nya!");
        }
    }

    private void OnEnable()
    {
        // --- BERLANGGANAN (SUBSCRIBE) KE EVENT ARCHITECTURE ---
        battleUnit.OnTurnStart += HandleTurnStart;
        battleUnit.OnTurnEnd += HandleTurnEnd;
        battleUnit.OnAttack += HandleAttack;
        battleUnit.OnTakeDamage += HandleTakeDamage;
        battleUnit.OnDeath += HandleDeath;
        
        // Cadangan pengaman jika unit di-reset dinamis saat character select
        battleUnit.OnTurnStart += UpdateTurnVisualStatus; 
    }

    private void OnDisable()
    {
        // --- UN-SUBSCRIBE SAAT DI-DISABLE (Mencegah Memory Leak) ---
        if (battleUnit != null)
        {
            battleUnit.OnTurnStart -= HandleTurnStart;
            battleUnit.OnTurnEnd -= HandleTurnEnd;
            battleUnit.OnAttack -= HandleAttack;
            battleUnit.OnTakeDamage -= HandleTakeDamage;
            battleUnit.OnDeath -= HandleDeath;
            battleUnit.OnTurnStart -= UpdateTurnVisualStatus;
        }
    }

    [Button("Turn Start")]
    private void HandleTurnStart()
    {
        SetCurrentTurn(true);
    }

    [Button("Turn End")]
    private void HandleTurnEnd()
    {
        SetCurrentTurn(false);
    }

    [Button("Attack")]
    private void HandleAttack()
    {
        PlayAttack();
    }

    private void HandleTakeDamage(int damageAmount)
    {
        PlayHit();
    }

    [Button("Die")]
    private void HandleDeath()
    {
        PlayDeath();
    }

    public void PlayAttack()
    {
        if(useTimeline)
        {
            // timelinePlayer.Play(attackTimeline);
            return;
        }

        animator.SetTrigger(AttackHash);
    }

    public void PlayHit()
    {
        if (animator == null) return;

        // Memicu trigger animasi terkena pukulan (Flinch/Hit)
        animator.SetTrigger(TakeDamageHash);
        
        // SANGAT COCOK DI SINI: Tempat memicu Post-Processing Camera Shake / Flash Screen!
        // CameraShake.Instance.Shake();
    }

    public void PlayDeath()
    {
        if (animator == null) return;

        // Mengubah status animasi mati menjadi true agar memutar animasi terkapar
        animator.SetBool(DieHash, true);
    }

    public void SetCurrentTurn(bool active)
    {
        if (animator == null) return;
        
        // Contoh: Membuat karakter mengambil pose siaga khusus saat gilirannya tiba
        animator.SetBool(ActiveTurnHash, active);
        
        // Reset karakter lain saat giliran berakhir diatur saat AdvanceTurn (bisa via Event global)
    }

    private void UpdateTurnVisualStatus()
    {
        // Logika tambahan untuk merapikan visual jika giliran berpindah
        // Mengubah parameter IsMyTurn menjadi false untuk semua unit lain yang tidak aktif
    }
}