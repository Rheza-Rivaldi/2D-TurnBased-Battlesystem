using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionData : MonoBehaviour
{
    public string regionName;
    public int MaxAmountEnemies = 4;
    public List<GameObject> possibleEnemies = new List<GameObject>();
}
