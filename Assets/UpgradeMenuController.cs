using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuController : MonoBehaviour
{
    [SerializeField] GameObject menuParent;
    [SerializeField] TextMeshProUGUI newLevel, pointsGained, totalPoints;
    [SerializeField] List<UpgradeOption> weaponUpgradeOptions, chassisUpgradeOptions;
    [SerializeField] Color canAffordColor;

    [Header("Sounds")]
    [SerializeField] Sound uiButtonSound;
    

    [Header("Healing")]
    [SerializeField] float healPercent = 20;
    [SerializeField] float healCost = 1, healCostMult = 1.3f;
    [SerializeField] TextMeshProUGUI healPercentText, healCostText;
    [SerializeField] Button healButton;
    int timesHealed;
    int numberOfAffordableButtons;

    PlayerXP pXP => FindObjectOfType<PlayerXP>();
    Gun gun;

    private void Start()
    {
        pXP.OnLevelUp.AddListener(OpenMenu);
        uiButtonSound = Instantiate(uiButtonSound);
    }

    public void Heal()
    {
        numberOfAffordableButtons -= 1;
        pXP.numPoints -= Mathf.RoundToInt(healCost);
        healCost *= healCostMult;
        pXP.GetComponent<PlayerCombat>().HealPercent(healPercent);
        timesHealed += 1;

        OpenMenu();

        uiButtonSound.Play();
        if (numberOfAffordableButtons <= 0) CloseMenu();
    }

    void OpenMenu()
    {
        numberOfAffordableButtons = 0;

        menuParent.SetActive(true);
        newLevel.text = "level " + pXP.getLevel();

        totalPoints.text = pXP.numPoints.ToString();
        pointsGained.text = "+" + Mathf.RoundToInt(Mathf.Sqrt(pXP.getLevel()-1));

        gun = pXP.GetComponent<PlayerCombat>().GetCurrentGun();
        for (int i = 0; i < weaponUpgradeOptions.Count; i++) {
            gun.ConfigureOption(weaponUpgradeOptions[i], i);

            EnableIfAffordable(weaponUpgradeOptions[i]);
        }

        for (int i = 0; i < chassisUpgradeOptions.Count; i++) {
            chassisUpgradeOptions[i].UpdateWithCurrentConfig();
            EnableIfAffordable(chassisUpgradeOptions[i]);
        }

        bool canAffordHeal = Mathf.RoundToInt(healCost) <= pXP.numPoints;
        healCostText.text = Mathf.RoundToInt(healCost).ToString();
        healCostText.color = canAffordHeal ? canAffordColor : Color.red;
        healButton.enabled = canAffordHeal;
        if (canAffordHeal) numberOfAffordableButtons += 1;
    }

    public void SelectWeaponUpgrade(int i)
    {
        numberOfAffordableButtons -= 1;
        pXP.numPoints -= weaponUpgradeOptions[i].currentStats.cost;

        gun.ChoseOption(i);
        gun.ConfigureOption(weaponUpgradeOptions[i], i);

        totalPoints.text = pXP.numPoints.ToString();
        EnableIfAffordable(weaponUpgradeOptions[i]);

        OpenMenu();

        uiButtonSound.Play();
        if (numberOfAffordableButtons <= 0) CloseMenu();
    }

    public void SelectChassisUpgrade(int i)
    {

        numberOfAffordableButtons -= 1;
        float amount = chassisUpgradeOptions[i].currentStats.amount;
        switch (chassisUpgradeOptions[i].currentStats.type) {
            case UpgradeOption.config.effect.HEALTH:
                pXP.GetComponent<PlayerCombat>().IncreaseMaxHealth(amount);
                break;
            case UpgradeOption.config.effect.SPEED:
                pXP.GetComponent<PlayerController>().IncreaseSpeed(amount);
                break;
            case UpgradeOption.config.effect.JUMPS:
                pXP.GetComponent<PlayerController>().IncreaseMaxJumps(amount);
                break;
            default: break;
        }

        pXP.numPoints -= weaponUpgradeOptions[i].currentStats.cost;
        totalPoints.text = pXP.numPoints.ToString();
        chassisUpgradeOptions[i].currentStats.Chose();
        chassisUpgradeOptions[i].UpdateWithCurrentConfig();

        EnableIfAffordable(chassisUpgradeOptions[i]);
        OpenMenu();

        uiButtonSound.Play();
        if (numberOfAffordableButtons <= 0) CloseMenu();
    }

    public void CloseMenu()
    {
        uiButtonSound.Play();
        GameManager.i.Resume();
        menuParent.SetActive(false);
    }

    void EnableIfAffordable(UpgradeOption option)
    {
        bool canAfford = option.currentStats.cost <= pXP.numPoints;
        option.SetEnabled(canAfford, canAffordColor);
        if (canAfford) numberOfAffordableButtons += 1;
    }
}
