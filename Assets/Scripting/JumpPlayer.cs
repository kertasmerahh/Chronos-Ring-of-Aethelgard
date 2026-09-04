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
        // 1. Saat Spasi baru ditekan
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isHolding = true;
            holdTimer = 0f;
            skillTriggered = false;
        }

        // 2. Selama Spasi ditahan
        if (Input.GetKey(KeyCode.Space) && isHolding)
        {
            holdTimer += Time.deltaTime;

            // Jika ditahan lebih dari batas waktu, picu skill
            if (holdTimer >= holdDurationNeeded && !skillTriggered)
            {
                TriggerGlobalSkill();
                skillTriggered = true;
            }
        }

        // 3. Saat Spasi dilepas
        if (Input.GetKeyUp(KeyCode.Space))
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