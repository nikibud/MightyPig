using UnityEngine;
using TMPro; // Required for TextMeshPro
using UnityEngine.UI;
using Cinemachine;
public class PlayerHealth : MonoBehaviour
{
    public GameObject Pig;
    public PigAttack pigAttack;
    public StartBossFight startBossFight;
    public PigAttackPattern PigAttackPattern;
    public PigHP pigHP;
    public controls controls;
    public CinemachineVirtualCamera vcam_Boss;
    private Vector2 offsetPerChild = new Vector3(1.5f, 0);
    public int maxHealth=5;
    private int currentHealth;
    public GameObject healthPrefab;
    public TextMeshProUGUI gameoOverText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth=maxHealth;
        GameObject foundObject = GameObject.Find("GameOverText");
        gameoOverText = foundObject.GetComponent<TextMeshProUGUI>();
        for (int i = 0; i < maxHealth ; i++)
        {
            GameObject newChild = Instantiate(healthPrefab);
            newChild.transform.SetParent(this.transform);
            newChild.transform.localPosition = offsetPerChild * i * 2.5f;
            newChild.name = "hp" + (i +1);
        }
    }

    public void ResetHealth()
    {
        currentHealth=maxHealth+1;
        for (int i = 1; i < maxHealth+1 ; i++)
        {
            Transform childTransform = transform.Find("hp"+i);
            if (childTransform != null)
            {
                // 2. Try to get the SpriteRenderer from that child
                Image healthSprite = childTransform.GetComponent<Image>();

                if (healthSprite != null)
                {
                    // 3. Apply the color
                    healthSprite.color = Color.white;
                }
            }
        }
        gameoOverText.text = "";
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
                Debug.Log("i blacked hp"+currentHealth );
                // 2. Try to get the SpriteRenderer from that child
                Image healthImage = childTransform.GetComponent<Image>();

                if (healthImage != null)
                {
                    // 3. Apply the color
                    healthImage.color = Color.black;
                }
            }
            if(currentHealth <= 1)
            {

                gameoOverText.text = "You Lose";
                Reset();
                
            }
            else
            {
                Debug.LogWarning("Could not find child named: " + " hp"+currentHealth);
            }
            currentHealth -= 1;
        }
        

    }
    public void Reset()
    {
        if (PigAttackPattern.bossFightStarted)
        {
            PigAttackPattern.bossFightStarted = false; 
            pigHP.health = pigHP.MaxHealth;
            startBossFight.LeftTreeBorder.SetActive(false);
            pigAttack.FullReset();
            vcam_Boss.m_Priority = 5;
            Pig.SetActive(false);
        }
        controls.Respawn();
        ResetHealth();
    }
}
