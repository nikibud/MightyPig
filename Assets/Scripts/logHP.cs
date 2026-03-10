using UnityEngine;
using System.Collections;
public class logHP : MonoBehaviour , IDamageable
{
    public float health= 30 ; 
    private Material originalMat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private SpriteRenderer sr;
    public Material whiteFlashMat;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalMat = sr.material; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float hitDamage)
    {
       
        StartCoroutine(FlashWhite());
        health -= hitDamage;
        
        
        if(health <= 0) gameObject.SetActive(false);

    }
    IEnumerator FlashWhite()
    {
        // If you have a shader with a "Flash" property
        // 0 = Normal, 1 = Solid White
        Debug.Log("flash white");
        sr.material = whiteFlashMat; // Looks solid white
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.brown;
        sr.material = originalMat; // Back to normal

    }
}
