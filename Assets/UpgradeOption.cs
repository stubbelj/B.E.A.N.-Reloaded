using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeOption : MonoBehaviour
{
    [System.Serializable]
    public class config
    {
        public enum effect {DAMAGE, FIRE_RATE, CLIP_SIZE, HEALTH, SPEED, JUMPS}
        public string title, subtitle;
        public int cost;
        public effect type;
        public float amount, amountMult, costMult;

        public void Chose()
        {
            amount *= amountMult;
            cost *= Mathf.RoundToInt(costMult);
        }
    }

    public config currentStats;
    public TextMeshProUGUI upgradeTitle, subtitle, cost;
    
    public void UpdateWithCurrentConfig()
    {
        upgradeTitle.text = currentStats.title;
        subtitle.text = currentStats.subtitle;
        cost.text = currentStats.cost.ToString();
    }

    public void SetEnabled(bool _enabled, Color canAfford)
    {
        cost.color = _enabled ? canAfford : Color.red;
        GetComponent<Button>().enabled = _enabled;
    }
}
