using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save the player's position to PlayerPrefs
            PlayerPrefs.SetFloat("CheckpointX", transform.position.x);
            PlayerPrefs.SetFloat("CheckpointY", transform.position.y);
            PlayerPrefs.Save(); // Forces Unity to write the data to disk
            
            Debug.Log("Checkpoint Saved at: " + transform.position);
        }
    }
    
}
