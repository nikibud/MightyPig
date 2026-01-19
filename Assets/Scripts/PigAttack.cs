using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;
public class PigAttack : MonoBehaviour
{
    public Transform visualChild;
    public CapsuleCollider2D normalCollider;
    public Rigidbody2D rb;
    public GameObject attackHitbox;


    [Header("Charge Attack")]
    public float chargeSpeed = 10f;
    public float prepareTime = 1f; // The "Warning" phase
    private bool hitWall = false;
    public bool isAttacking = false;
    private float faceDirection;

    [Header("Bounce Attack")]
    public CircleCollider2D ballCollider;
    public GameObject ballForm;
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

    


    [Header("Animations")]
    public Animator anim; // Drag your Pig's Animator here
    public GameObject pigSprite;
    public SpriteSkin spriteSkin;
    
    [Header("Stuff")]
    public Transform player; // Drag the Player here in Inspector
    //public SpriteRenderer childSprite;
    private PigAttackPattern pigAttackPattern;

    void Start()
    {
        spriteSkin = GetComponentInChildren<SpriteSkin>();
        rb = GetComponent<Rigidbody2D>();
        FlipSprite(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (hitWall)
        {
        //   rb.linearVelocity = new Vector2(faceDirection * 2f, 1f);
        }
    }
    public void FireScatterShot()
    {
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

            Vector3 spawnPos = transform.position + new Vector3(launchDir.x* spawnDistance, launchDir.y, 0) ;
            // Spawn and launch
            GameObject dirt = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Rigidbody2D dirtRb = dirt.GetComponent<Rigidbody2D>();

            // Choose a random spin speed (positive is counter-clockwise, negative is clockwise)
            float spinSpeed = Random.Range(360f, 720f) * faceDirection;; 

            //dirtRb.angularVelocity = spinSpeed;
            dirtRb.linearVelocity = launchDir * projectileForce;
        }
    }
    public IEnumerator ScatterShotAttack()
    {
        isAttacking = true;
        hitWall = false;

        // 1. WALK TO THE WALL
        FlipSprite(true);
        anim.SetBool("Wallking", true);
        while (!hitWall) 
        {
            rb.linearVelocity = new Vector2(faceDirection * scatterSpeed, rb.linearVelocity.y);
            yield return null;
        }
        anim.SetBool("Wallking", false);
        FlipSprite(false);
        while (hitWall)
        {
            rb.linearVelocity = new Vector2(faceDirection * scatterSpeed, rb.linearVelocity.y);
            yield return null;
        }
        anim.SetTrigger("ThrowMud");
        // 2. PREPARE
        rb.linearVelocity = Vector2.zero;
        
        

        yield return new WaitForSeconds(1f);
        //pigAttackPattern.ChangeState(BossState.Idle);
        isAttacking = false;
    }
    public IEnumerator ChargeAttack() {
        FlipSprite(false);
        isAttacking = true;
        // 1. WARNING PHASE (Telegraph)
        // Boss stops and looks at player
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Boss is preparing to charge!");

        // Optional: Change color to red or play an animation here
        
        yield return new WaitForSeconds(prepareTime);

        // 2. CALCULATE DIRECTION
        // We decide the direction once at the start of the rush
        anim.SetBool("isCharging", true); // START ANIMATION

        // 3. RUSH PHASE
        float startTime = Time.time;
        while (!hitWall) {
            rb.linearVelocity = new Vector2(faceDirection * chargeSpeed, rb.linearVelocity.y);
            yield return null; // Wait for next frame
        }
        anim.SetBool("isCharging", false); // STOP ANIMATION
        anim.SetBool("hitWall", true); // STOP ANIMATION
        rb.linearVelocity = new Vector2(-faceDirection * 10f, 4f);
        yield return new WaitForSeconds(0.2f);
        
        // 4. RECOVERY PHASE
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Boss is tired...");
        yield return new WaitForSeconds(1.5f); // Boss is vulnerable here
        
        anim.SetBool("hitWall", false); // STOP ANIMATION
        isAttacking = false;
        FlipSprite(false);
    }
    public IEnumerator HeadButt()
    {
        attackHitbox.transform.localPosition = new Vector2(1.7f , 0);
        yield return new WaitForSeconds(0.3f);
        attackHitbox.transform.localPosition = new Vector2(0, 0);
        isAttacking = false;
    }
    public IEnumerator BounceAttack()
    {
        FlipSprite(false);
        currentBounces = 0;
        isBouncingPhase = true;
        isAttacking = true;
        // 1. SWAP TO CIRCLE
        normalCollider.enabled = false;
        ballCollider.enabled = true;
        ballForm.SetActive(true);
        pigSprite.SetActive(false);
        attackHitbox.SetActive(false);
        rb.constraints = RigidbodyConstraints2D.None;

        yield return new WaitForSeconds(0.5f);

        // ... (Your bounce logic here) ...
        //rb.linearVelocity = new Vector2(faceDirection * bounceSpeed, bounceForce);

        while (currentBounces < maxBounces)
        {
            rb.linearVelocity = new Vector2(faceDirection * bounceSpeed, rb.linearVelocity.y);
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;
        // 2. SWAP BACK TO BOX
        isBouncingPhase = false;
        isAttacking = false;
        rb.linearVelocity = Vector2.zero;

        anim.Play("Idle", 0, 0f);
        anim.Update(0f);
        transform.rotation = Quaternion.identity;
        visualChild.localRotation = Quaternion.Euler(0, 0, 0);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        ballCollider.enabled = false;
        normalCollider.enabled = true;
        ballForm.SetActive(false);
        pigSprite.SetActive(true);
        attackHitbox.SetActive(true);
        FlipSprite(false);
        
        yield return new WaitForSeconds(1f);

        
    }
    
    
    void FlipSprite(bool flip)
    {
        faceDirection = (player.position.x > transform.position.x) ? 1 : -1;
        if(flip) faceDirection = -faceDirection ;
        transform.localScale = new Vector3(3*faceDirection, 3, 1);
        
        
    }
    public void HitWall(bool state)
    {
        if (state)
        {
            if(isBouncingPhase ) faceDirection=-faceDirection;
        }
        
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

