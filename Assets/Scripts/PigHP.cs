using UnityEngine;
using System.Collections;
public class PigHP : MonoBehaviour , IDamageable
{
    public SpriteRenderer pigSpriteRenderer;
    public float health = 500;
    public Material whiteFlashMat;
    
    // ADD THIS LINE HERE
    private Material originalMat;
    void Start()
    {
        // Store the starting material so we can go back to it later
        originalMat = pigSpriteRenderer.material; 
    }
    public void TakeDamage(float hitDamage)
    {
       
        StartCoroutine(FlashWhite());
        health -= hitDamage;
        
        if(health <= 0) Debug.Log("IM DEAD");

    }

    IEnumerator FlashWhite()
    {
        // If you have a shader with a "Flash" property
        // 0 = Normal, 1 = Solid White
        Debug.Log("flash white");
        pigSpriteRenderer.material = whiteFlashMat; // Looks solid white
        yield return new WaitForSeconds(0.1f);
        pigSpriteRenderer.material = originalMat; // Back to normal

    }
}
