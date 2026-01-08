using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        } 
            
        
       
        
    }
}
