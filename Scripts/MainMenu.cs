using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject creatorText;
    [SerializeField] private Text[] sliderValueTexts;
    [SerializeField] private Text fpsLimit;
    [SerializeField] private Dropdown fpsLimitDropdown;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown graphicsDropdown;
    [SerializeField] private Text graphics;
    [SerializeField] public Slider sensivitySlider;
    [SerializeField] public Slider sfxSlider;
    [SerializeField] public AudioMixer sfxMixer;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    private void Start()
    {
        //Applies default settings
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = int.Parse(fpsLimit.text);
        fpsLimitDropdown.value = 1;
        graphicsDropdown.value = 2;
        Time.timeScale = 1;
        AudioListener.volume = 1;
        SetGraphics();
        //Applies saved settings
        if (PlayerPrefs.HasKey("sensivity"))
        {
            sensivitySlider.value = PlayerPrefs.GetFloat("sensivity");
        }
        else
        {
            PlayerPrefs.SetFloat("sensivity", 2f);
            sensivitySlider.value = PlayerPrefs.GetFloat("sensivity");
        }
        if (PlayerPrefs.HasKey("sfx"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("sfx");
        }
        else
        {
            PlayerPrefs.SetFloat("sfx", 0f);
            sfxSlider.value = PlayerPrefs.GetFloat("sfx");
        }

    }

    private void Update()
    {
        //Something like easter egg to show owner of the game
        if (Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.R))
        {
            creatorText.SetActive(true);
        }
        //Changes value of saved settings based on slider values
        PlayerPrefs.SetFloat("sensivity", sensivitySlider.value);
        PlayerPrefs.SetFloat("sfx", sfxSlider.value);
        sliderValueTexts[0].text = System.Math.Round(sensivitySlider.value, 2).ToString();
        Application.targetFrameRate = int.Parse(fpsLimit.text);

        ResolutionSettings();
    }

    public void ResolutionSettings()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;

        //Gets resolutions with current refresh rate
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        //Creates new options list with names filtered resolutions, sets current option of dropdown to current resolution of device
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height;
            options.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetGraphics()
    {
        if (graphics.text == "Low")
        {
            QualitySettings.SetQualityLevel(1);
        }
        if (graphics.text == "Medium")
        {
            QualitySettings.SetQualityLevel(2);
        }
        if (graphics.text == "High")
        {
            QualitySettings.SetQualityLevel(4);
        }
        if (graphics.text == "Ultra")
        {
            QualitySettings.SetQualityLevel(5);
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }


    public void SetSFX(float f)
    {
        sfxMixer.SetFloat("value", f);
    }
}
