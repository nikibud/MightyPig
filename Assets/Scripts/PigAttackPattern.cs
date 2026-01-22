using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum BossState { Idle, ScatterShot, Charge, BounceAttack, HeadButt }

public class PigAttackPattern : MonoBehaviour
{
    public PigAttack pigAttacks;

    public List<BossState> attackPool = new List<BossState> 
    { 
        BossState.ScatterShot, 
        BossState.ScatterShot, 
        
    };
    private List<BossState> activeBag = new List<BossState>();

    
    public BossState currentState = BossState.Idle;
    private int randomIndex;
    private BossState chosenAttack;

    public float timeBetweenMoves = 120f;
    public float timer=0;
    void Update() {
        
        if(!pigAttacks.isAttacking)
        {
            
            if (Time.time >= timer ) {
                Debug.Log("the time is: " + Time.time + " and next attack is in: " + timer);
                AttackCooldown(); // This changes state to ScatterShot/Dash
                timer = Time.time + timeBetweenMoves;
            }
        
            switch (currentState) {
                case BossState.Idle:
                    break;
                case BossState.ScatterShot:
                    StartCoroutine(pigAttacks.ScatterShotAttack()); // The code you wrote earlier!
                    Debug.Log("throwing");
                    ChangeState(BossState.Idle);
                    break;

                case BossState.Charge:
                    StartCoroutine(pigAttacks.ChargeAttack());
                    Debug.Log("Charging");
                    ChangeState(BossState.Idle);
                    break;

                case BossState.BounceAttack:
                    StartCoroutine(pigAttacks.BounceAttack());
                    ChangeState(BossState.Idle);
                    break;
                case BossState.HeadButt:
                    StartCoroutine(pigAttacks.HeadButt());
                    ChangeState(BossState.Idle);
                    break;
            }
        }
        
        
        
    }

    
    public void AttackCooldown()
    {
        ChangeState(GetNextAttack());
    }
    // The active bag we pull from
    
    public BossState GetNextAttack()
    {
        // 1. If the bag is empty, refill it
        if (activeBag.Count == 0)
        {
            RefillBag();
            return BossState.Idle;
        }

        // 2. Pick a random index from the active bag
        randomIndex = Random.Range(0, activeBag.Count);
        chosenAttack = activeBag[randomIndex];

        // 3. Remove the attack so it can't be picked again immediately
        activeBag.RemoveAt(randomIndex);
        Debug.Log("the chosen attack is: " + chosenAttack);
        return chosenAttack;
    }

    void RefillBag()
    {
        // Copy everything from the Master List into the active bag
        activeBag = new List<BossState>(attackPool);
    }

    public void ChangeState(BossState newState)
    {

        currentState = newState;
        
    }
}
