using UnityEngine;

public class AnomalySpawner : MonoBehaviour
{
    public GameObject anomalyPrefab;  // Prefab kotak merah
    public Transform titikSpawn;       // Objek penanda di kanan tadi
    public Transform groundTanah;      // Lingkaran tanah berputar
    public float jedaWaktu = 3f;       // Muncul tiap berapa detik

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= jedaWaktu)
        {
            MunculkanMusuh();
            timer = 0f;
        }
    }

    void MunculkanMusuh()
    {
        // 1. Gandakan musuh di posisi TitikSpawn
        GameObject musuhBaru = Instantiate(anomalyPrefab, titikSpawn.position, Quaternion.identity);

        // 2. Tempelkan musuh ke tanah biar ikut berputar menghampiri player
        musuhBaru.transform.SetParent(groundTanah, true);
    }
}