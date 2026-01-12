using UnityEngine;

public class PigAnimationRelay : MonoBehaviour
{
    private PigAttack parentScript;

    void Start() {
        // Find the PigAttack script on the parent object
        parentScript = GetComponentInParent<PigAttack>();
    }

    // This is the function you select in the Animation Window
    public void FireScatterShot() {
        if (parentScript != null) {
            parentScript.FireScatterShot();
        }
    }
    public void HeadButt() {
        if (parentScript != null) {
            parentScript.StartCoroutine(parentScript.HeadButt());
            parentScript.isAttacking = true;
        }
    }
}
