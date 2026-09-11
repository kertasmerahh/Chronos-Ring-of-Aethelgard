using UnityEngine;

public class AnomalyHealth : MonoBehaviour
{
    [Header("Darah Anomali")]
    public int health = 1;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Hancurkan objek Anomali saat ditebas pedang/skill
        Destroy(gameObject);
    }

    // Tabrakan dengan Player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Ambil script PlayerHealth di objek Player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }

            // Hancurkan anomali setelah berhasil menabrak Player
            Destroy(gameObject);
        }
    }
}