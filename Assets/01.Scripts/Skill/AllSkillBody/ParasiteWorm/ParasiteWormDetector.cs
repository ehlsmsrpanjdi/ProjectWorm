using System.Collections.Generic;
using UnityEngine;

public class ParasiteWormDetector : MonoBehaviour
{
    public List<Edible> nearbyEdibles = new List<Edible>();

    private void OnTriggerEnter2D(Collider2D col)
    {
        Edible e = col.GetComponent<Edible>();
        if (e != null && !nearbyEdibles.Contains(e))
        {
            nearbyEdibles.Add(e);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Edible e = col.GetComponent<Edible>();
        if (e != null && nearbyEdibles.Contains(e))
        {
            nearbyEdibles.Remove(e);
        }
    }

    public Edible GetNearestEdible(Transform self)
    {
        Edible nearest = null;
        float minDist = float.MaxValue;

        foreach (var e in nearbyEdibles)
        {
            if (e == null || e.IsDead()) continue;

            float d = Vector2.Distance(self.position, e.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = e;
            }
        }
        return nearest;
    }
}
