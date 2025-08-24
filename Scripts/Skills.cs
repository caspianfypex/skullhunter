using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Rendering.PostProcessing;

public class Skills : MonoBehaviour
{

    public static Skills instance;
    public string chosenSkill;
    public AudioClip thanosFingerSnapClip;
    public AudioClip crystalsDestroyClip;
    public AudioClip superManClip;
    public bool isSkillUsed = false;
    private int useCount = 0;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (chosenSkill.Equals("Thanos") && useCount < 3 && Input.GetKeyDown(KeyCode.F) && !isSkillUsed && GameManager.GetGameManager().enemiesParent.childCount > 0)
        {
            useCount++;
            isSkillUsed = true;
            StartCoroutine(ThanosFingerSnap());
        }
        if (chosenSkill.Equals("CrystalCrasher") && useCount < 2 && Input.GetKeyDown(KeyCode.F) && !isSkillUsed && GameManager.GetGameManager().canEnemiesSpawn && GameManager.GetGameManager().crystalsParent.childCount > 0)
        {
            useCount++;
            isSkillUsed = true;
            StartCoroutine(CrystalsDestroy());
        }
        if (chosenSkill.Equals("SuperHuman") && useCount < 2 && Input.GetKeyDown(KeyCode.F) && !isSkillUsed)
        {
            useCount++;
            isSkillUsed = true;
            StartCoroutine(SuperMan());
        }
    }


    public IEnumerator ThanosFingerSnap()
    {
        GetComponent<AudioSource>().PlayOneShot(thanosFingerSnapClip);
        yield return new WaitForSeconds(1f);
        foreach (Transform t in GameManager.GetGameManager().enemiesParent.transform)
        {
            if (!t.GetComponent<Enemy>().isBoss)
            {
                if (t.GetComponent<AudioSource>())
                {
                    t.GetComponent<AudioSource>().enabled = false;
                }
                //Disables enemy script
                t.gameObject.GetComponent<Enemy>().enabled = false;
                int c = t.childCount;
                //Disables visibility of all meshes and colliders of enemy object's parent
                for (int i = 0; i < c; i++)
                {
                    if (t.GetChild(i).GetComponent<MeshCollider>() || t.GetChild(i).GetComponent<BoxCollider>())
                    {
                        t.GetChild(i).gameObject.SetActive(false);
                    }
                }
                //Disables child object for animations in enemy object's parent
                if (t.Find("AnimModel"))
                {
                    t.Find("AnimModel").gameObject.SetActive(false);
                }
                //Enables particle effect of death of enemy object
                t.Find("ParticleEffect").gameObject.SetActive(true);
                Destroy(t.gameObject, 2);
                if (GameManager.GetGameManager().wave < 6)
                {
                    GameManager.GetGameManager().coin += 1 * GameManager.GetGameManager().wave;
                }
                else
                {
                    GameManager.GetGameManager().coin += 6;
                }
            }
        }
        yield return new WaitForSeconds(2f);
        //Makes skill usage available again
        isSkillUsed = false;
    }

    private IEnumerator CrystalsDestroy()
    {
        GetComponent<AudioSource>().PlayOneShot(crystalsDestroyClip);
        //Destroys all remaining crystals
        foreach (Transform t in GameManager.GetGameManager().crystalsParent.transform)
        {
            t.GetComponent<MeshCollider>().enabled = false;
            t.GetComponent<MeshRenderer>().enabled = false;
            t.Find("ParticleEffect").gameObject.SetActive(true);
            Destroy(t.gameObject, 2);
            GameManager.GetGameManager().hitCrystals += 1;
        }
        yield return new WaitForSeconds(2f);
        //Makes skill usage available again
        isSkillUsed = false;
    }

    private IEnumerator SuperMan()
    {
        GetComponent<AudioSource>().PlayOneShot(superManClip);
        Vignette vignette;
        //Boosts player's speed and shotgun's jump effect
        GetComponent<FirstPersonController>().m_WalkSpeed += 4f;
        GetComponent<FirstPersonController>().m_RunSpeed += 7F;
        GetComponent<Shoot>().shotgunImpact -= 1.5f;
        Camera.main.GetComponent<PostProcessVolume>().profile.TryGetSettings(out vignette);
        //Enables vignette and pov animation
        vignette.SetAllOverridesTo(true);
        StartCoroutine(LerpFov(75));
        yield return new WaitForSeconds(30f);
        //Resets everything after delay
        StartCoroutine(LerpFov(60));
        GetComponent<FirstPersonController>().m_WalkSpeed -= 4f;
        GetComponent<FirstPersonController>().m_RunSpeed -= 7F;
        GetComponent<Shoot>().shotgunImpact += 1.5f;
        vignette.SetAllOverridesTo(false);
        //Makes skill usage available again
        isSkillUsed = false;
    }

    IEnumerator LerpFov(float fov)
    {
        GetComponent<FirstPersonController>().m_FovKick.originalFov = fov;
        //Increases or decreases FOV smoothly
        while (Camera.main.fieldOfView != fov)
        {
            if (Camera.main.fieldOfView < fov)
            {
                Camera.main.fieldOfView += 1f;
            }
            else
            {
                Camera.main.fieldOfView -= 1f;
            }
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void ChangeSkill(string skillName)
    {
        chosenSkill = skillName;
    }

}
