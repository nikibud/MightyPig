using UnityEngine;
using System.Collections;
public class BeeHP : MonoBehaviour , IDamageable
{
    public CircleCollider2D Collider;
    public Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("bee got hit");
        StartCoroutine(ColliderDisable());
        anim.SetTrigger("explode");
        return;
    }
    IEnumerator ColliderDisable()
    {
        Collider.enabled = false;
        yield return new WaitForSeconds(1.6f);
        Collider.enabled = true;
    }
}
