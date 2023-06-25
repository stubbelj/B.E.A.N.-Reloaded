using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerXP : MonoBehaviour
{
    [SerializeField] float currentXP, nextThreshold;
    [SerializeField] int currentLevel;
    [HideInInspector] public UnityEvent OnLevelUp = new UnityEvent();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) AddXP(20);
    }

    private void Start()
    {
        currentLevel = 1;
        nextThreshold = CalculateNextThreshold();
    }

    public void AddXP(float amount)
    {
        currentXP += amount;

        if (currentXP > nextThreshold) LevelUp();
    }

    public int getLevel()
    {
        return currentLevel;
    }

    void LevelUp()
    {
        currentXP -= nextThreshold;
        currentLevel += 1;
        nextThreshold = CalculateNextThreshold();
        GameManager.i.Pause(false);
        OnLevelUp.Invoke();

        if (currentXP > nextThreshold) LevelUp();
    }

    float CalculateNextThreshold()
    {
        return Mathf.FloorToInt(Mathf.Sqrt(currentLevel)) * 100;
    }

    public float GetXPPercent()
    {
        return currentXP / nextThreshold;
    }
}
