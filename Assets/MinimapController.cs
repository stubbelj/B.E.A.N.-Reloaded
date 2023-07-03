using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    void Start()
    {
        if (!FindObjectOfType<LevelGenerator>()) gameObject.SetActive(false);
    }

}
