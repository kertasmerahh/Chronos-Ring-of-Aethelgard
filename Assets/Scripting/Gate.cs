using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateController : MonoBehaviour
{
    [Header("Pengaturan")]
    public float delayMuncul = 30f;       // Waktu tunggu sebelum gate aktif
    public string namaSceneMenu = "MainMenu"; 

    [Header("Objek Tambahan (Opsional)")]
    public GameObject spawnerMusuh;       // Tarik objek Spawner ke sini agar musuh berhenti keluar
    public BossGluttony bossGluttony;     // Tarik objek Boss Gluttony ke sini jika sudah ada

    private SpriteRenderer spriteRend;
    private Collider2D col;
    private bool isGateChoiceActive = false;

    void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Sembunyikan gate dan matikan tabrakannya di awal
        if (spriteRend != null) spriteRend.enabled = false;
        if (col != null) col.enabled = false;
    }

    void Start()
    {
        StartCoroutine(HitungMundurMuncul());
    }

    private IEnumerator HitungMundurMuncul()
    {
        yield return new WaitForSeconds(delayMuncul);
        MunculkanGateSekarang();
    }

    public void MunculkanGateSekarang()
    {
        // Munculkan fisik dan visual gate
        if (spriteRend != null) spriteRend.enabled = true;
        if (col != null) col.enabled = true;

        Debug.Log("--- GATE TERBUKA! Dekati gate untuk memilih takdirmu! ---");
    }

    void Update()
    {
        // Shortcut testing cepat: Tekan G untuk langsung memunculkan Gate
        if (Input.GetKeyDown(KeyCode.G) && spriteRend != null && !spriteRend.enabled)
        {
            StopCoroutine(HitungMundurMuncul());
            MunculkanGateSekarang();
        }

        // Mendengarkan input keyboard saat game di-pause di Gate
        if (isGateChoiceActive)
        {
            // Opsi 1: Boss Fight ([1], [Keypad 1], [Enter])
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1) ||
                Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                PilihLanjutKeBoss();
            }
            // Opsi 2: Main Menu ([2], [Keypad 2], [Escape])
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2) ||
                     Input.GetKeyDown(KeyCode.Escape))
            {
                PilihKeMainMenu();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Pause gameplay saat menyentuh Gate
            Time.timeScale = 0f;
            isGateChoiceActive = true;
            Debug.Log("--- PLAYER MENYENTUH GATE: Menampilkan pilihan takdir ---");
        }
    }

    public void PilihLanjutKeBoss()
    {
        isGateChoiceActive = false;

        // 1. Matikan spawner musuh
        if (spawnerMusuh != null)
        {
            spawnerMusuh.SetActive(false);
        }
        else
        {
            AnomalySpawner spawner = Object.FindAnyObjectByType<AnomalySpawner>();
            if (spawner != null) spawner.gameObject.SetActive(false);
        }

        // 2. Hentikan rotasi tanah (WorldRotator)
        WorldRotator rotator = Object.FindAnyObjectByType<WorldRotator>();
        if (rotator != null)
        {
            rotator.rotationSpeed = 0f;
            rotator.enabled = false;
        }

        // 3. Bersihkan sisa anomali di arena
        AnomalyHealth[] allEnemies = Object.FindObjectsByType<AnomalyHealth>(FindObjectsSortMode.None);
        foreach (AnomalyHealth enemy in allEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }

        // 4. Sembunyikan Gate
        if (spriteRend != null) spriteRend.enabled = false;
        if (col != null) col.enabled = false;

        // 5. Resume game dan aktifkan Boss Gluttony
        Time.timeScale = 1f;

        if (bossGluttony != null)
        {
            bossGluttony.ActivateBoss();
        }
        else
        {
            BossGluttony boss = BossGluttony.EnsureBossExists();
            boss.ActivateBoss();
        }

        gameObject.SetActive(false);
    }

    public void PilihKeMainMenu()
    {
        isGateChoiceActive = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(namaSceneMenu);
    }

    void OnGUI()
    {
        if (isGateChoiceActive)
        {
            int pWidth = 420;
            int pHeight = 210;
            int px = (Screen.width - pWidth) / 2;
            int py = (Screen.height - pHeight) / 2;

            // Box Pop-up
            GUI.Box(new Rect(px, py, pWidth, pHeight), "⛩️ GATE OF AETHELGARD ⛩️");
            GUI.Label(new Rect(px + 20, py + 35, pWidth - 40, 45), 
                "Gerbang waktu telah terbuka! Pilih tindakanmu:\n" +
                "[1] Hadapi Gatekeeper (Boss Gluttony)\n" +
                "[2] Selesai petualangan & Kembali ke Main Menu");

            // Tombol Opsi 1
            if (GUI.Button(new Rect(px + 30, py + 105, 360, 40), "[1] Hadapi Boss Gluttony (Enter / 1)"))
            {
                PilihLanjutKeBoss();
            }

            // Tombol Opsi 2
            if (GUI.Button(new Rect(px + 30, py + 152, 360, 40), "[2] Selesai & Ke Menu (Esc / 2)"))
            {
                PilihKeMainMenu();
            }
        }
    }
}