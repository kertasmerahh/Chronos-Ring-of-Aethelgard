using UnityEngine;
using UnityEngine.UI;

public class GlobalBrightness : MonoBehaviour
{
    public static GlobalBrightness Instance { get; private set; }

    [SerializeField] private Image brightnessOverlay;
    private Canvas overlayCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupOverlay();

        // Muat nilai tersimpan
        float savedBrightness = PlayerPrefs.GetFloat("ScreenBrightness", 1.0f);
        SetBrightness(savedBrightness);
    }

    private void SetupOverlay()
    {
        if (brightnessOverlay != null) return;

        // Buat Canvas Overlay khusus kecerahan secara runtime jika belum di-assign
        GameObject canvasObj = new GameObject("BrightnessOverlayCanvas");
        canvasObj.transform.SetParent(transform);

        overlayCanvas = canvasObj.AddComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayCanvas.sortingOrder = 9999; // Lapisan paling atas

        canvasObj.AddComponent<CanvasScaler>();

        // Buat Image hitam full-screen
        GameObject imageObj = new GameObject("DarkOverlay");
        imageObj.transform.SetParent(canvasObj.transform, false);

        brightnessOverlay = imageObj.AddComponent<Image>();
        brightnessOverlay.color = new Color(0f, 0f, 0f, 0f);
        brightnessOverlay.raycastTarget = false; // Jangan halangi klik mouse

        RectTransform rt = imageObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// Mengatur kecerahan layar:
    /// val = 1.0 (100% normal) -> overlay hitam 0%
    /// val = 0.0 (0% gelap)    -> overlay hitam 70%
    /// </summary>
    public void SetBrightness(float val)
    {
        float clamped = Mathf.Clamp01(val);
        float darkAlpha = (1.0f - clamped) * 0.72f;

        if (brightnessOverlay != null)
        {
            brightnessOverlay.color = new Color(0f, 0f, 0f, darkAlpha);
        }
    }
}
