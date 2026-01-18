using UnityEngine;

public class PigHP : MonoBehaviour , IDamageable
{
    public float health = 500;
    public void TakeDamage(float hitDamage)
    {
        health -= hitDamage;
        Debug.Log("ay! my current health is: " + health);
        if(health <= 0) Debug.Log("IM DEAD");

    }
}
