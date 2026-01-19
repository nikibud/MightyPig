using UnityEngine;

public class PigAnimationRelay : MonoBehaviour
{

    private PigAttack parentScript;
    private PigAttackPattern PigAttackPattern;
    void Start() {
        // Find the PigAttack script on the parent object
        parentScript = GetComponentInParent<PigAttack>();
        PigAttackPattern = GetComponentInParent<PigAttackPattern>();
    }

    // This is the function you select in the Animation Window
    public void FireScatterShot() {
        if (parentScript != null) {
            parentScript.FireScatterShot();
        }
    }
    public void HeadButt() {
        if (PigAttackPattern != null) {
            PigAttackPattern.ChangeState(BossState.HeadButt);
        }
    }
}
