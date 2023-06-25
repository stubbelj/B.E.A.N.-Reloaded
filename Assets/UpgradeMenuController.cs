using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeMenuController : MonoBehaviour
{
    [SerializeField] GameObject menuParent;
    [SerializeField] TextMeshProUGUI newLevel, pointsGained;
    [SerializeField] List<UpgradeOption> weaponUpgradeOptions, chassisUpgradeOptions;
    
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

        gun = pXP.GetComponent<PlayerCombat>().GetCurrentGun();
        for (int i = 0; i < weaponUpgradeOptions.Count; i++) {
            gun.ConfigureOption(weaponUpgradeOptions[i], i);
        }
    }

    public void SelectWeaponUpgrade(int i)
    {
        gun.ChoseOption(i);
        gun.ConfigureOption(weaponUpgradeOptions[i], i);
    }
}
