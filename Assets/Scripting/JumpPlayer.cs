using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Pengaturan Lompat")]
    public float jumpForce = 7f;

    [Header("Pengaturan Skill")]
    public float holdThreshold = 0.4f; // Durasi tahan Spasi (dalam detik) untuk memicu skill

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private float spaceHoldTimer = 0f;
    private bool isHoldingSpace = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleJump();
        HandleCombatInput();
    }

    private void HandleJump()
    {
        // Lompat menggunakan tombol W saat menyentuh tanah
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(0f, jumpForce);
        }
    }

    private void HandleCombatInput()
    {
        // Deteksi mulai menekan Spasi
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceHoldTimer = 0f;
            isHoldingSpace = true;
        }

        // Menghitung durasi tombol Spasi ditahan
        if (Input.GetKey(KeyCode.Space) && isHoldingSpace)
        {
            spaceHoldTimer += Time.deltaTime;
        }

        // Deteksi saat tombol Spasi dilepas
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (spaceHoldTimer >= holdThreshold)
            {
                TriggerSkill();
            }
            else
            {
                TriggerSlash();
            }

            // Reset status input
            isHoldingSpace = false;
            spaceHoldTimer = 0f;
        }
    }

    private void TriggerSlash()
    {
        // TODO: Taruh logika animasi / hit serang (slashing) di sini
        Debug.Log("Melakukan Slashing!");
    }

    private void TriggerSkill()
    {
        // TODO: Taruh logika animasi / efek skill di sini
        Debug.Log("Mengaktifkan Skill!");
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