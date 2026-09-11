using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Panel & Modal Referensi")]
    [SerializeField] private GameObject optionsModal;
    [SerializeField] private GameObject panelSettings;
    [SerializeField] private GameObject panelCredits;
    [SerializeField] private GameObject panelCollections;

    [Header("Pengaturan Slider")]
    [SerializeField] private Slider sliderBrightness;
    [SerializeField] private TextMeshProUGUI textBrightnessValue;
    [SerializeField] private Text textBrightnessValueLegacy;
    [SerializeField] private Slider sliderVolume;
    [SerializeField] private TextMeshProUGUI textVolumeValue;
    [SerializeField] private Text textVolumeValueLegacy;

    [Header("Visual Logo")]
    [SerializeField] private RectTransform logoTransform;
    [SerializeField] private bool animateLogo = false; // Dibuat statis (tidak goyang)
    private Vector2 initialLogoPosition;

    private void Awake()
    {
        // Pastikan timescale normal saat di menu
        Time.timeScale = 1f;

        if (logoTransform != null)
        {
            initialLogoPosition = logoTransform.anchoredPosition;
        }
    }

    private void Start()
    {
        // Tutup modal options di awal
        if (optionsModal != null)
        {
            optionsModal.SetActive(false);
        }

        // Inisialisasi Kecerahan Layar dari PlayerPrefs (default 1.0f)
        float savedBrightness = PlayerPrefs.GetFloat("ScreenBrightness", 1.0f);
        if (sliderBrightness != null)
        {
            sliderBrightness.value = savedBrightness;
            sliderBrightness.onValueChanged.AddListener(SetBrightness);
        }
        UpdateBrightnessText(savedBrightness);
        ApplyBrightness(savedBrightness);

        // Inisialisasi Volume dari PlayerPrefs (default 1.0f)
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        if (sliderVolume != null)
        {
            sliderVolume.value = savedVolume;
            sliderVolume.onValueChanged.AddListener(SetVolume);
        }
        UpdateVolumeText(savedVolume);
        AudioListener.volume = savedVolume;
    }

    private void Update()
    {
        // Dibuat statis: hanya bergerak jika animateLogo sengaja diaktifkan
        if (animateLogo && logoTransform != null)
        {
            float newY = initialLogoPosition.y + Mathf.Sin(Time.time * 2.2f) * 7f;
            logoTransform.anchoredPosition = new Vector2(initialLogoPosition.x, newY);
        }
    }

    #region Navigasi Utama
    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }

    public void OpenOptions()
    {
        if (optionsModal != null)
        {
            optionsModal.SetActive(true);
            ShowSettingsTab(); // Buka tab Settings secara default
        }
    }

    public void CloseOptions()
    {
        if (optionsModal != null)
        {
            optionsModal.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Debug.Log("[MainMenu] Keluar dari permainan...");
        Application.Quit();
    }
    #endregion

    #region Tab Options
    public void ShowSettingsTab()
    {
        if (panelSettings != null) panelSettings.SetActive(true);
        if (panelCredits != null) panelCredits.SetActive(false);
        if (panelCollections != null) panelCollections.SetActive(false);
    }

    public void ShowCreditsTab()
    {
        if (panelSettings != null) panelSettings.SetActive(false);
        if (panelCredits != null) panelCredits.SetActive(true);
        if (panelCollections != null) panelCollections.SetActive(false);
    }

    public void ShowCollectionsTab()
    {
        if (panelSettings != null) panelSettings.SetActive(false);
        if (panelCredits != null) panelCredits.SetActive(false);
        if (panelCollections != null) panelCollections.SetActive(true);
    }
    #endregion

    #region Pengaturan Kecerahan & Volume
    public void SetBrightness(float val)
    {
        PlayerPrefs.SetFloat("ScreenBrightness", val);
        PlayerPrefs.Save();

        UpdateBrightnessText(val);
        ApplyBrightness(val);
    }

    private void UpdateBrightnessText(float val)
    {
        string txt = Mathf.RoundToInt(val * 100f) + "%";
        if (textBrightnessValue != null) textBrightnessValue.text = txt;
        if (textBrightnessValueLegacy != null) textBrightnessValueLegacy.text = txt;
    }

    private void ApplyBrightness(float val)
    {
        if (GlobalBrightness.Instance != null)
        {
            GlobalBrightness.Instance.SetBrightness(val);
        }
    }

    public void SetVolume(float val)
    {
        PlayerPrefs.SetFloat("MasterVolume", val);
        PlayerPrefs.Save();

        AudioListener.volume = val;
        UpdateVolumeText(val);
    }

    private void UpdateVolumeText(float val)
    {
        string txt = Mathf.RoundToInt(val * 100f) + "%";
        if (textVolumeValue != null) textVolumeValue.text = txt;
        if (textVolumeValueLegacy != null) textVolumeValueLegacy.text = txt;
    }
    #endregion
}
