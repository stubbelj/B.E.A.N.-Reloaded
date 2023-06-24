using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCoord : MonoBehaviour
{
    PlayerCombat pCombat => GetComponentInParent<PlayerCombat>();
    PlayerAnimator pAnim => GetComponentInParent<PlayerAnimator>();
    PlayerSound pSound => GetComponentInParent<PlayerSound>();
    BaseEnemy enemy => GetComponentInParent<BaseEnemy>();

    public void EndAttack()
    {
        if (enemy) enemy.EndAttack();
        if (pCombat) pCombat.EndAttack();
    }

    public void BigPunchCameraShake()
    {
        CameraShake.i.Shake(0.2f, 0.2f);
    }

    public void PlayFootStep()
    {
        if (pSound) pSound.footStep.Play();
    }

    public void StartAttack()
    {
        if (enemy) enemy.StartAttck();
        if (pCombat) pCombat.Punch3StartHit();
    }

    public void EndPunch()
    {
        if (pCombat) pCombat.EndPunch();
    }

    public void ShowArms()
    {
        if (pAnim) pAnim.ShowArms();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
