using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject ammoObject;
    public GameObject player;
    public Text ammoText;
    public GameObject gameOverScreen;
    private static GameManager gameManager;
    public float playerHealth;
    public float playerArmor;
    public Slider healthBar;
    public Slider armorBar;
    public bool canEnemiesSpawn = false;
    public int wave = 1;
    public int hitCrystals = 0;
    public int coin = 0;
    public GameObject shop;
    public GameObject shopMenu;
    public bool canOpenShop = false;
    public Text waveText;
    public Text waveTextOutline;
    public Text timerTextOutline;
    public Text timerText;
    public GameObject anySpawner;
    public bool isBossSpawned;
    public GameObject bossBarObject;
    public Text bossNameText;
    public Slider bossBarSlider;
    public Transform enemiesParent;
    public Transform crystalsParent;
    public GameObject gameFinishedBlackScreen;
    public GameObject victoryMenu;
    public GameObject victoryMainMenu;

    private void Awake()
    {
        //Initial data settings
        gameManager = this;
        float _sensivity = PlayerPrefs.GetFloat("sensivity");
        float _sfx = PlayerPrefs.GetFloat("sfx");
        float _music = PlayerPrefs.GetFloat("music");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("sensivity", _sensivity);
        PlayerPrefs.SetFloat("sfx", _sfx);
        PlayerPrefs.SetFloat("music", _music);
        DataManager.SaveWeaponData(shop.GetComponent<Shop>().shootScr.weapons[0].GetComponent<Weapon>());
        DataManager.SaveWeaponData(shop.GetComponent<Shop>().shootScr.weapons[1].GetComponent<Weapon>());
        DataManager.SaveWeaponData(shop.GetComponent<Shop>().shootScr.weapons[2].GetComponent<Weapon>());
        Time.timeScale = 1;
        AudioListener.volume = 1;
    }

    private void Update()
    {
        //Opens shop if it is available and given keycode is used
        if (Input.GetKeyDown(KeyCode.Tab) && canOpenShop)
        {
            shopMenu.SetActive(!shopMenu.active);
            player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = !shopMenu.active;
            player.GetComponent<Shoot>().enabled = !shopMenu.active;
            Cursor.visible = shopMenu.active;
            if (shopMenu.active)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        //If wave is inactive and it is not final wave, makes another wave begin using given keycode
        if (Input.GetKeyDown(KeyCode.End) && !canEnemiesSpawn && wave != 30)
        {
            StartCoroutine(StarWave());
            timerText.gameObject.SetActive(false);
            timerTextOutline.gameObject.SetActive(false);
        }
        //Updates ammo info, if player has one
        if (Weapon.current != null)
        {
            ammoObject.SetActive(true);
            ammoText.text = Weapon.current.currentAmmoInMagazine + "/" + Weapon.current.totalAmmo;
        }
        else
        {
            ammoObject.SetActive(false);
        }
        //Nexts wave if number of hit crystals is 5 or more
        if(hitCrystals >= 5)
        {
            hitCrystals = 0;
            StartCoroutine(NextWave());        
        }
        //Updates sensivity depending on settings
        player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().m_MouseLook.XSensitivity = PlayerPrefs.GetFloat("sensivity");
        player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().m_MouseLook.YSensitivity = PlayerPrefs.GetFloat("sensivity");
        //Updates ui values depending on real values
        armorBar.value = playerArmor;
        healthBar.value = playerHealth;
    }

    public IEnumerator NextWave()
    {
        //If it is not wave 30 enables shop and texts, starts timer for starting next wave, but if it is wave 30 it finishes game
        if (wave != 30)
        {
            canEnemiesSpawn = false;
            waveText.text = "Wave " + wave + " Has Ended\nShop Is Utilizable";
            waveTextOutline.text = "Wave " + wave + " Has Ended\nShop Is Utilizable";
            int i = 60;
            wave += 1;
            shop.SetActive(true);
            timerText.gameObject.SetActive(true);
            timerTextOutline.gameObject.SetActive(true);
            while (!canEnemiesSpawn)
            {
                timerText.text = i.ToString();
                timerTextOutline.text = i.ToString();
                if (i <= 0)
                {
                    StartCoroutine(StarWave());
                    timerText.gameObject.SetActive(false);
                    timerTextOutline.gameObject.SetActive(false);
                    break;
                }
                yield return new WaitForSeconds(1f);
                i--;
            }
        }
        else
        {
            canEnemiesSpawn = false;
            waveText.text = "Wave " + wave + " Has Ended\nEnemies Disappeared";
            waveTextOutline.text = "Wave " + wave + " Has Ended\nEnemies Disappeared";
            StartCoroutine(Skills.instance.ThanosFingerSnap());
            yield return new WaitForSeconds(5f);
            victoryMainMenu.SetActive(true);
            gameFinishedBlackScreen.SetActive(true);
            gameFinishedBlackScreen.GetComponent<Animation>().Play();
            yield return new WaitForSeconds(3.3f);
            victoryMenu.SetActive(true);
            player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
            player.GetComponent<Shoot>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public IEnumerator StarWave()
    {
        waveText.text = "Wave " + wave + " Has Started";
        waveTextOutline.text = "Wave " + wave + " Has Started";
        yield return new WaitForSeconds(3f);
        //Every given wave a new drop spawner is activated, so more drops are spawned next waves
        if (wave == 8)
        {
            StartCoroutine(DropEvents.instance.StartDropEvents());
            StartCoroutine(DropEvents.instance.StartDropEvents());
        }
        if (wave == 11)
        {
            StartCoroutine(DropEvents.instance.StartDropEvents());
        }
        if (wave == 14)
        {
            StartCoroutine(DropEvents.instance.StartDropEvents());
        }
        if (wave == 18)
        {
            StartCoroutine(DropEvents.instance.StartDropEvents());
        }
        if (wave == 22)
        {
            StartCoroutine(DropEvents.instance.StartDropEvents());
        }
        //Changes texts and other settings
        waveText.text = "Wave: " + wave;
        waveTextOutline.text = "Wave: " + wave;
        canOpenShop = false;
        canEnemiesSpawn = true;
        //Closes shop if it is active
        if (shopMenu.active)
        {
            shopMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            player.GetComponent<Shoot>().enabled = true;
            player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
        }
        //Spawns boss depending on wave(if boss spawned it must be killed for crystals being spawned)
        if (wave == 15)
        {
            BossSpawn();
        }
        else if (wave == 23)
        {
            BossSpawn();
        }
        else if (wave == 30)
        {
            BossSpawn();
        }
        else
        {
            CrystalSpawner.instance.spawnCrystals();
        }
    }


    public static GameManager GetGameManager()
    {
        return gameManager;
    }

    public void DamagePlayer(float damage)
    {
        if(0 < playerArmor)
        {
            if(damage > playerArmor)
            {
                damage -= playerArmor;
                playerArmor = 0;
            }
            else if(damage < playerArmor)
            {
                playerArmor -= damage;
                damage = 0;
            }
            else
            {
                playerArmor = 0;
                damage = 0;
            }
        }
        playerHealth -= damage;
        CheckPlayerHealth();
    }

    public void CheckPlayerHealth()
    {
        if(playerHealth <= 0f)
        {
            //Enables game over screen and changes settings if player's health equals or less than 0
            gameOverScreen.SetActive(true);
            AudioListener.volume = 0;
            Time.timeScale = 0.0f;
            player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
            player.GetComponent<Shoot>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void BossSpawn()
    {
        //Spawns boss and sets its values based on wave
        if (wave == 15)
        {
            isBossSpawned = true;
            bossNameText.text = "Skull King";
            int bossHealth = anySpawner.GetComponent<Spawner>().skullBossPrefab.GetComponent<Enemy>().health;
            bossBarSlider.maxValue = bossHealth;
            bossBarSlider.value = bossHealth;
            bossBarObject.SetActive(true);
            anySpawner.GetComponent<Spawner>().SpawnBoss(anySpawner.GetComponent<Spawner>().skullBossPrefab);
        }
        if (wave == 23)
        {
            isBossSpawned = true;
            bossNameText.text = "Skibidi Helicopter";
            int bossHealth = anySpawner.GetComponent<Spawner>().skibidiBossPrefab.GetComponent<Enemy>().health;
            bossBarSlider.maxValue = bossHealth;
            bossBarSlider.value = bossHealth;
            bossBarObject.SetActive(true);
            anySpawner.GetComponent<Spawner>().SpawnBoss(anySpawner.GetComponent<Spawner>().skibidiBossPrefab);
        }
        if (wave == 30)
        {
            isBossSpawned = true;
            bossNameText.text = "Skull Lord";
            int bossHealth = anySpawner.GetComponent<Spawner>().skullDestroyerPrefab.GetComponent<Enemy>().health;
            bossBarSlider.maxValue = bossHealth;
            bossBarSlider.value = bossHealth;
            bossBarObject.SetActive(true);
            anySpawner.GetComponent<Spawner>().SpawnBoss(anySpawner.GetComponent<Spawner>().skullDestroyerPrefab);
        }
    }

}
