using UnityEngine;
using TMPro; // Required for TextMeshPro
public class PlayerHealth : MonoBehaviour
{
    private Vector2 offsetPerChild = new Vector3(1.5f, 0);
    public int maxHealth=5;
    private int currentHealth;
    public GameObject healthPrefab;
    public TextMeshProUGUI gameoOverText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject foundObject = GameObject.Find("GameOverText");
        gameoOverText = foundObject.GetComponent<TextMeshProUGUI>();
        currentHealth=maxHealth;
        for (int i = 0; i < maxHealth ; i++)
        {
            GameObject newChild = Instantiate(healthPrefab);
            newChild.transform.SetParent(this.transform);
            newChild.transform.localPosition = offsetPerChild * i * 2.5f;
            newChild.name = "hp" + (i +1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount) 
    {
        for (int i = 0; i < amount ; i++)
        {
            Transform childTransform = transform.Find("hp"+currentHealth);
            if (childTransform != null)
            {
                // 2. Try to get the SpriteRenderer from that child
                SpriteRenderer healthSprite = childTransform.GetComponent<SpriteRenderer>();

                if (healthSprite != null)
                {
                    // 3. Apply the color
                    healthSprite.color = Color.black;
                }
            }
            else if(currentHealth <= 1)
            {
                gameoOverText.text = "You Lose";
            }
            else
            {
                Debug.LogWarning("Could not find child named: " + " hp"+currentHealth);
            }
            currentHealth -= 1;
            Debug.Log("Player health is now: " + currentHealth);
        }
        

    }
}
