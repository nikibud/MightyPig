using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // For Triggers, we check 'other.gameObject' or just 'other'
        if (other.CompareTag("Wall") || other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.CompareTag("Player"))
        {

            Destroy(gameObject);
        } 
    }
}
