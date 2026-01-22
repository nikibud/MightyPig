using UnityEngine;

public class PlayerAnimationRelay : MonoBehaviour
{
    public controls playerControls;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerControls = GetComponentInParent<controls>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DownAttack()
    {
        playerControls.AttackHitbox(PlayerAttack.Down);
    }
    public void UpAttack()
    {
        playerControls.AttackHitbox(PlayerAttack.Up);
    }
    public void FrontAttack()
    {
        playerControls.AttackHitbox(PlayerAttack.Front);
    }
}
