using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class controls : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpForce = 20f;
    public float jumpCutMultiplier = 0.3f; // How much speed to keep when letting go (0.5 = half)
    public LayerMask groundLayer;
    public Rigidbody2D rb;
    private bool isGrounded;
    public float dashCooldown = 1f;
    public float dashPower = 20f;
    public float dashTime = 0.2f;
    private bool canDash = true;
    private bool isDashing;     


    public GameObject upAttackVisual;
    public GameObject downAttackVisual;
    public GameObject frontAttackVisual;
    public Transform attackPoint;    // Drag your AttackPoint object here
    public float attackRange = 0.5f; // Size of the hit circle
    public LayerMask enemyLayers;    // Set this to "Enemy" in Inspector
    public int attackDamage = 10;
    public float attackRate = 2f;
    float nextAttackTime = 0f;
    public SpriteRenderer bodySprite;


    private float moveX = 0;
    private float horizontalInput;


    void Start() {

    }

    void Update() {
        // 1. Get Left/Right input
        
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1;
        else moveX = 0;
        horizontalInput = moveX;
        // 2. Check if we are touching the ground

        // 3. Jump Input
        if (Keyboard.current.zKey.wasPressedThisFrame && isGrounded) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
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

        if (horizontalInput == 0 && isGrounded) {
            // Force the player to a dead stop if no input is given
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        } else {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
        if (rb.linearVelocity.y < 0) {
            rb.gravityScale = 4f; // Fall fast
        } else {
            rb.gravityScale = 3f; // Rise normally
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("i triggered somthing");
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("it was an enemy");
            StartCoroutine(FlashBlue());
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object we hit is on the "Enemy" layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) isGrounded = true;
        if (collision.gameObject.CompareTag("Projectile")||collision.gameObject.CompareTag("Enemy"))  StartCoroutine(FlashBlue());
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the object we hit is on the "Enemy" layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) isGrounded = false;
    }

    public IEnumerator FlashBlue() {
        bodySprite.color = Color.blue;        // Turn red
        yield return new WaitForSeconds(0.1f); // Wait a tiny bit
        bodySprite.color = Color.white;      // Reset to normal (White is default)
    }
    void Attack() {
        // 1. Determine the direction of the attack
        Vector3 attackOffset = Vector3.zero;
        float currentAttackRange = attackRange; // Start with your default range    
        // Check vertical input from the keyboard
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) {
            attackOffset = Vector3.up * 1.2f; // Attack Above
            StartCoroutine(ShowSlash(upAttackVisual));
        }
        else if ((Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) && !isGrounded) {
            attackOffset = Vector3.down * 1.6f; // Attack Below (only if in air)
            currentAttackRange = attackRange * 1.5f; // Hitbox is 50% larger for pogo!
            StartCoroutine(ShowSlash(downAttackVisual));
        }
        else {
            // Default: Attack Forward (multiplied by facing direction)
            attackOffset = new Vector3(transform.localScale.x * 1.2f, 0, 0);
            StartCoroutine(ShowSlash(frontAttackVisual));
        }

        // 2. Set the position of the attack
        Vector3 finalPos = transform.position + attackOffset;

        // 3. Detect and Damage
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(finalPos, currentAttackRange, enemyLayers);
        
        if(hitEnemies.Length > 0){
            if (attackOffset.y < 0) {
                // Reset vertical velocity and add a small upward boost
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.75f); 
            }
        }
        foreach (Collider2D enemy in hitEnemies) {
            Debug.Log("Hit " + enemy.name);
            
            // enemy.GetComponent<BossHealth>().TakeDamage(10);
        }
    }
    IEnumerator ShowSlash(GameObject attack) {
        attack.SetActive(true);
        
        // Adjust the rotation of the slash based on Up/Down/Side input here if needed
        
        yield return new WaitForSeconds(0.1f); // Show for 1/10th of a second
        attack.SetActive(false);
    }
    void FlipSprite()
    {
        if (horizontalInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
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



    

