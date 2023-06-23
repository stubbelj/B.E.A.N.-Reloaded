using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCoord : MonoBehaviour
{
    PlayerCombat pCombat => GetComponentInParent<PlayerCombat>();
    PlayerAnimator pAnim => GetComponentInParent<PlayerAnimator>();

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void EndPunch()
    {
        if (pCombat) pCombat.EndPunch();
    }

    public void ShowArms()
    {
        if (pAnim) pAnim.ShowArms();
    }
}
