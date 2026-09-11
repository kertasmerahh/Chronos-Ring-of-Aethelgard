using UnityEngine;

public class AnomalySpawner : MonoBehaviour
{
    [Header("Anomali Darat")]
    [Tooltip("Prefab untuk musuh anomali darat")]
    public GameObject anomalyPrefab;          // Prefab kotak merah (Anomaly Darat)
    [Tooltip("Jeda waktu muncul musuh darat (detik)")]
    public float jedaWaktu = 3f;

    [Header("Anomali Udara")]
    [Tooltip("Prefab untuk musuh anomali udara / terbang")]
    public GameObject anomalyUdaraPrefab;     // Prefab Anomaly Udara (terbang)
    [Tooltip("Jeda waktu muncul musuh udara (detik)")]
    public float jedaWaktuUdara = 5f;
    [Tooltip("Ketinggian terbang musuh udara dari permukaan tanah")]
    public float ketinggianUdara = 1.8f;

    [Header("Referensi Lingkungan")]
    [Tooltip("Titik penanda lokasi spawn")]
    public Transform titikSpawn;              // Objek penanda di kanan
    [Tooltip("Objek lingkaran tanah berputar")]
    public Transform groundTanah;             // Lingkaran tanah berputar

    [Header("Status Spawner")]
    public bool isSpawning = true;

    private float timerDarat = 0f;
    private float timerUdara = 0f;

    void Start()
    {
        // Jika Anomaly Udara belum di-assign di Inspector, coba cari atau gunakan default
        if (anomalyUdaraPrefab == null && anomalyPrefab != null)
        {
            Debug.Log("[AnomalySpawner] anomalyUdaraPrefab belum di-assign di Inspector, bisa ditarik dari Assets/Anomaly Udara.prefab");
        }
    }

    void Update()
    {
        if (!isSpawning) return;

        // 1. Timer dan spawn musuh darat
        timerDarat += Time.deltaTime;
        if (timerDarat >= jedaWaktu)
        {
            MunculkanMusuhDarat();
            timerDarat = 0f;
        }

        // 2. Timer dan spawn musuh udara (hanya jika prefab tersedia)
        if (anomalyUdaraPrefab != null)
        {
            timerUdara += Time.deltaTime;
            if (timerUdara >= jedaWaktuUdara)
            {
                MunculkanMusuhUdara();
                timerUdara = 0f;
            }
        }
    }

    public void MunculkanMusuhDarat()
    {
        if (anomalyPrefab == null || titikSpawn == null || groundTanah == null) return;

        // Gandakan musuh di posisi TitikSpawn
        GameObject musuhBaru = Instantiate(anomalyPrefab, titikSpawn.position, titikSpawn.rotation);

        // Tempelkan musuh ke tanah agar ikut berputar mengitari lingkaran tanah
        musuhBaru.transform.SetParent(groundTanah, true);
    }

    public void MunculkanMusuhUdara()
    {
        if (anomalyUdaraPrefab == null || titikSpawn == null || groundTanah == null) return;

        // Hitung arah keluar tegak lurus dari pusat lingkaran tanah (radial direction)
        Vector3 arahDariPusat = (titikSpawn.position - groundTanah.position).normalized;
        Vector3 posisiUdara = titikSpawn.position + (arahDariPusat * ketinggianUdara);

        // Gandakan musuh udara di ketinggian melayang
        GameObject musuhUdara = Instantiate(anomalyUdaraPrefab, posisiUdara, titikSpawn.rotation);

        // Tempelkan musuh udara ke tanah berputar dengan worldPositionStays = true
        // agar ikut mengorbit planet di ketinggian udara
        musuhUdara.transform.SetParent(groundTanah, true);
    }

    public void StopSpawning()
    {
        isSpawning = false;
        Debug.Log("[AnomalySpawner] Spawning dihentikan.");
    }

    public void ResumeSpawning()
    {
        isSpawning = true;
    }
}