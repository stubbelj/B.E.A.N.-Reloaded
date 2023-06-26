using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("If this is OFF, goal string will be ignored and any type of hit will activate the target")]
    bool tutorialTarget;

    [SerializeField] string goalString;
    [SerializeField] GameObject linkedObject;
    
    void Start(){}

    public void Hit(string hitString){
        if(hitString.Equals(goalString) || !tutorialTarget) {
            GameObject.Destroy(linkedObject);
            GameObject.Destroy(gameObject);
        }
    }
}