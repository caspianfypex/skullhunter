using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public float moveSpeed = 10.0f;
    public GameObject playerObject;
    public AudioClip[] damageSounds;
    public bool canDamage = true;
    public float defaultDamage;
    private float damage;
    public int health = 100;
    public bool isBoss;

    private void Start()
    {
        //Increase of damage depending on wave
        if (GameManager.GetGameManager().wave < 16)
        {
            damage = GameManager.GetGameManager().wave * defaultDamage * 0.55f;
        }
        else
        {
            damage = 15 * defaultDamage * 0.55f;
        }

        playerObject = GameObject.Find("Player");
    }

    void Update()
    {
        //Depending on type of enemy(boss or not), logic of enemy is chosen
        if (!isBoss)
        {
            SkeletonHead();
        }
        else
        {
            SkeletonBoss();
        }
    }


    private void SkeletonHead()
    {
        //Looks at player
        transform.rotation = Quaternion.Slerp(transform.rotation,
        Quaternion.LookRotation(Camera.main.transform.position - transform.position), 1.3f * Time.deltaTime);
        //If there is a given distance between player and enemy it still moves, otherwise it damages player with cooldown
        if (Vector3.Distance(gameObject.transform.position, Camera.main.transform.position) >= 2.5f)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            if (canDamage)
            {
                GameManager.GetGameManager().DamagePlayer(damage);
                playerObject.GetComponent<AudioSource>().PlayOneShot(damageSounds[Random.Range(0, damageSounds.Length)]);
                StartCoroutine(damageCooldown());
            }
        }
    }

    private void SkeletonBoss()
    {
        //Choosing boss logic depending on name of enemy
        if (gameObject.name.Contains("SkullBoss") || gameObject.name.Contains("SkullDestroyer"))
        {
            //Looks at player
            transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(Camera.main.transform.position - transform.position), Time.deltaTime * 0.75F);
            //If there is a given distance between enemy and player enemy moves, otherwise it damages player with cooldown
            if (Vector3.Distance(gameObject.transform.position, Camera.main.transform.position) >= 8f)
            {
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
            else
            {
                if (canDamage)
                {
                    GameManager.GetGameManager().DamagePlayer(damage);
                    playerObject.GetComponent<AudioSource>().PlayOneShot(damageSounds[Random.Range(0, damageSounds.Length)]);
                    StartCoroutine(damageCooldown());
                }
            }
        }
        else
        {
            //Looks at player
            transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(Camera.main.transform.position - transform.position), Time.deltaTime * 1.3F);
            //If there is a given distance between enemy and player enemy moves, otherwise it damages player with cooldown
            if (Vector3.Distance(gameObject.transform.position, Camera.main.transform.position) >= 4f)
            {
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
            else
            {
                if (canDamage)
                {
                    GameManager.GetGameManager().DamagePlayer(damage);
                    playerObject.GetComponent<AudioSource>().PlayOneShot(damageSounds[Random.Range(0, damageSounds.Length)]);
                    StartCoroutine(damageCooldown());
                }
            }
        }
    }

    IEnumerator damageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(1.5f);
        canDamage = true;
    }
}
