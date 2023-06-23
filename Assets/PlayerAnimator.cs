using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator backArm, cockpit, frontArm;
    [SerializeField] float punchComboBreakTime = 1;
    float punchComboCooldown;

    public bool testWalking, punch;
    int punchStep;

    private void Update()
    {
        punchComboCooldown -= Time.deltaTime;

        Walk(testWalking);

        if (punch) {
            punch = false;
            Punch();
        }
    }

    public void Punch()
    {
        if (punchComboCooldown <= 0 || punchStep > 4) punchStep = 1;

        if (punchStep == 1) frontArm.SetTrigger("PUNCH1");
        if (punchStep == 2) backArm.SetTrigger("PUNCH2");
        if (punchStep == 3) frontArm.SetTrigger("PUNCH3");
        if (punchStep == 4) backArm.SetTrigger("PUNCH4");

        punchComboCooldown = punchComboBreakTime;
        punchStep += 1;
    }

    public void Walk(bool walking)
    {
        cockpit.SetBool("WALK", walking);
    }
}
