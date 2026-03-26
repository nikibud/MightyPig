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

    public bool bossFightStarted = false ;
    public BossState currentState = BossState.Idle;
    private int randomIndex;
    private BossState chosenAttack;

    public float timeBetweenMoves = 5f;
    public float timer=0;
    void Update() {
        
        if(!pigAttacks.isAttacking && bossFightStarted)
        {
            
            if (Time.time >= timer ) {
                ChangeState(GetNextAttack());
                //AttackCooldown(); // This changes state to ScatterShot/Dash
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
         //ChangeState(GetNextAttack());
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
        if(currentState != BossState.Idle && newState == BossState.Idle)
            timer = timeBetweenMoves + Time.time;
        currentState = newState;
        
    }
}
