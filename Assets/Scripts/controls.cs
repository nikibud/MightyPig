using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public enum PlayerAttack { Down, Up, Front }
public class controls : MonoBehaviour
{
    [Header("GameObjects")]
    public LayerMask groundLayer;
    public Rigidbody2D rb;
    private PlayerHealth playerHealth;
    public LayerMask enemyLayers;    // Set this to "Enemy" in Inspector
    //public SpriteRenderer bodySprite;

    [Header("Attacks")]
    public Transform attackPoint;    // Drag your AttackPoint object here
    public float attackRange = 0.5f; // Size of the hit circle
    public int attackDamage = 10;
    public float attackRate = 2f;
    float nextAttackTime = 0f;
    private bool isAttacking = false;

    [Header("Movment")]

    private float moveX = 0;
    private float horizontalInput;
    public float moveSpeed = 8f;
    public float jumpForce = 20f;
    public float jumpCutMultiplier = 0.3f; // How much speed to keep when letting go (0.5 = half)
    public float dashCooldown = 1f;
    public float dashPower = 20f;
    public float dashTime = 0.2f;
    private bool canDash = true;
    private bool isDashing;     
    private bool isGrounded;

    [Header("Damage")]
    private int playerLayer = 3;
    private int enemyAttackLayer=8;    
    private bool isKnockedBack =false;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;
    public CameraShake shakeScript;


    [Header("Animations")]
    public Animator anim; // Drag your Pig's Animator here
    


    


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        // Finds the object with the tag "Player" and gets the script
        GameObject player = GameObject.FindWithTag("PlayerHealth");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update() {
        // 1. Get Left/Right input
        
        if ((Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) && !isAttacking ) moveX = -1;
        else if ((Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) && !isAttacking ) moveX = 1;
        else if(isGrounded ) moveX = 0;
        horizontalInput = moveX;
        if(horizontalInput != 0 && isGrounded) anim.SetBool("Wallking",true);
        // 2. Check if we are touching the ground

        // 3. Jump Input
        if (Keyboard.current.zKey.wasPressedThisFrame && isGrounded) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            anim.SetBool("Jump0", true);
        }
        if (Keyboard.current.zKey.wasReleasedThisFrame) {
            if (rb.linearVelocity.y > 0) {
                // Cut the vertical velocity
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
        }
        
        if (Keyboard.current.cKey.wasPressedThisFrame && canDash) {
            StartCoroutine(Dash());
        }
        
        if (Time.time >= nextAttackTime) {
            if (Keyboard.current.xKey.wasPressedThisFrame && !isDashing) {
                Attack();
                nextAttackTime = Time.time + 1f /  attackRate;
            }
        }
        FlipSprite();
        
    }

