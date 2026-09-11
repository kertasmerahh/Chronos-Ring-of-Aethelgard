using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Pengaturan Lompat")]
    public float jumpForce = 12f;

    [Header("Pengaturan Serang Tebasan")]
    public Collider2D attackCollider;      // Tarik Box Collider milik AttackHitbox ke sini
    public float attackDuration = 0.25f;   // Durasi pedang aktif (detik)

    [Header("Pengaturan Skill (Hold)")]
    public float holdDurationNeeded = 0.5f;
    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool skillTriggered = false;

    private Rigidbody2D rb;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Pastikan collider tebasan mati di awal
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    void Update()
    {
        // Jangan proses input jika game sedang pause atau player mati
        if (Time.timeScale == 0f) return;
        if (PlayerHealth.Instance != null && PlayerHealth.Instance.IsDead) return;

        HandleJump();
        HandleCombatInput();
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(0f, jumpForce);
        }
    }

    private void HandleCombatInput()
    {
        bool isAttackDown = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.J);
        bool isAttackHeld = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.J);
        bool isAttackUp = Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.J);

        // 1. Saat Spasi atau J baru ditekan
        if (isAttackDown)
        {
            isHolding = true;
            holdTimer = 0f;
            skillTriggered = false;
        }

        // 2. Selama Spasi atau J ditahan
        if (isAttackHeld && isHolding)
        {
            holdTimer += Time.deltaTime;

            // Jika ditahan lebih dari batas waktu, picu skill
            if (holdTimer >= holdDurationNeeded && !skillTriggered)
            {
                TriggerGlobalSkill();
                skillTriggered = true;
            }
        }

        // 3. Saat Spasi atau J dilepas
        if (isAttackUp)
        {
            // Jika dilepas sebelum durasi hold tercapai -> TEBASAN BIASA (Tap)
            if (!skillTriggered)
            {
                TriggerSlash();
            }

            isHolding = false;
            holdTimer = 0f;
            skillTriggered = false;
        }
    }

    private void TriggerSlash()
    {
        Debug.Log("--- TEBASAN DILEPASKAN! ---");
        StopCoroutine("SlashCoroutine");
        StartCoroutine("SlashCoroutine");
    }

    private IEnumerator SlashCoroutine()
    {
        if (attackCollider != null)
        {
            // Nyalakan collider tebasan
            attackCollider.enabled = true;

            // Tunggu sesaat (durasi ayunan pedang)
            yield return new WaitForSeconds(attackDuration);

            // Matikan kembali
            attackCollider.enabled = false;
        }
    }

    private void TriggerGlobalSkill()
    {
        Debug.Log("--- SKILL AREA AKTIF! ---");

        AnomalyHealth[] allEnemies = Object.FindObjectsByType<AnomalyHealth>(FindObjectsSortMode.None);
        foreach (AnomalyHealth enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
        }

        if (BossGluttony.Instance != null && BossGluttony.Instance.isBossActive)
        {
            BossGluttony.Instance.TakeHit();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}