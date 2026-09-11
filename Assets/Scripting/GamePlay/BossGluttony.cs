using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossGluttony : MonoBehaviour
{
    public static BossGluttony Instance { get; private set; }

    [Header("Pengaturan Boss")]
    [Tooltip("Jumlah hit yang dibutuhkan untuk mengalahkan boss")]
    public int maxHits = 20;
    [Tooltip("Jumlah hit saat ini")]
    public int currentHits = 0;

    [Header("Status")]
    public bool isBossActive = false;
    public bool isDefeated = false;

    [Header("Pengaturan Visual & Feedback")]
    public Color normalColor = new Color(0.9f, 0.2f, 0.2f, 1f); // Merah khas monster
    public Color hitFlashColor = Color.white;
    public float flashDuration = 0.08f;
    public Vector3 baseScale = new Vector3(1.8f, 1.8f, 1f);

    private SpriteRenderer spriteRenderer;
    private Collider2D bossCollider;
    private float lastHitTime = -1f;
    private const float MIN_HIT_INTERVAL = 0.08f; // Mencegah double hit di frame yang sama

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        bossCollider = GetComponent<Collider2D>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
        transform.localScale = baseScale;
    }

    void Start()
    {
        // Matikan visual dan collider di awal permainan sebelum Gate dibuka
        if (!isBossActive)
        {
            SetBossVisible(false);
        }
    }

    void Update()
    {
        if (isDefeated)
        {
            // Shortcut kemenangan
            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            }
            return;
        }

        if (!isBossActive) return;

        // Mendeteksi serangan tap tombol Spasi atau J saat fase boss
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.J))
        {
            TakeHit();
        }
    }

    public void ActivateBoss()
    {
        isBossActive = true;
        isDefeated = false;
        currentHits = 0;

        // Letakkan boss di hadapan Player (Player ada di sekitar x: -4.5)
        // Posisi boss di sekitar x: -1.8f sampai -1.5f agar pas di jarak serang
        transform.position = new Vector3(-1.8f, -0.4f, 0f);
        transform.localScale = baseScale;

        SetBossVisible(true);

        Debug.Log("=== BOSS GLUTTONY MUNCUL! Tekan Spasi/J untuk menyerang! ===");
    }

    public void TakeHit()
    {
        if (!isBossActive || isDefeated) return;

        // Debounce agar tidak multi-register dalam hitungan milidetik
        if (Time.unscaledTime - lastHitTime < MIN_HIT_INTERVAL) return;
        lastHitTime = Time.unscaledTime;

        currentHits++;
        Debug.Log($"Boss Gluttony terkena serangan! [{currentHits}/{maxHits}]");

        // Efek visual feedback
        StopCoroutine("HitFeedbackCoroutine");
        StartCoroutine("HitFeedbackCoroutine");

        // Cek apakah sudah 20 hit
        if (currentHits >= maxHits)
        {
            DefeatBoss();
        }
    }

    private IEnumerator HitFeedbackCoroutine()
    {
        // 1. Scale punch (sedikit membesar)
        transform.localScale = baseScale * 1.15f;

        // 2. Flash warna putih
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hitFlashColor;
        }

        yield return new WaitForSeconds(flashDuration);

        // Kembalikan ke normal
        transform.localScale = baseScale;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }

    private void DefeatBoss()
    {
        isDefeated = true;
        isBossActive = false;
        Debug.Log("=== BOSS GLUTTONY KALAH! VICTORY! ===");

        StartCoroutine(DefeatAnimationCoroutine());
    }

    private IEnumerator DefeatAnimationCoroutine()
    {
        float elapsed = 0f;
        float duration = 0.6f;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            if (spriteRenderer != null)
            {
                Color c = normalColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = c;
            }
            yield return null;
        }

        SetBossVisible(false);
    }

    private void SetBossVisible(bool visible)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = visible;
        if (bossCollider != null) bossCollider.enabled = visible;
    }

    // Trigger tabrakan jika terkena tebasan AttackHitbox
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isBossActive || isDefeated) return;

        if (other.CompareTag("AttackHitbox") || other.name.Contains("Attack") || other.GetComponent<PlayerAttackTrigger>() != null)
        {
            TakeHit();
        }
    }

    /// <summary>
    /// Memastikan objek BossGluttony tersedia di scene secara otomatis jika belum dibuat di Editor.
    /// </summary>
    public static BossGluttony EnsureBossExists()
    {
        if (Instance != null) return Instance;

        BossGluttony existing = Object.FindAnyObjectByType<BossGluttony>();
        if (existing != null)
        {
            Instance = existing;
            return Instance;
        }

        // Buat GameObject BossGluttony otomatis di scene
        GameObject bossObj = new GameObject("BossGluttony");
        bossObj.transform.position = new Vector3(-1.8f, -0.4f, 0f);

        SpriteRenderer sr = bossObj.AddComponent<SpriteRenderer>();
        // Buat sprite kotak default 1x1 jika belum ada asset
        Texture2D texture = Texture2D.whiteTexture;
        sr.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        sr.color = new Color(0.9f, 0.2f, 0.2f, 1f);
        sr.sortingOrder = 5;

        BoxCollider2D col = bossObj.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.5f, 1.5f);

        BossGluttony bossComp = bossObj.AddComponent<BossGluttony>();
        Instance = bossComp;

        return bossComp;
    }

    void OnGUI()
    {
        // 1. HUD Boss saat bertarung
        if (isBossActive && !isDefeated)
        {
            // Background bar atas
            int boxWidth = 360;
            int boxHeight = 70;
            int x = (Screen.width - boxWidth) / 2;
            int y = 20;

            GUI.Box(new Rect(x, y, boxWidth, boxHeight), "⚔️ GATEKEEPER: BOSS GLUTTONY ⚔️");

            // Progress bar
            float progress = (float)currentHits / maxHits;
            GUI.HorizontalScrollbar(new Rect(x + 20, y + 26, boxWidth - 40, 16), 0, progress, 0, 1);

            GUI.Label(new Rect(x + 30, y + 44, boxWidth - 60, 20), $"Hits: {currentHits} / {maxHits}  (Spam [SPASI] / [J] untuk Serang!)");
        }

        // 2. Layar Victory saat Boss Kalah
        if (isDefeated)
        {
            int vWidth = 380;
            int vHeight = 180;
            int vx = (Screen.width - vWidth) / 2;
            int vy = (Screen.height - vHeight) / 2;

            GUI.Box(new Rect(vx, vy, vWidth, vHeight), "★ VICTORY! ★");
            GUI.Label(new Rect(vx + 20, vy + 35, vWidth - 40, 40), "The Gatekeeper Has Fallen!\nAethelgard berhasil diselamatkan dari anomali waktu!");

            if (GUI.Button(new Rect(vx + 30, vy + 95, 150, 45), "Main Lagi [R]"))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (GUI.Button(new Rect(vx + 200, vy + 95, 150, 45), "Main Menu [M]"))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
