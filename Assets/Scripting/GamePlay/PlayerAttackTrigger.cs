using UnityEngine;

public class PlayerAttackTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Cek apakah objek yang terkena tebasan punya komponen AnomalyHealth
        AnomalyHealth anomaly = other.GetComponent<AnomalyHealth>();
        if (anomaly != null)
        {
            anomaly.TakeDamage(1);
            return;
        }

        // 2. Cek apakah objek yang terkena tebasan adalah BossGluttony
        BossGluttony boss = other.GetComponent<BossGluttony>();
        if (boss != null)
        {
            boss.TakeHit();
        }
    }
}