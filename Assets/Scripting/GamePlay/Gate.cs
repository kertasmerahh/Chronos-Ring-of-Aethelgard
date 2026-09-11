using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateController : MonoBehaviour
{
    [Header("Pengaturan Waktu & Scene")]
    [Tooltip("Waktu tunggu sebelum gate di-spawn (detik)")]
    public float delayMuncul = 30f;
    public string namaSceneMenu = "MainMenu";

    [Header("Pengaturan Spawner Gate")]
    [Tooltip("Titik kemunculan gate di sebelah kanan")]
    public Transform titikSpawn;
    [Tooltip("Objek lingkaran tanah berputar")]
    public Transform groundTanah;
    [Tooltip("Tempelkan gate ke tanah saat muncul agar ikut berputar menghampiri player")]
    public bool spawnSebagaiObjekTanah = true;

    [Header("Pengaturan Orientasi & Posisi")]
    [Tooltip("Offset rotasi (derajat) jika ingin mengatur kemiringan secara manual")]
    public float rotasiOffset = 0f;
    [Tooltip("Offset ketinggian agar dasar gate menapak di permukaan tanah (tidak tenggelam)")]
    public float tinggiOffset = 1.1f;

    [Header("UI Pop-up (Canvas)")]
    [Tooltip("Tarik Panel Pop-up buatanmu di Canvas ke sini. Jika diisi, OnGUI otomatis dimatikan!")]
    public GameObject panelGatePopup;

    [Header("Objek Tambahan (Opsional)")]
    public GameObject spawnerMusuh;
    public BossGluttony bossGluttony;

    private SpriteRenderer spriteRend;
    private Collider2D col;
    private bool isGateChoiceActive = false;
    private bool isGateSpawned = false;

    void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // PENTING: Jika di scene Gate berada di dalam parent Ground, lepaskan dari parent
        // agar Gate TIDAK ikut berputar secara sembunyi-sembunyi dari awal!
        if (transform.parent != null)
        {
            if (groundTanah == null) groundTanah = transform.parent;
            transform.SetParent(null, true);
        }

        // Sembunyikan visual, matikan tabrakan, dan jauhkan sementara sebelum waktunya spawn
        if (spriteRend != null) spriteRend.enabled = false;
        if (col != null) col.enabled = false;
        if (panelGatePopup != null) panelGatePopup.SetActive(false);
        transform.position = new Vector3(999f, 999f, 0f);
    }

    void Start()
    {
        // Cari otomatis referensi titikSpawn dan groundTanah jika belum diisi di Inspector
        if (titikSpawn == null || groundTanah == null || spawnerMusuh == null)
        {
            AnomalySpawner spawner = Object.FindAnyObjectByType<AnomalySpawner>();
            if (spawner != null)
            {
                if (titikSpawn == null) titikSpawn = spawner.titikSpawn;
                if (groundTanah == null) groundTanah = spawner.groundTanah;
                if (spawnerMusuh == null) spawnerMusuh = spawner.gameObject;
            }
        }

        StartCoroutine(HitungMundurMuncul());
    }

    private IEnumerator HitungMundurMuncul()
    {
        yield return new WaitForSeconds(delayMuncul);
        MunculkanGateSekarang();
    }

    public void MunculkanGateSekarang()
    {
        if (isGateSpawned) return;
        isGateSpawned = true;

        // 1. Hentikan spawner musuh agar jalan menuju Gate bersih
        if (spawnerMusuh != null)
        {
            AnomalySpawner spawner = spawnerMusuh.GetComponent<AnomalySpawner>();
            if (spawner != null)
            {
                spawner.StopSpawning();
            }
            else
            {
                spawnerMusuh.SetActive(false);
            }
        }

        // 2. Letakkan Gate di posisi titikSpawn dan pastikan BERDIRI TEGAK
        if (titikSpawn != null)
        {
            Vector3 pos = titikSpawn.position;
            Quaternion rot = titikSpawn.rotation;

            if (groundTanah != null)
            {
                // Arah tegak lurus keluar dari pusat lingkaran tanah (radial UP)
                Vector3 radialUp = (titikSpawn.position - groundTanah.position).normalized;

                // Rotasikan agar sumbu Y (tinggi kapsul) berdiri tegak mengarah ke luar/langit
                float angle = (Mathf.Atan2(radialUp.y, radialUp.x) * Mathf.Rad2Deg) - 90f + rotasiOffset;
                rot = Quaternion.Euler(0f, 0f, angle);

                // Offset ketinggian agar dasar gerbang pas menapak di permukaan tanah (tidak tenggelam separuh)
                pos = titikSpawn.position + (radialUp * tinggiOffset);
            }

            transform.position = pos;
            transform.rotation = rot;
        }

        // 3. BARU tempelkan Gate ke lingkaran tanah berputar
        // Gate sekarang mulai bergerak menghampiri player dari kanan ke kiri!
        if (groundTanah != null && spawnSebagaiObjekTanah)
        {
            transform.SetParent(groundTanah, true);
        }

        // 4. Nyalakan visual dan collider gate
        if (spriteRend != null) spriteRend.enabled = true;
        if (col != null) col.enabled = true;

        Debug.Log("--- GATE BERHASIL DI-SPAWN! Gerbang sedang bergerak menghampiri player... ---");
    }

    void Update()
    {
        // Shortcut testing cepat: Tekan G untuk langsung memunculkan Gate di titik spawn
        if (Input.GetKeyDown(KeyCode.G) && !isGateSpawned)
        {
            StopCoroutine("HitungMundurMuncul");
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
            if (panelGatePopup != null) panelGatePopup.SetActive(true);
            Debug.Log("--- PLAYER MENYENTUH GATE: Menampilkan pilihan takdir ---");
        }
    }

    public void PilihLanjutKeBoss()
    {
        isGateChoiceActive = false;
        if (panelGatePopup != null) panelGatePopup.SetActive(false);

        // 1. Pastikan spawner musuh mati total
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
        if (panelGatePopup != null) panelGatePopup.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(namaSceneMenu);
    }

    void OnGUI()
    {
        // Jika kamu sudah membuat Panel UI di Canvas dan memasukkannya ke Inspector, OnGUI otomatis berhenti digambar!
        if (panelGatePopup != null) return;

        if (isGateChoiceActive)
        {
            int pWidth = 420;
            int pHeight = 210;
            int px = (Screen.width - pWidth) / 2;
            int py = (Screen.height - pHeight) / 2;

            // Box Pop-up
            GUI.Box(new Rect(px, py, pWidth, pHeight), "GATE OF AETHELGARD");
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