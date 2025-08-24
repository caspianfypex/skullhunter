using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEvents : MonoBehaviour
{

    public GameObject healthDropPrefab;
    public GameObject ammoDropPrefab;
    public GameObject armorDropPrefab;
    public Transform spawnLocs;
    public GameObject eButtonImage;
    public Transform dropsParent;
    public static DropEvents instance;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        eButtonImage.SetActive(false);
        RaycastHit hit;
        // Checks for types of drops deppending on their tags
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f))
        {
            if (hit.transform.gameObject.CompareTag("HealthBall"))
            {
                eButtonImage.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    //Sets player health to 100 if current health is more than 50, otherwise adds 50
                    if (GameManager.GetGameManager().playerHealth + 50 > 100)
                    {
                        GameManager.GetGameManager().playerHealth = 100;
                    }
                    else
                    {
                        GameManager.GetGameManager().playerHealth += 50;
                    }
                    //Removes tag to avoid reusing same drop
                    hit.transform.gameObject.tag = "Untagged";
                    StartCoroutine(DropDestroy(hit.transform.parent.gameObject));
                    Destroy(hit.transform.gameObject);
                }
            }
            else if (hit.transform.gameObject.CompareTag("AmmoBall"))
            {
                eButtonImage.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Weapon.current.totalAmmo += Weapon.current.maxAmmoPerMagazine;
                    //Removes tag to avoid reusing same drop
                    hit.transform.gameObject.tag = "Untagged";
                    StartCoroutine(DropDestroy(hit.transform.parent.gameObject));
                    Destroy(hit.transform.gameObject);
                }
            }
            else if (hit.transform.gameObject.CompareTag("ArmorBall"))
            {
                eButtonImage.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    //Sets player armor to 100 if current armor is more than 50, otherwise adds 50
                    if (GameManager.GetGameManager().playerArmor + 50 > 100)
                    {
                        GameManager.GetGameManager().playerArmor = 100;
                    }
                    else
                    {
                        GameManager.GetGameManager().playerArmor += 50;
                    }
                    //Removes tag to avoid reusing same drop
                    hit.transform.gameObject.tag = "Untagged";
                    StartCoroutine(DropDestroy(hit.transform.parent.gameObject));
                    Destroy(hit.transform.gameObject);
                }
            }
        }

    }

    public IEnumerator StartDropEvents()
    {
        while (true)
        {
            //Works only if enemies can spawn(wave is active)
            if (GameManager.GetGameManager().canEnemiesSpawn)
            {
                //All types of drops are spawned after wave 10, otherwise only health and ammo drops are spawned
                if (GameManager.GetGameManager().wave > 10)
                {
                    int i = Random.Range(1, 4);

                    if (i == 1)
                    {
                        StartCoroutine(SpawnHealthDrop());
                    }
                    else if (i == 2)
                    {
                        StartCoroutine(SpawnArmorDrop());
                    }
                    else
                    {
                        StartCoroutine(SpawnAmmoDrop());
                    }
                }
                else
                {
                    int i = Random.Range(1, 3);

                    if (i == 1)
                    {
                        StartCoroutine(SpawnHealthDrop());
                    }
                    else
                    {
                        StartCoroutine(SpawnAmmoDrop());
                    }
                }
            }
            yield return new WaitForSeconds(45f);
        }
    }

    public IEnumerator SpawnHealthDrop()
    {
        //Chooses index of location to spawn a drop
        int r = Random.Range(0, spawnLocs.childCount);
        Vector3 pos = Vector3.zero;
        while (true)
        {
            //Checks if chosen spawn loc is available(active means it is), otherwise picks another index
            if (spawnLocs.GetChild(r).gameObject.active)
            {
                pos = spawnLocs.GetChild(r).position;
                break;
            }
            else
            {
                r = Random.Range(0, spawnLocs.childCount);
            }
            yield return new WaitForSeconds(0.01f);
        }
        //Creation of drop object
        GameObject newObject = Instantiate(healthDropPrefab, new Vector3(pos.x, pos.y - 7.442666f, pos.z), Quaternion.identity);
        newObject.name = r.ToString();
        newObject.transform.parent = dropsParent;
        spawnLocs.GetChild(r).gameObject.SetActive(false);
        //Animation of appeareance of drop object
        for (int i = 0; i < 148; i++)
        {
            newObject.transform.position = new Vector3(newObject.transform.position.x, newObject.transform.position.y + 0.05f, newObject.transform.position.z);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(25f);
        //After given seconds if drop is stil not used it is destroyed automatically
        if (newObject)
        {
            StartCoroutine(DropDestroy(newObject));
        }
    }

    public IEnumerator SpawnArmorDrop()
    {
        //Chooses index of location to spawn a drop
        int r = Random.Range(0, spawnLocs.childCount);
        Vector3 pos = Vector3.zero;
        while (true)
        {
            //Checks if chosen spawn loc is available(active means it is), otherwise picks another index
            if (spawnLocs.GetChild(r).gameObject.active)
            {
                pos = spawnLocs.GetChild(r).position;
                break;
            }
            else
            {
                r = Random.Range(0, spawnLocs.childCount - 1);
            }
            yield return new WaitForSeconds(0.01f);
        }
        //Creation of drop object
        GameObject newObject = Instantiate(armorDropPrefab, new Vector3(pos.x, pos.y - 7.442666f, pos.z), Quaternion.identity);
        newObject.name = r.ToString();
        newObject.transform.parent = dropsParent;
        spawnLocs.GetChild(r).gameObject.SetActive(false);
        //Animation of appeareance of drop object
        for (int i = 0; i < 148; i++)
        {
            newObject.transform.position = new Vector3(newObject.transform.position.x, newObject.transform.position.y + 0.05f, newObject.transform.position.z);
            yield return new WaitForSeconds(0.05f);
        }
        //After given seconds if drop is stil not used it is destroyed automatically
        yield return new WaitForSeconds(25f);
        if (newObject)
        {
            StartCoroutine(DropDestroy(newObject));
        }
    }

    public IEnumerator SpawnAmmoDrop()
    {
        //Chooses index of location to spawn a drop
        int r = Random.Range(0, spawnLocs.childCount);
        Vector3 pos = Vector3.zero;
        while (true)
        {
            //Checks if chosen spawn loc is available(active means it is), otherwise picks another index
            if (spawnLocs.GetChild(r).gameObject.active)
            {
                pos = spawnLocs.GetChild(r).position;
                break;
            }
            else
            {
                r = Random.Range(0, spawnLocs.childCount - 1);
            }
            yield return new WaitForSeconds(0.01f);
        }
        //Creation of drop object
        GameObject newObject = Instantiate(ammoDropPrefab, new Vector3(pos.x, pos.y - 7.442666f, pos.z), Quaternion.identity);
        newObject.name = r.ToString();
        newObject.transform.parent = dropsParent;
        spawnLocs.GetChild(r).gameObject.SetActive(false);
        //Animation of appeareance of drop object
        for (int i = 0; i < 148; i++)
        {
            newObject.transform.position = new Vector3(newObject.transform.position.x, newObject.transform.position.y + 0.05f, newObject.transform.position.z);
            yield return new WaitForSeconds(0.05f);
        }
        //After given seconds if drop is stil not used it is destroyed automatically
        yield return new WaitForSeconds(25f);
        if (newObject)
        {
            StartCoroutine(DropDestroy(newObject));
        }
    }

    public IEnumerator DropDestroy(GameObject obj)
    {
        //Shows particle effect
        obj.transform.Find("ParticleEffect").gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        if (obj)
        {
            obj.transform.Find("ParticleEffect").gameObject.SetActive(false);
        }
        //Animation of disappereance of drop object is showed
        for (int i = 0; i < 148; i++)
        {
            if (obj)
            {
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y - 0.05f, obj.transform.position.z);
            }
            yield return new WaitForSeconds(0.05f);
        }
        //If drop was not used during animation, spawn loc of drop object becomes available and drop object self is destroyed
        if (obj)
        {
            spawnLocs.GetChild(int.Parse(obj.name)).gameObject.SetActive(true);
            Destroy(obj);
        }
    }
}
