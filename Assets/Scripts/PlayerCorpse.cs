using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCorpse : MonoBehaviour
{
    public GameObject[] limbs;
    public float XLaunchMag = 4f;
    public float YLaunchMag = 2f;
    [SerializeField] Vector2 xRange = new Vector2(-7, 7), yRange = new Vector2(3, 10);

    public void Init(Vector2 playerMag) {

        foreach (GameObject limb in limbs) {
            limb.GetComponent<Rigidbody2D>().velocity = playerMag;
            limb.GetComponent<Rigidbody2D>().velocity += new Vector2(Random.Range(xRange.x, xRange.y), Random.Range(yRange.x, yRange.y));
        }
    }
}
