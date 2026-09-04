using UnityEngine;

public class AnomalyHealth : MonoBehaviour
{
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
        // Hancurkan objek Anomali
        Destroy(gameObject);
    }
}