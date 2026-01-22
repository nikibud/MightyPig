using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    public float rotationSpeed = 500f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
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
