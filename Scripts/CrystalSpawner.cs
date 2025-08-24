using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSpawner : MonoBehaviour
{

    public GameObject crystalPrefab;
    public Transform crystalLocs;
    public static CrystalSpawner instance;
    public Transform crystalsParent;

    private void Awake()
    {
        spawnCrystals();
        instance = this;
    }

    public void spawnCrystals()
    {
        List<Transform> spawnLocs = new List<Transform>();
        for(int i = 0; i < crystalLocs.childCount - 1; i++)
        {
            spawnLocs.Add(crystalLocs.GetChild(i));
        }
        for (int i = 0; i < 5; i++)
        {
            int index = Random.Range(0, spawnLocs.Count - 1);
            GameObject newCrystal = Instantiate(crystalPrefab, spawnLocs[index].position, Quaternion.identity);
            newCrystal.transform.parent = crystalsParent;
            spawnLocs.Remove(spawnLocs[index]);
        }
    }

}
