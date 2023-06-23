using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCoord : MonoBehaviour
{
    PlayerCombat pCombat => GetComponentInParent<PlayerCombat>();

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void PunchImpact()
    {
        if (pCombat) pCombat.PunchImpact();
    }
}
