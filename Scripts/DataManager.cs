using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public static void SaveWeaponData(Weapon weapon)
    {
        PlayerPrefs.SetInt(weapon.weaponName + "_" + "totalAmmo", weapon.totalAmmo);
        PlayerPrefs.SetInt(weapon.weaponName + "_" + "currentAmmoInMagazine", weapon.currentAmmoInMagazine);
        PlayerPrefs.SetFloat(weapon.weaponName + "_" + "delay", weapon.delay);
        PlayerPrefs.SetInt(weapon.weaponName + "_" + "damage", weapon.damage);
        PlayerPrefs.SetInt(weapon.weaponName + "_" + "upgradeLevel", weapon.upgradeLevel);
        PlayerPrefs.SetInt(weapon.weaponName + "_" + "isUnlocked", weapon.isUnlocked ? 1 : 0);
    }

    public static void LoadWeaponData(Weapon weapon)
    {
        if (PlayerPrefs.HasKey(weapon.weaponName + "_" + "upgradeLevel") && PlayerPrefs.HasKey(weapon.weaponName + "_" + "totalAmmo") && PlayerPrefs.HasKey(weapon.weaponName + "_" + "currentAmmoInMagazine") && PlayerPrefs.HasKey(weapon.weaponName + "_" + "delay") && PlayerPrefs.HasKey(weapon.weaponName + "_" + "damage"))
        {
            weapon.totalAmmo = PlayerPrefs.GetInt(weapon.weaponName + "_" + "totalAmmo");
            weapon.delay = PlayerPrefs.GetFloat(weapon.weaponName + "_" + "delay");
            weapon.currentAmmoInMagazine = PlayerPrefs.GetInt(weapon.weaponName + "_" + "currentAmmoInMagazine");
            weapon.damage = PlayerPrefs.GetInt(weapon.weaponName + "_" + "damage");
            weapon.upgradeLevel = PlayerPrefs.GetInt(weapon.weaponName + "_" + "upgradeLevel");
            weapon.isUnlocked = PlayerPrefs.GetInt(weapon.weaponName + "_" + "isUnlocked") != 0;
        }
    }
}
