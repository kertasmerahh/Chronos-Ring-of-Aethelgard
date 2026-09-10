using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Pengaturan Darah")]
    [Tooltip("Batas maksimal darah pemain")]
    public int maxHealth = 3;
    [Tooltip("Darah pemain saat ini")]
    public int currentHealth;

    [Header("Pengaturan I-Frame (Invincibility)")]
    [Tooltip("Durasi kebal setelah terkena serangan (detik)")]
    public float invincibilityDuration = 1.0f;
    [Tooltip("Kecepatan kedip visual saat kebal")]
    public float flashInterval = 0.1f;

    [Header("UI Game Over")]
    [Tooltip("Tarik Panel Game Over dari Canvas ke sini")]
    public GameObject panelGameOver;

    [Header("Nama Scene")]
    public string namaSceneMenu = "MainMenu";

    private bool isDead = false;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    public bool IsDead => isDead;
    public bool IsInvincible => isInvincible;

    void Awake()
    {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    void Update()
    {
        // Shortcut restart / menu saat Game Over jika panel belum ada
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            else if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Escape))
            {
                KeMainMenu();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // Abaikan jika sudah mati atau sedang dalam masa I-Frame
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log($"Player terkena serangan! Sisa HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
            float elapsed = 0f;

            while (elapsed < invincibilityDuration)
            {
                // Berkedip transparan
                spriteRenderer.color = transparentColor;
                yield return new WaitForSeconds(flashInterval);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(flashInterval);

                elapsed += flashInterval * 2f;
            }

            spriteRenderer.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(invincibilityDuration);
        }

        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;
        currentHealth = 0;
        Debug.Log("--- PLAYER MATI (GAME OVER) ---");

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void KeMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(namaSceneMenu);
    }

    void OnGUI()
    {
        // Tampilkan HUD HP sederhana & Layar Game Over jika panel canvas belum ada
        if (panelGameOver == null && isDead)
        {
            GUI.Box(new Rect(Screen.width / 2 - 160, Screen.height / 2 - 80, 320, 160), "GAME OVER");
            GUI.Label(new Rect(Screen.width / 2 - 140, Screen.height / 2 - 40, 280, 30), "Player kehabisan darah!");

            if (GUI.Button(new Rect(Screen.width / 2 - 120, Screen.height / 2, 110, 40), "Retry [R]"))
            {
                RestartGame();
            }

            if (GUI.Button(new Rect(Screen.width / 2 + 10, Screen.height / 2, 110, 40), "Menu [M]"))
            {
                KeMainMenu();
            }
        }
        else if (!isDead)
        {
            // Info darah sederhana di pojok kiri atas
            string hearts = "";
            for (int i = 0; i < maxHealth; i++)
            {
                hearts += (i < currentHealth) ? " ♥" : " ♡";
            }
            GUI.Label(new Rect(20, 20, 200, 30), $"DARAH:{hearts} ({currentHealth}/{maxHealth})");
        }
    }
}