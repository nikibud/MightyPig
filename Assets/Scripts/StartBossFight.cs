using UnityEngine;
using Cinemachine;
public class StartBossFight : MonoBehaviour
{
    public GameObject LeftTreeBorder;
    public CinemachineVirtualCamera vcam_Boss;
    public PigAttackPattern pigAttackPattern;
    public PigAttack pigAttack;
    public GameObject Pig;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && pigAttackPattern.bossFightStarted == false)
        {
            pigAttack.FullReset();
            Pig.SetActive(true);
            LeftTreeBorder.SetActive(true);
            pigAttackPattern.bossFightStarted=true;
            vcam_Boss.m_Priority = 20;
            
        }
    }
}
