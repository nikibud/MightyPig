using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class PigAttack : MonoBehaviour
{
    public Transform visualChild;
    public BoxCollider2D normalCollider;
    public CircleCollider2D ballCollider;
    public GameObject ballForm;
    public GameObject baseForm;
    public Rigidbody2D rb;


    [Header("Charge Attack")]
    public float chargeSpeed = 10f;
    public float prepareTime = 1f; // The "Warning" phase
    private bool hitWall = false;
    private bool isAttacking = false;
    private float faceDirection;

    [Header("Bounce Attack")]
    public float bounceForce = 7f;
    public float bounceSpeed = 10f;
    public int maxBounces = 3;
    private bool isBouncingPhase = false;
    private int currentBounces = 0;

    [Header("Scatter Attack")]
    public GameObject projectilePrefab;
    public float scatterSpeed = 3f;
    public int projectileCount = 6;
    public float spreadAngle = 60f; // The total width of the arc
    public float projectileForce = 20f;
    public float spawnDistance = 1.5f;

    public Transform player; // Drag the Player here in Inspector
    public SpriteRenderer childSprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame && !isAttacking) {
            StartCoroutine(ChargeAttack()); 
        }
        if (Keyboard.current.yKey.wasPressedThisFrame && !isAttacking) {
            StartCoroutine(BounceAttack()); 
        }
        if (Keyboard.current.uKey.wasPressedThisFrame && !isAttacking) {
            StartCoroutine(ScatterShotAttack()); 
        }
    }
    IEnumerator ScatterShotAttack()
    {
        isAttacking = true;
        hitWall = false;

        // 1. WALK TO THE WALL
        FlipSprite(true);
        while (!hitWall) 
        {
            rb.linearVelocity = new Vector2(faceDirection * scatterSpeed, rb.linearVelocity.y);
            yield return null;
        }
        FlipSprite(false);
        rb.linearVelocity = new Vector2(faceDirection * 5f, 1f);
        yield return new WaitForSeconds(0.2f);
        // 2. PREPARE
        rb.linearVelocity = Vector2.zero;
        childSprite.color = Color.cyan;
        yield return new WaitForSeconds(0.5f);

        // 3. THROW RANDOMIZED PROJECTILES
        
        float centerAngle = faceDirection == 1 ? 45f : 135f;
        
        // How much "chaos" do you want? 
        // Higher number = projectiles fly more wildly.
        float randomVariation = 15f; 

        for (int i = 0; i < projectileCount; i++)
        {
            // Calculate the base angle in the arc
            float baseAngle = (centerAngle - (spreadAngle / 2)) + (spreadAngle / (projectileCount - 1)) * i;
            
            // ADD THE RANDOMNESS HERE
            float finalAngle = baseAngle + Random.Range(-randomVariation, randomVariation);

            // Convert angle to Direction
            float radian = finalAngle * Mathf.Deg2Rad;
            Vector2 launchDir = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

            Vector3 spawnPos = transform.position + (new Vector3(launchDir.x, launchDir.y, 0) * spawnDistance);
            // Spawn and launch
            GameObject dirt = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Rigidbody2D dirtRb = dirt.GetComponent<Rigidbody2D>();
            
            dirtRb.linearVelocity = launchDir * projectileForce;
        }

        // 4. RECOVERY
        childSprite.color = Color.white;
        yield return new WaitForSeconds(1f);
        childSprite.color = Color.red;
        isAttacking = false;
    }
    IEnumerator ChargeAttack() {
        FlipSprite(false);
        isAttacking = true;
        // 1. WARNING PHASE (Telegraph)
        // Boss stops and looks at player
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Boss is preparing to charge!");
        childSprite.color = Color.yellow;
        // Optional: Change color to red or play an animation here
        
        yield return new WaitForSeconds(prepareTime);

        // 2. CALCULATE DIRECTION
        // We decide the direction once at the start of the rush
        
        childSprite.color = Color.red;
        // 3. RUSH PHASE
        float startTime = Time.time;
        while (!hitWall) {
            rb.linearVelocity = new Vector2(faceDirection * chargeSpeed, rb.linearVelocity.y);
            yield return null; // Wait for next frame
        }
        rb.linearVelocity = new Vector2(-faceDirection * 5f, 2f);
        yield return new WaitForSeconds(0.2f);
        
        // 4. RECOVERY PHASE
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Boss is tired...");
        childSprite.color = Color.white;
        yield return new WaitForSeconds(1.5f); // Boss is vulnerable here
        
        childSprite.color = Color.red;
        isAttacking = false;
        FlipSprite(false);
    }
    IEnumerator BounceAttack()
    {
        FlipSprite(false);
        currentBounces = 0;
        isBouncingPhase = true;
        isAttacking = true;
        // 1. SWAP TO CIRCLE
        normalCollider.enabled = false;
        ballCollider.enabled = true;
        ballForm.SetActive(true);
        baseForm.SetActive(false);

        childSprite.color = Color.magenta; 
        yield return new WaitForSeconds(0.5f);

        // ... (Your bounce logic here) ...
        //rb.linearVelocity = new Vector2(faceDirection * bounceSpeed, bounceForce);

        while (currentBounces <= maxBounces)
        {
            rb.linearVelocity = new Vector2(faceDirection * bounceSpeed, rb.linearVelocity.y);
            yield return null;
        }

        // 2. SWAP BACK TO BOX
        isBouncingPhase = false;
        isAttacking = false;
        childSprite.color = Color.white;
        rb.linearVelocity = Vector2.zero;

        transform.rotation = Quaternion.identity;
        visualChild.localRotation = Quaternion.Euler(0, 0, 90f);
        ballCollider.enabled = false;
        normalCollider.enabled = true;
        ballForm.SetActive(false);
        baseForm.SetActive(true);
        FlipSprite(false);
        yield return new WaitForSeconds(1f);
        childSprite.color = Color.red;
        
    }
    
    void FlipSprite(bool flip)
    {
        faceDirection = (player.position.x > transform.position.x) ? 1 : -1;
        if(flip) faceDirection = -faceDirection ;
        if (faceDirection > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (faceDirection < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        
    }
    public void HitWall(bool state)
    {
        if(isBouncingPhase && state) faceDirection=-faceDirection;
        hitWall = state;   
    }
    
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("hit wall");
            HitWall(true);
        } 
            
        
       
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")  && isBouncingPhase)
        {
            Debug.Log("hit ground and i bounced " + currentBounces + "times");
            rb.linearVelocity = new Vector2(faceDirection * bounceSpeed, bounceForce);
            currentBounces++;
        }
        
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("hit wall");
            HitWall(false);
        } 
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // 1. Determine direction (same logic as your attack)
        float facePlayerDir = (player.position.x > transform.position.x) ? 1 : -1;
        float centerAngle = facePlayerDir == 1 ? 45f : 135f;

        // 2. Calculate the edge angles
        float leftAngle = centerAngle - (spreadAngle / 2);
        float rightAngle = centerAngle + (spreadAngle / 2);

        // 3. Convert angles to Direction Vectors
        Vector3 leftDir = new Vector3(Mathf.Cos(leftAngle * Mathf.Deg2Rad), Mathf.Sin(leftAngle * Mathf.Deg2Rad), 0);
        Vector3 rightDir = new Vector3(Mathf.Cos(rightAngle * Mathf.Deg2Rad), Mathf.Sin(rightAngle * Mathf.Deg2Rad), 0);

        // 4. Draw the Cone
        Gizmos.color = Color.cyan;
        
        // Draw the "Muzzle" spawn offset
        Vector3 spawnCenter = transform.position + (new Vector3(Mathf.Cos(centerAngle * Mathf.Deg2Rad), 0, 0) * 1.5f);
        Gizmos.DrawWireSphere(spawnCenter, 0.3f);

        // Draw the limit lines (how far the projectiles fly roughly)
        float debugRange = 5f; 
        Gizmos.DrawLine(spawnCenter, spawnCenter + leftDir * debugRange);
        Gizmos.DrawLine(spawnCenter, spawnCenter + rightDir * debugRange);

        // Draw an arc connecting the lines
        Gizmos.DrawWireSphere(spawnCenter, debugRange);
    }


}

