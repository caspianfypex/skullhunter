using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour
{

    public GameObject[] weapons;
    public GameObject weaponPoint;
    public ParticleSystem muzzleFlash;
    Rigidbody rb;
    public bool canShoot = true;
    float velocity;
    float mass = 3f;
    public float shotgunImpact = 3f;
    Vector3 impact = Vector3.zero;
    private CharacterController characterController;
    public Image gunIcon;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        canShoot = true;
        SwitchWeapon(weapons[0]);
    }

    void Update()
    {
        StartCoroutine(ShootFunc());
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(weapons[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(weapons[1]);
        }
        //Firstly checks if weapon is bought
        if (Input.GetKeyDown(KeyCode.Alpha3) && PlayerPrefs.GetInt(weapons[2].GetComponent<Weapon>().weaponName + "_" + "isUnlocked") != 0)
        {
            SwitchWeapon(weapons[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SwitchWeapon(null);
        }

        if (impact.magnitude > 0.2) characterController.Move(impact * Time.deltaTime);
        impact = Vector3.Lerp(impact, Vector3.zero, shotgunImpact * Time.deltaTime);
    }


    IEnumerator ShootFunc()
    {
        //Checks if player can shoot through given paramaters and if it is AK47 or not(Difference between holding and pressing button)
        if (Input.GetMouseButtonDown(0) && canShoot && Weapon.current != null && !Weapon.current.weaponName.Equals("AK47") && Weapon.current.totalAmmo + Weapon.current.currentAmmoInMagazine != 0 && !Weapon.current.gameObject.GetComponent<Animation>().isPlaying)
        {
            Weapon.current.currentAmmoInMagazine -= 1;
            if (Weapon.current.GetComponent<Animation>())
            {
                Weapon.current.GetComponent<Animation>().Play();
            }
            Weapon.current.transform.Find("FirePoint").transform.Find("MuzzleFlash").GetComponent<ParticleSystem>().Play();
            Weapon.current.gameObject.GetComponent<AudioSource>().PlayOneShot(Weapon.current.shootSound);
            CameraShake.ShakeOnce(0.2f, 4.5f);
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 50f))
            {
                //If player's current weapons is shotgun and player looks down, it makes player jump higher
                if (Weapon.current.gameObject.name.Contains("Shotgun") && Camera.main.transform.localRotation.x >= 0.15f)
                {
                    AddImpact(transform.TransformDirection(Vector3.up + Vector3.back), 120f);
                }
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    GameObject enemy = hit.collider.gameObject;
                    enemy.transform.parent.GetComponent<Enemy>().health -= Weapon.current.damage;
                    if (enemy.transform.parent.GetComponent<Enemy>().health <= 0)
                    {
                        //Disables audio of enemy after its death
                        if (enemy.transform.parent.GetComponent<AudioSource>())
                        {
                            enemy.transform.parent.GetComponent<AudioSource>().enabled = false;
                        }
                        //Spawns crystalls and changes paramaters if killed enemy is boss
                        if (enemy.transform.parent.GetComponent<Enemy>().isBoss)
                        {
                            CrystalSpawner.instance.spawnCrystals();
                            GameManager.GetGameManager().isBossSpawned = false;
                            GameManager.GetGameManager().bossBarObject.SetActive(false);
                        }
                        //Disables enemy script
                        enemy.transform.parent.gameObject.GetComponent<Enemy>().enabled = false;
                        int c = enemy.transform.parent.childCount;
                        //Disables visibility of all meshes and colliders of enemy object's parent
                        for (int i = 0; i < c; i++)
                        {
                            if (enemy.transform.parent.GetChild(i).GetComponent<MeshCollider>() || enemy.transform.parent.GetChild(i).GetComponent<BoxCollider>())
                            {
                                enemy.transform.parent.GetChild(i).gameObject.SetActive(false);
                            }
                        }
                        //Disables child object for animations in enemy object's parent
                        if (enemy.transform.parent.Find("AnimModel"))
                        {
                            enemy.transform.parent.Find("AnimModel").gameObject.SetActive(false);
                        }
                        //Enables particle effect of death of enemy object
                        enemy.transform.parent.Find("ParticleEffect").gameObject.SetActive(true);
                        Destroy(enemy.transform.parent.gameObject, 2);
                        if (GameManager.GetGameManager().wave < 6)
                        {
                            GameManager.GetGameManager().coin += 1 * GameManager.GetGameManager().wave;
                        }
                        else
                        {
                            GameManager.GetGameManager().coin += 6;
                        }
                    }
                    else
                    {
                        //Changes boss bar slider's value to new health of enemy
                        if (enemy.transform.parent.GetComponent<Enemy>().isBoss)
                        {
                            GameManager.GetGameManager().bossBarSlider.value = enemy.transform.parent.GetComponent<Enemy>().health;
                        }
                        //Plays animation if enemy has, otherwise plays default animation
                        if (enemy.GetComponent<Animation>())
                        {
                            enemy.GetComponent<Animation>().Play();
                        }
                        else
                        {
                            enemy.transform.parent.Find("AnimModel").GetComponent<Animation>().Play();
                        }
                    }
                }
                else if (hit.collider.gameObject.CompareTag("Crystal"))
                {
                    //Add 1 to hit crystals, shows particle effect and destroyed
                    GameManager.GetGameManager().hitCrystals += 1;
                    hit.transform.GetComponent<MeshCollider>().enabled = false;
                    hit.transform.GetComponent<MeshRenderer>().enabled = false;
                    hit.transform.Find("ParticleEffect").gameObject.SetActive(true);
                    Destroy(hit.transform.gameObject, 2);
                }
            }
            if (Weapon.current.currentAmmoInMagazine <= 0)
            {
                StartCoroutine(ReloadWeapon());
            }
            else
            {
                //Starts cooldown for shooting based on weapons' specific delay
                canShoot = false;
                yield return new WaitForSeconds(Weapon.current.delay);
                canShoot = true;
            }

        }
        //Checks if it is available to shoot, current weapon is ak47 and mouse button is hold
        else if (Input.GetMouseButton(0) && canShoot && Weapon.current != null && Weapon.current.weaponName.Equals("AK47") && Weapon.current.totalAmmo + Weapon.current.currentAmmoInMagazine != 0 && !Weapon.current.gameObject.GetComponent<Animation>().isPlaying)
        {
            Weapon.current.currentAmmoInMagazine -= 1;
            if (Weapon.current.GetComponent<Animation>())
            {
                Weapon.current.GetComponent<Animation>().Play();
            }
            Weapon.current.transform.Find("FirePoint").transform.Find("MuzzleFlash").GetComponent<ParticleSystem>().Play();
            Weapon.current.gameObject.GetComponent<AudioSource>().PlayOneShot(Weapon.current.shootSound);
            CameraShake.ShakeOnce(0.2f, 4.5f);
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 50f))
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    GameObject enemy = hit.collider.gameObject;
                    enemy.transform.parent.GetComponent<Enemy>().health -= Weapon.current.damage;
                    if (enemy.transform.parent.GetComponent<Enemy>().health <= 0)
                    {
                        //Disables audio of enemy after its death
                        if (enemy.transform.parent.GetComponent<AudioSource>())
                        {
                            enemy.transform.parent.GetComponent<AudioSource>().enabled = false;
                        }
                        //Spawns crystalls and changes paramaters if killed enemy is boss
                        if (enemy.transform.parent.GetComponent<Enemy>().isBoss)
                        {
                            CrystalSpawner.instance.spawnCrystals();
                            GameManager.GetGameManager().isBossSpawned = false;
                            GameManager.GetGameManager().bossBarObject.SetActive(false);
                        }
                        //Disables enemy script
                        enemy.transform.parent.gameObject.GetComponent<Enemy>().enabled = false;
                        int c = enemy.transform.parent.childCount;
                        //Disables visibility of all meshes and colliders of enemy object's parent
                        for (int i = 0; i < c; i++)
                        {
                            if (enemy.transform.parent.GetChild(i).GetComponent<MeshCollider>() || enemy.transform.parent.GetChild(i).GetComponent<BoxCollider>())
                            {
                                enemy.transform.parent.GetChild(i).gameObject.SetActive(false);
                            }
                        }
                        //Disables child object for animations in enemy object's parent
                        if (enemy.transform.parent.Find("AnimModel"))
                        {
                            enemy.transform.parent.Find("AnimModel").gameObject.SetActive(false);
                        }
                        //Enables particle effect of death of enemy object
                        enemy.transform.parent.Find("ParticleEffect").gameObject.SetActive(true);
                        Destroy(enemy.transform.parent.gameObject, 2);
                        if (GameManager.GetGameManager().wave < 6)
                        {
                            GameManager.GetGameManager().coin += 1 * GameManager.GetGameManager().wave;
                        }
                        else
                        {
                            GameManager.GetGameManager().coin += 6;
                        }
                    }
                    else
                    {
                        //Changes boss bar slider's value to new health of enemy
                        if (enemy.transform.parent.GetComponent<Enemy>().isBoss)
                        {
                            GameManager.GetGameManager().bossBarSlider.value = enemy.transform.parent.GetComponent<Enemy>().health;
                        }
                        //Plays animation if enemy has, otherwise plays default animation
                        if (enemy.GetComponent<Animation>())
                        {
                            enemy.GetComponent<Animation>().Play();
                        }
                        else
                        {
                            enemy.transform.parent.Find("AnimModel").GetComponent<Animation>().Play();
                        }
                    }
                }
                else if (hit.collider.gameObject.CompareTag("Crystal"))
                {
                    //Add 1 to hit crystals, shows particle effect and destroyed
                    GameManager.GetGameManager().hitCrystals += 1;
                    hit.transform.GetComponent<MeshCollider>().enabled = false;
                    hit.transform.GetComponent<MeshRenderer>().enabled = false;
                    hit.transform.Find("ParticleEffect").gameObject.SetActive(true);
                    Destroy(hit.transform.gameObject, 2);
                }
            }
            if (Weapon.current.currentAmmoInMagazine <= 0)
            {
                StartCoroutine(ReloadWeapon());
            }
            else
            {
                //Starts cooldown for shooting based on weapons' specific delay
                canShoot = false;
                yield return new WaitForSeconds(Weapon.current.delay);
                canShoot = true;
            }

        }
    }


    void AddImpact(Vector3 dir, float force)
    {
        //Adds impact according to given direction and force
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y;
        impact += dir.normalized * force / mass;
    }

    private void SwitchWeapon(GameObject weaponPrefab)
    {
        //Stops all actions made with previous weapon
        StopAllCoroutines();
        //If previous weapon is not None, shooting becomes active, weapon data is saved, and previous weapon is destroyed
        if (Weapon.current != null)
        {
            canShoot = true;
            DataManager.SaveWeaponData(Weapon.current);
            Destroy(Weapon.current.gameObject);
        }
        //Changes weapon, weapond data and ui image of current weapon, if new selected weapon is not None
        if (weaponPrefab != null)
        {
            GameObject newWeapon = Instantiate(weaponPrefab, weaponPoint.transform);
            Weapon.current = newWeapon.GetComponent<Weapon>();
            DataManager.LoadWeaponData(Weapon.current);
            gunIcon.sprite = Weapon.current.icon;
        }
        else
        {
            Weapon.current = null;
        }
    }

    private IEnumerator ReloadWeapon()
    {
        //Checks if total is ammo is not zero and ammo in magazine is not max
        if (Weapon.current.totalAmmo != 0 && Weapon.current.currentAmmoInMagazine != Weapon.current.maxAmmoPerMagazine)
        {
            string _weapon;
            canShoot = false;
            //Plays reload animation based on current weapon
            if (Weapon.current.weaponName.Contains("Shotgun"))
            {
                Weapon.current.gameObject.GetComponent<Animation>().Stop();
                Weapon.current.gameObject.GetComponent<Animation>().Play("shotgunReload");
                _weapon = "shotgun";
            }
            else if (Weapon.current.weaponName.Contains("AK47"))
            {
                Weapon.current.gameObject.GetComponent<Animation>().Stop();
                Weapon.current.gameObject.GetComponent<Animation>().Play("ak47Reload");
                _weapon = "ak47";
            }
            else
            {
                Weapon.current.gameObject.GetComponent<Animation>().Stop();
                Weapon.current.gameObject.GetComponent<Animation>().Play("pistolReload");
                _weapon = "pistol";
            }
            //Adds current ammo to total ammo data, so no ammo is lost
            Weapon.current.totalAmmo += Weapon.current.currentAmmoInMagazine;
            //Checks if total ammo is more than max ammo per magazine
            if (Weapon.current.totalAmmo >= Weapon.current.maxAmmoPerMagazine)
            {
                //Sets current ammo to max ammo per magazine and reduces total ammo
                Weapon.current.currentAmmoInMagazine = Weapon.current.maxAmmoPerMagazine;
                Weapon.current.totalAmmo -= Weapon.current.maxAmmoPerMagazine;
            }
            else
            {
                //Sets current ammo to total ammo, as total ammo is less than max ammo per magazine
                Weapon.current.currentAmmoInMagazine = Weapon.current.totalAmmo;
                Weapon.current.totalAmmo = 0;
            }
            //Waits based on current weapon's animation delay
            yield return new WaitForSeconds(Weapon.current.GetComponent<Animation>().GetClip(_weapon + "Reload").averageDuration);
            canShoot = true;
        }
    }

}
