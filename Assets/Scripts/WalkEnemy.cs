using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkEnemy : BaseEnemy
{
    [SerializeField] float range;

    protected override void Update()
    {
        base.Update();

        if (stunTime > 0) return;

        if (dist > range) WalkTowardPlayer();
        else Stop();
    }
}
