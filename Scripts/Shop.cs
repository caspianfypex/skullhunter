using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{

    public Shoot shootScr;
    public static Shop instance;
    public GameObject trigger;
    public Text coinText;
    public Text pistolUpPrice;
    public Text shotgunUpPrice;
    public Text ak47UpPrice;
    public int healthPrice;
    public int armorPrice;
    public GameObject ak47Buy;
    public GameObject ak47ButtonsAfterBuy;


    void Start()
    {
        instance = this;
    }

    private void Update()
    {
        coinText.text = GameManager.GetGameManager().coin.ToString();
        //Defines prices for upgrading weapons, if current weapon matches with compared weapon, data is driven from current weapon's data self(from its Weapon script), otherwise if it does not match, data is driven from saved upgrade levels
        if (Weapon.current.weaponName == shootScr.weapons[0].GetComponent<Weapon>().weaponName)
        {
            pistolUpPrice.text = (Weapon.current.upgradeCost * Weapon.current.upgradeLevel).ToString();
        }
        else
        {
            pistolUpPrice.text = (shootScr.weapons[0].GetComponent<Weapon>().upgradeCost * PlayerPrefs.GetInt("Pistol_upgradeLevel")).ToString();
        }
        if(Weapon.current.weaponName == shootScr.weapons[1].GetComponent<Weapon>().weaponName)
        {
            shotgunUpPrice.text = (Weapon.current.upgradeCost * Weapon.current.upgradeLevel).ToString();
        }
        else
        {
            shotgunUpPrice.text = (shootScr.weapons[1].GetComponent<Weapon>().upgradeCost * PlayerPrefs.GetInt("Shotgun_upgradeLevel")).ToString();
        }
        if(Weapon.current.weaponName == shootScr.weapons[2].GetComponent<Weapon>().weaponName)
        {
            ak47UpPrice.text = (Weapon.current.upgradeCost * Weapon.current.upgradeLevel).ToString();
        }
        else
        {
            ak47UpPrice.text = (shootScr.weapons[2].GetComponent<Weapon>().upgradeCost * PlayerPrefs.GetInt("AK47_upgradeLevel")).ToString();
        }
    }

    public void BuyHealth()
    {
        //Checks if player has enough money and its health does not equal 100
        if (GameManager.GetGameManager().coin >= healthPrice && GameManager.GetGameManager().playerHealth < 100)
        {
            GameManager.GetGameManager().coin -= healthPrice;
            if (GameManager.GetGameManager().playerHealth + 50 > 100)
            {
                GameManager.GetGameManager().playerHealth = 100;
            }
            else
            {
                GameManager.GetGameManager().playerHealth += 50;
            }
        }
    }

    public void BuyArmor()
    {
        //Checks if player has enough money and its armor does not equal 100
        if (GameManager.GetGameManager().coin >= armorPrice && GameManager.GetGameManager().playerArmor < 100)
        {
            GameManager.GetGameManager().coin -= armorPrice;
            if (GameManager.GetGameManager().playerArmor + 50 > 100)
            {
                GameManager.GetGameManager().playerArmor = 100;
            }
            else
            {
                GameManager.GetGameManager().playerArmor += 50;
            }
        }
    }

    public void BuyWeapon(int weaponIndex)
    {
        //Gets paramaters of bought weapon from weapon prefabs included in shootScr
        Weapon weapon = shootScr.weapons[weaponIndex].GetComponent<Weapon>();
        if (GameManager.GetGameManager().coin >= weapon.buyCost)
        {
            GameManager.GetGameManager().coin -= weapon.buyCost;
            //Disable buy button of ak47, as only ak47 is available as purchasable, code can be changed if there are more weapons
            ak47Buy.SetActive(false);
            ak47ButtonsAfterBuy.SetActive(true);
            PlayerPrefs.SetInt(weapon.weaponName + "_" + "isUnlocked", 1);
        }
    }

    public void BuyAmmo(int weaponIndex)
    {
        //Gets paramaters of bought weapon from weapon prefabs included in shootScr
        Weapon weapon = shootScr.weapons[weaponIndex].GetComponent<Weapon>();
        Weapon newWeapon = new Weapon();
        if (GameManager.GetGameManager().coin >= weapon.magazineCost)
        {
            GameManager.GetGameManager().coin -= weapon.magazineCost;
            //Checks if weapon's name matches with current weapon's, otherwise different code is applied
            if (weapon.weaponName == Weapon.current.weaponName)
            {
                Weapon.current.totalAmmo += weapon.maxAmmoPerMagazine;
            }
            else
            {
                //Sets new weapon object's name to weapon's name with given index
                //It is important to create new weapon object, but not using weapon self, because weapon which is got from shootScr.weapons is prefab, what means its
                //paramaters(totalAmmo, currentAmmo etc.) are completely new just like it was first time entering the game, so instead new weapon object is created which copies
                //name of given weapon and copies other paramaters from saved data(PlayerPrefs)
                newWeapon.weaponName = weapon.weaponName;
                newWeapon.currentAmmoInMagazine = PlayerPrefs.GetInt(newWeapon.weaponName + "_" + "currentAmmoInMagazine");
                newWeapon.totalAmmo = PlayerPrefs.GetInt(newWeapon.weaponName + "_" + "totalAmmo");
                newWeapon.damage = PlayerPrefs.GetInt(newWeapon.weaponName + "_" + "damage");
                newWeapon.delay = PlayerPrefs.GetFloat(newWeapon.weaponName + "_" + "delay");
                newWeapon.upgradeLevel = PlayerPrefs.GetInt(newWeapon.weaponName + "_" + "upgradeLevel");
                newWeapon.isUnlocked = PlayerPrefs.GetInt(weapon.weaponName + "_" + "isUnlocked") != 0;
                newWeapon.totalAmmo += weapon.maxAmmoPerMagazine;
                DataManager.SaveWeaponData(newWeapon);
            }
        }
    }

    public void UpgradeWeapon(int weaponIndex)
    {
        Weapon weapon = shootScr.weapons[weaponIndex].GetComponent<Weapon>();
        Weapon newWeapon = new Weapon();
        if (GameManager.GetGameManager().coin >= weapon.upgradeCost * weapon.upgradeLevel)
        {
            //Checks if given weapon matches with current weapon, otherwise different code is applied
            if (weapon.weaponName == Weapon.current.weaponName)
            {
                //Checks if upgrade level is not max
                if (Weapon.current.upgradeLevel < weapon.maxUpgradeLevel)
                {
                    GameManager.GetGameManager().coin -= weapon.upgradeCost * weapon.upgradeLevel;
                    //Paramaters of weapon changed based on upgrade level
                    Weapon.current.damage += weapon.upgradeLevel * 4;
                    Weapon.current.delay -= weapon.upgradeLevel * 0.015f;
                    Weapon.current.upgradeLevel += 1;
                }
            }
            else
            {
                if (PlayerPrefs.GetInt(weapon.weaponName + "_" + "upgradeLevel") < weapon.maxUpgradeLevel)
                {
                    GameManager.GetGameManager().coin -= weapon.upgradeCost * weapon.upgradeLevel;
                    //Sets new weapon object's name to weapon's name with given index
                    //It is important to create new weapon object, but not using weapon self, because weapon which is got from shootScr.weapons is prefab, what means its
                    //paramaters(totalAmmo, currentAmmo etc.) are completely new just like it was first time entering the game, so instead new weapon object is created which copies
                    //name of given weapon and copies other paramaters from saved data(PlayerPrefs)
                    newWeapon.weaponName = weapon.weaponName;
                    newWeapon.currentAmmoInMagazine = PlayerPrefs.GetInt(newWeapon.weaponName + "_" + "currentAmmoInMagazine");
                    newWeapon.totalAmmo = PlayerPrefs.GetInt(newWeapon.weaponName + "_" + "totalAmmo");
                    newWeapon.damage = PlayerPrefs.GetInt(newWeapon.weaponName + "_" + "damage");
                    newWeapon.delay = PlayerPrefs.GetFloat(newWeapon.weaponName + "_" + "delay");
                    newWeapon.upgradeLevel = PlayerPrefs.GetInt(newWeapon.weaponName + "_" + "upgradeLevel");
                    newWeapon.isUnlocked = PlayerPrefs.GetInt(weapon.weaponName + "_" + "isUnlocked") != 0;
                    newWeapon.damage += newWeapon.upgradeLevel * 4;
                    newWeapon.delay -= newWeapon.upgradeLevel * 0.015f;
                    newWeapon.upgradeLevel += 1;
                    DataManager.SaveWeaponData(newWeapon);
                }
            }
        }
    }
}
