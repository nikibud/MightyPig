using UnityEngine;
using System.Collections;
using TMPro; // Required for TextMeshPro
public class PigHP : MonoBehaviour , IDamageable
{
    public SpriteRenderer pigSpriteRenderer;
    public SpriteRenderer ballPigSpriteRenderer;
    public TextMeshProUGUI gameoOverText;
    public float MaxHealth = 500; 
    public float health;
    public Material whiteFlashMat;
    
    // ADD THIS LINE HERE
    private Material originalMat;
    private Material originalBallMat;
    void Start()
    {
        health = MaxHealth;
        GameObject foundObject = GameObject.Find("GameOverText");
        gameoOverText = foundObject.GetComponent<TextMeshProUGUI>();
        // Store the starting material so we can go back to it later
        originalMat = pigSpriteRenderer.material; 
        originalBallMat = ballPigSpriteRenderer.material;
    }
    public void TakeDamage(float hitDamage)
    {
       
        StartCoroutine(FlashWhite());
        health -= hitDamage;
        
        if(health <= 0) gameoOverText.text = "You Win";

    }

    IEnumerator FlashWhite()
    {
        // If you have a shader with a "Flash" property
        // 0 = Normal, 1 = Solid White
        Debug.Log("flash white");
        pigSpriteRenderer.material = whiteFlashMat; // Looks solid white
        ballPigSpriteRenderer.material = whiteFlashMat;
        yield return new WaitForSeconds(0.1f);
        ballPigSpriteRenderer.material = originalBallMat;
        pigSpriteRenderer.material = originalMat; // Back to normal

    }
}
