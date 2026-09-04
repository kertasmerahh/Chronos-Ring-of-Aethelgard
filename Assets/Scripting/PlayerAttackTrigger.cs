using UnityEngine;

public class PlayerAttackTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah objek yang terkena tebasan punya komponen AnomalyHealth
        AnomalyHealth anomaly = other.GetComponent<AnomalyHealth>();
        if (anomaly != null)
        {
            anomaly.TakeDamage(1);
        }
    }
}