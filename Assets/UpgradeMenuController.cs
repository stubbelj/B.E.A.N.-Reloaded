using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UpgradeMenuController : MonoBehaviour
{
    [SerializeField] GameObject menuParent;
    [SerializeField] TextMeshProUGUI newLevel, pointsGained, totalPoints;
    [SerializeField] List<UpgradeOption> weaponUpgradeOptions, chassisUpgradeOptions;
    [SerializeField] Color canAffordColor;

    PlayerXP pXP => FindObjectOfType<PlayerXP>();
    Gun gun;

    private void Start()
    {
        pXP.OnLevelUp.AddListener(OpenMenu);
    }

    void OpenMenu()
    {
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
    }

    public void SelectWeaponUpgrade(int i)
    {
        gun.ChoseOption(i);
        gun.ConfigureOption(weaponUpgradeOptions[i], i);

        pXP.numPoints -= weaponUpgradeOptions[i].currentStats.cost;
        totalPoints.text = pXP.numPoints.ToString();

        EnableIfAffordable(weaponUpgradeOptions[i]);
    }

    public void SelectChassisUpgrade(int i)
    {
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
    }

    public void CloseMenu()
    {
        GameManager.i.Resume();
        menuParent.SetActive(false);
    }

    void EnableIfAffordable(UpgradeOption option)
    {
        bool canAfford = option.currentStats.cost <= pXP.numPoints;
        option.SetEnabled(canAfford, canAffordColor);
    }
}
