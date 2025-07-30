using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 10;

    void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
        }
    }
}
