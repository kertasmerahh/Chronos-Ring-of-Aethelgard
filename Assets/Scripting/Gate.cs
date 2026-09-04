using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateController : MonoBehaviour
{
    [Header("Pengaturan")]
    public float delayMuncul = 10f;       // Waktu tunggu sebelum gate aktif
    public string namaSceneMenu = "MainMenuScene"; // Nama scene menu kamu

    [Header("Objek Tambahan (Opsional)")]
    public GameObject spawnerMusuh;       // Tarik objek Spawner ke sini agar musuh berhenti keluar

    private SpriteRenderer spriteRend;
    private Collider2D col;

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

        // Matikan spawner musuh jika diisi
        if (spawnerMusuh != null) spawnerMusuh.SetActive(false);

        // Munculkan fisik dan visual gate
        if (spriteRend != null) spriteRend.enabled = true;
        if (col != null) col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Langsung pindah ke Main Menu begitu disentuh Player
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}