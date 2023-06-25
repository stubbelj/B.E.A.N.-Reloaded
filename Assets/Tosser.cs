using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tosser : BaseEnemy
{
    [SerializeField] float range;

    private void OnDrawGizmosSelected()
    {
        if (!debug) return;
    }
}
