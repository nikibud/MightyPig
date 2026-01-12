using UnityEngine;

public class InMeleeRange : MonoBehaviour
{
    private PigAttack parentScript;
    public float cooldown; 
    private float canHeadButt; 
    public Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parentScript = GetComponentInParent<PigAttack>();
        canHeadButt = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if((Time.time > canHeadButt) && !parentScript.isAttacking )
            {
                anim.SetTrigger("HeadButt");
                canHeadButt = Time.time + cooldown;
            }
        }
        
        
    }

}
