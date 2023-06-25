using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager i;

    private void Awake(){ i = this; }

    [SerializeField] List<Transform> tossers = new List<Transform>();

    public void AddTosser(Transform newTosser)
    {
        tossers.Add(newTosser);
    }

    public Transform GetClosestTosser(Vector2 pos, float maxDist)
    {
        float closest = Mathf.Infinity;
        Transform best = null;
        for (int i = 0; i < tossers.Count; i++) {
            if (tossers[i] == null) {
                tossers.RemoveAt(i);
                i -= 1;
            }
            else if (!tossers[i].GetComponent<Tosser>().HasBomber()) {
                var dist = Vector2.Distance(tossers[i].position, pos);
                if (dist >= closest || dist > maxDist) continue;
                closest = dist;
                best = tossers[i];
            }
        }
        return best;
    }
}
