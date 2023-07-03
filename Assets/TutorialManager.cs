using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager i;
    private void Awake() { i = this; }

    public List<string> tutorialState;

    public bool CheckTrigger(string trigger)
    {
        return tutorialState.Contains(trigger);
    }
}