    void FixedUpdate() {
        if(isDashing) return;
        if(!isKnockedBack){ 
            if (horizontalInput == 0 && isGrounded ) {
                // Force the player to a dead stop if no input is given
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                anim.SetBool("Wallking",false);
            } else {
                rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
                
            }
        }
        if (rb.linearVelocity.y < 0) {
            rb.gravityScale = 8f; // Fall fast
            anim.SetTrigger("Falling");
            anim.SetBool("Jump0", false);
        } else {
            
            rb.gravityScale = 3f; // Rise normally
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("i triggered somthing");
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyAttack"))
        {
            Debug.Log("i got hit");
            playerHealth.TakeDamage(1);
            StartCoroutine(TakeDamage(other.transform.position));
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            if(!isGrounded)
                anim.SetBool("Falling",false);
            isGrounded = true;
            anim.SetBool("Grounded", isGrounded);
            Debug.Log("im on the ground");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) 
        {
            isGrounded = false;
            anim.SetBool("Grounded", isGrounded);
        }
    }

    public IEnumerator TakeDamage(Vector2 attackerPos) {
        // KnockbackRoutine
        StartCoroutine(shakeScript.ManualShake(0.2f, 0.5f));

        Time.timeScale = 0f; 
        yield return new WaitForSecondsRealtime(0.1f); // Realtime ignores the 0 timescale
        Time.timeScale = 1f;

        // 2. Start Knockback State
        isKnockedBack = true;

        // 3. Directional Force
        // Calculate if the enemy is to our left or right
        float side = transform.position.x < attackerPos.x ? -1 : 1;
        
        rb.linearVelocity = Vector2.zero; // Stop current movement
        //if(rb.linearVelocity.y < 0) rb.AddForce(new Vector2(side * knockbackForce, knockbackForce * 1.5f ), ForceMode2D.Impulse);
        //else 
        rb.AddForce(new Vector2(side * knockbackForce, knockbackForce * 0.5f ), ForceMode2D.Impulse);
        horizontalInput = 0 ;
        // 4. Invincibility (I-Frames)
        StartCoroutine(InvincibilityRoutine());

        yield return new WaitForSeconds(knockbackDuration); // Duration of the "stun"
        isKnockedBack = false;
        
    }
    private IEnumerator InvincibilityRoutine()
    {
        // Layer 9 = Player, Layer 10 = Enemy (Example)
        // Ignore collisions so you don't get hit twice immediately
        Physics2D.IgnoreLayerCollision(playerLayer, enemyAttackLayer, true);

        // Visual Flash
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        for (int i = 0; i < 5; i++)
        {
            sr.color = new Color(1, 1, 1, 0.5f); // Semi-transparent
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        Physics2D.IgnoreLayerCollision(playerLayer, enemyAttackLayer, false);
    }
    public IEnumerator IsAttacking()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
    }
    void Attack() {
            
        // Check vertical input from the keyboard
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) {
            anim.SetTrigger("UpAttack");

            StartCoroutine(IsAttacking());
        }
        else if ((Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) && !isGrounded) {
            
            anim.SetTrigger("DownAttack");

        }
        else {
            anim.SetTrigger("FrontAttack");
            StartCoroutine(IsAttacking());
        }

        
    }
    public void AttackHitbox(PlayerAttack attack)
    {
        // 1. Determine the direction of the attack
        Vector3 attackOffset = Vector3.zero;
        float currentAttackRange = attackRange; // Start with your default range

        switch (attack)
        {
            case PlayerAttack.Up:
                attackOffset = Vector3.up * 2.5f; // Attack Above
                break;
            case PlayerAttack.Down:
                attackOffset = Vector3.down * 1.6f; // Attack Below (only if in air)
                currentAttackRange = attackRange * 1.5f; // Hitbox is 50% larger for pogo!
                break ;
            case PlayerAttack.Front:
                // Default: Attack Forward (multiplied by facing direction)
                attackOffset = new Vector3(transform.localScale.x * 2f, 0, 0);
                break ;
        }
        
        // 2. Set the position of the attack
        Vector3 finalPos = transform.position + attackOffset;

        // 3. Detect and Damage
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(finalPos, currentAttackRange, enemyLayers);
        //pogo logic
        if(hitEnemies.Length > 0){
            if (attackOffset.y < 0) {
                // Reset vertical velocity and add a small upward boost
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.85f); 
            }
        }
        foreach (Collider2D enemy in hitEnemies) {
            // This looks at the object hit, AND all its parents
            IDamageable hitObj = enemy.GetComponentInParent<IDamageable>();

            if (hitObj != null) 
            {
                hitObj.TakeDamage(10);
            }
        }
    }
    void FlipSprite()
    {
        /*
        Vector3 currentScale = transform.localScale;
        currentScale.x = Mathf.Abs(currentScale.x) * horizontalInput;
        visualsFolder.transform.localScale = currentScale;
        */
        if (!isAttacking)
        {
            if (horizontalInput > 0)
                transform.localScale = new Vector3(2, 2, 1);
            else if (horizontalInput < 0)
                transform.localScale = new Vector3(-2, 2, 1);
        }
       
    }
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // 1. Save original gravity and set it to 0 (so you don't fall during the dash)
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // 2. Apply Dash Velocity based on facing direction
        float direction = transform.localScale.x;
        rb.linearVelocity = new Vector2(direction * dashPower, 0f);

        // 3. Wait for the dash to finish
        yield return new WaitForSeconds(dashTime);

        // 4. Reset gravity and state
        rb.gravityScale = originalGravity;
        isDashing = false;

        // 5. Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    void OnDrawGizmosSelected() {
        // Draw the Forward attack zone
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(transform.localScale.x * 1.2f, 0, 0), attackRange);
        
        // Draw the Up attack zone
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.2f, attackRange);
        
        // Draw the Down attack zone
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * 1.6f, attackRange* 1.5f);
    }
}



    

