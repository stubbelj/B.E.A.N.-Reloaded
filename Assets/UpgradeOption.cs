using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeOption : MonoBehaviour
{
    [System.Serializable]
    public class config
    {
        public enum effect {DAMAGE, FIRE_RATE, CLIP_SIZE }
        public string title, subtitle;
        public int cost;
        public effect type;
        public float amount, amountIncreasePerPurchase, costIncreasePerPurchase;

        public void Chose()
        {
            amount *= amountIncreasePerPurchase;
            cost *= Mathf.RoundToInt(costIncreasePerPurchase);
        }
    }

    public TextMeshProUGUI upgradeTitle, subtitle, cost;
}
