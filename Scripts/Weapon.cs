using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public float delay;
    public int maxAmmoPerMagazine;
    public int currentAmmoInMagazine;
    public int totalAmmo;
    public AudioClip shootSound;
    public int magazineCost;
    public int damage;
    public int upgradeCost;
    public int upgradeLevel = 1;
    public int maxUpgradeLevel;
    public int buyCost;
    public bool isUnlocked;
    public Sprite icon;


    public static Weapon current;

}
