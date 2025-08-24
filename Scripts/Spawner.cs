using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject[] prefabs;
    public Transform enemiesParent;
    public AudioSource audioSrc;
    public GameObject skullBossPrefab;
    public GameObject skullBoss;
    public GameObject skibidiBossPrefab;
    public GameObject skibidiBoss;
    public GameObject skullDestroyerPrefab;
    public GameObject skullDestroyer;


    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }


    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(0.01f);
        while (true)
        {
            if (enemiesParent.childCount < 13 && GameManager.GetGameManager().canEnemiesSpawn && GameManager.GetGameManager().wave < 31)
            {
                //Spawns random enemies with different types based on wave
                if (GameManager.GetGameManager().wave <= 2)
                {
                    GameObject newEnemy = Instantiate(prefabs[0], transform.position, Quaternion.identity, enemiesParent);
                }
                else if (GameManager.GetGameManager().wave > 26)
                {
                    GameObject newEnemy = Instantiate(prefabs[Random.Range(3, 11)], transform.position, Quaternion.identity, enemiesParent);
                }
                else if (GameManager.GetGameManager().wave > 24)
                {
                    GameObject newEnemy = Instantiate(prefabs[Random.Range(2, 10)], transform.position, Quaternion.identity, enemiesParent);
                }
                else if (GameManager.GetGameManager().wave > 19)
                {
                    GameObject newEnemy = Instantiate(prefabs[Random.Range(1, 9)], transform.position, Quaternion.identity, enemiesParent);
                }
                else if (GameManager.GetGameManager().wave > 17)
                {
                    GameObject newEnemy = Instantiate(prefabs[Random.Range(0, 8)], transform.position, Quaternion.identity, enemiesParent);
                }
                else if (GameManager.GetGameManager().wave > 13)
                {
                    GameObject newEnemy = Instantiate(prefabs[Random.Range(0, 7)], transform.position, Quaternion.identity, enemiesParent);
                }
                else if (GameManager.GetGameManager().wave > 11)
                {
                    GameObject newEnemy = Instantiate(prefabs[Random.Range(0, 6)], transform.position, Quaternion.identity, enemiesParent);
                }
                else if (GameManager.GetGameManager().wave > 9)
                {
                    GameObject newEnemy = Instantiate(prefabs[Random.Range(0, 5)], transform.position, Quaternion.identity, enemiesParent);
                }
                else if (GameManager.GetGameManager().wave > 7)
                {
                    GameObject newEnemy = Instantiate(prefabs[Random.Range(0, 4)], transform.position, Quaternion.identity, enemiesParent);
                }
                else if (GameManager.GetGameManager().wave > 5)
                {
                    GameObject newEnemy = Instantiate(prefabs[Random.Range(0, 3)], transform.position, Quaternion.identity, enemiesParent);
                }
                else if (GameManager.GetGameManager().wave > 2)
                {
                    GameObject newEnemy = Instantiate(prefabs[Random.Range(0, 2)], transform.position, Quaternion.identity, enemiesParent);
                }
            }
            //Spawner's delay changes based on wave
            if (GameManager.GetGameManager().wave <= 17)
            {
                yield return new WaitForSeconds(Random.Range(20f - GameManager.GetGameManager().wave, 24f - GameManager.GetGameManager().wave));
            }
            else if (GameManager.GetGameManager().wave <= 20)
            {
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(3f, 4f));
            }
        }
    }

    public void SpawnBoss(GameObject boss)
    {
        if (boss == skullBossPrefab)
        {
            skullBoss = Instantiate(boss, transform.position, Quaternion.identity, enemiesParent);
        }
        else if(boss == skibidiBossPrefab)
        {
            skibidiBoss = Instantiate(boss, transform.position, Quaternion.identity, enemiesParent);
        }
        else if (boss == skullDestroyerPrefab)
        {
            skullDestroyer = Instantiate(boss, transform.position, Quaternion.identity, enemiesParent);
        }
        audioSrc.Play();
    }

}
