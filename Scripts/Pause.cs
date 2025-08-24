using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider sensivitySlider;
    [SerializeField] private AudioMixer sfxMixer;
    [SerializeField] private Text[] sliderValueTexts;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject victoryMenu;
    [SerializeField] private GameObject gameOverMenu;


    private void Awake()
    {
        //Changes values of sliders to saved values
        sensivitySlider.value = PlayerPrefs.GetFloat("sensivity");
        sfxSlider.value = PlayerPrefs.GetFloat("sfx");
    }

    private void Update()
    {
        //Updates saved values to new values of sliders
        PlayerPrefs.SetFloat("sensivity", sensivitySlider.value);
        PlayerPrefs.SetFloat("sfx", sfxSlider.value);
        sliderValueTexts[0].text = System.Math.Round(sensivitySlider.value, 2).ToString();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.active)
            {
                Resume();
            }
            else
            {
                Paused();
            }
        }
    }

    public void Paused()
    {
        if (!victoryMenu.active && !gameOverMenu.active && !shopMenu.active) {
            //Pauses game completely
            AudioListener.volume = 0;
            Time.timeScale = 0.0f;
            pauseMenu.SetActive(true);
            GameManager.GetGameManager().player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
            GameManager.GetGameManager().player.GetComponent<Shoot>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Resume()
    {
        AudioListener.volume = 1;
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        GameManager.GetGameManager().player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
        GameManager.GetGameManager().player.GetComponent<Shoot>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void SetSFX(float f)
    {
        sfxMixer.SetFloat("value", f);
    }

}
