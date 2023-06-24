using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCorpse : MonoBehaviour
{
    public GameObject[] limbs;
    public float XLaunchMag = 4f;
    public float YLaunchMag = 2f;

    public void Init(Vector3 playerMag) {
        foreach (GameObject limb in limbs) {
            limb.GetComponent<Rigidbody2D>().velocity = playerMag;
            limb.GetComponent<Rigidbody2D>().velocity += new Vector2(Random.Range(-5, 5), Random.Range(-3, 3));
        }
    }
}
