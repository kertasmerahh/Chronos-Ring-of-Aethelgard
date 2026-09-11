using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuBuilder : MonoBehaviour
{
    [MenuItem("Chronos/Setup Main Menu")]
    public static void BuildMainMenu()
    {
        string scenePath = "Assets/Scenes/MainMenu.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        // 1. Setup Main Camera
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camObj.tag = "MainCamera";
            cam = camObj.GetComponent<Camera>();
        }
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.transform.position = new Vector3(0, 0, -10);

        // 2. Setup EventSystem
        EnsureEventSystem();

        // 3. Load Sprites & Font Assets
        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Menu/Menu_Background.png");
        Sprite logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Menu/Logo_Chronos.png");
        Sprite subtitleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Menu/Subtitle_RingOfAethelgard.png");
        Sprite btnPlaySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Menu/Button_Play.png");
        Sprite btnOptionsSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Menu/Button_Options.png");
        Sprite btnExitSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Menu/Button_Exit.png");
        Sprite modalBgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Menu/Modal_Options_BG.png");
        Sprite btnCloseSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Menu/Button_Close_X.png");
        Sprite tabSettingsSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Menu/Button_Tab_Settings.png");
        Sprite tabCollectionsSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Menu/Button_Tab_Collections.png");
        Sprite tabCreditsSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Menu/Button_Tab_Credits.png");
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/ChronosFont-ChildHood SDF.asset");

        // 4. Setup MainMenuManager
        GameObject managerObj = GameObject.Find("MainMenuManager");
        if (managerObj == null)
        {
            // Cek jika ada UIManager lama
            GameObject oldUI = GameObject.Find("UIManager");
            if (oldUI != null) DestroyImmediate(oldUI);

            managerObj = new GameObject("MainMenuManager");
        }
        MainMenuController menuController = managerObj.GetComponent<MainMenuController>();
        if (menuController == null) menuController = managerObj.AddComponent<MainMenuController>();

        GlobalBrightness globalBrightness = managerObj.GetComponent<GlobalBrightness>();
        if (globalBrightness == null) globalBrightness = managerObj.AddComponent<GlobalBrightness>();

        PindahScene pindahScene = managerObj.GetComponent<PindahScene>();
        if (pindahScene == null) pindahScene = managerObj.AddComponent<PindahScene>();

        // 5. Setup Canvas
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        }

        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // Bersihkan child lama di canvas agar bersih dan fresh
        for (int i = canvasObj.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(canvasObj.transform.GetChild(i).gameObject);
        }

        // ==========================================
        // 6. BUAT ELEMEN MENU
        // ==========================================

        // A. BACKGROUND FULLSCREEN
        GameObject bgObj = CreateImage(canvasObj.transform, "Background_Cosmos", bgSprite, Color.white);
        RectTransform bgRt = bgObj.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.sizeDelta = Vector2.zero;

        // B. HEADER LOGO & SUBTITLE
        GameObject headerObj = new GameObject("Header_Title", typeof(RectTransform));
        headerObj.transform.SetParent(canvasObj.transform, false);
        RectTransform headerRt = headerObj.GetComponent<RectTransform>();
        headerRt.anchorMin = new Vector2(0.5f, 1f);
        headerRt.anchorMax = new Vector2(0.5f, 1f);
        headerRt.pivot = new Vector2(0.5f, 1f);
        headerRt.anchoredPosition = new Vector2(0, -110);
        headerRt.sizeDelta = new Vector2(900, 320);

        // Logo Chronos Emas
        GameObject logoObj = CreateImage(headerObj.transform, "Logo_Chronos", logoSprite, Color.white);
        RectTransform logoRt = logoObj.GetComponent<RectTransform>();
        logoRt.anchorMin = new Vector2(0.5f, 1f);
        logoRt.anchorMax = new Vector2(0.5f, 1f);
        logoRt.pivot = new Vector2(0.5f, 1f);
        logoRt.anchoredPosition = new Vector2(0, 0);
        logoRt.sizeDelta = new Vector2(820, 223);

        // Subtitle Ring of Aethelgard
        GameObject subObj = CreateImage(headerObj.transform, "Subtitle_RingOfAethelgard", subtitleSprite, Color.white);
        RectTransform subRt = subObj.GetComponent<RectTransform>();
        subRt.anchorMin = new Vector2(0.5f, 1f);
        subRt.anchorMax = new Vector2(0.5f, 1f);
        subRt.pivot = new Vector2(0.5f, 1f);
        subRt.anchoredPosition = new Vector2(0, -220);
        subRt.sizeDelta = new Vector2(620, 42);

        // C. BUTTONS GROUP (PLAY, OPTIONS, EXIT)
        GameObject buttonsObj = new GameObject("Buttons_Group", typeof(RectTransform));
        buttonsObj.transform.SetParent(canvasObj.transform, false);
        RectTransform buttonsRt = buttonsObj.GetComponent<RectTransform>();
        buttonsRt.anchorMin = new Vector2(0.5f, 0f);
        buttonsRt.anchorMax = new Vector2(0.5f, 0f);
        buttonsRt.pivot = new Vector2(0.5f, 0f);
        buttonsRt.anchoredPosition = new Vector2(0, 110);
        buttonsRt.sizeDelta = new Vector2(500, 260);

        // 1. Play Button
        Button btnPlay = CreateImageButton(buttonsObj.transform, "Button_Play", btnPlaySprite, new Vector2(300, 108), new Vector2(0, 140));
        UnityEventTools.AddPersistentListener(btnPlay.onClick, menuController.PlayGame);

        // 2. Options Button
        Button btnOptions = CreateImageButton(buttonsObj.transform, "Button_Options", btnOptionsSprite, new Vector2(185, 78), new Vector2(-110, 30));
        UnityEventTools.AddPersistentListener(btnOptions.onClick, menuController.OpenOptions);

        // 3. Exit Button
        Button btnExit = CreateImageButton(buttonsObj.transform, "Button_Exit", btnExitSprite, new Vector2(185, 78), new Vector2(110, 30));
        UnityEventTools.AddPersistentListener(btnExit.onClick, menuController.ExitGame);

        // D. FOOTER VERSION
        GameObject versionObj = CreateText(canvasObj.transform, "Text_Version", "v1.0.0 - Remnant Edition", fontAsset, 20, new Color(1f, 1f, 1f, 0.45f), TextAlignmentOptions.BottomRight);
        RectTransform versionRt = versionObj.GetComponent<RectTransform>();
        versionRt.anchorMin = new Vector2(1f, 0f);
        versionRt.anchorMax = new Vector2(1f, 0f);
        versionRt.pivot = new Vector2(1f, 0f);
        versionRt.anchoredPosition = new Vector2(-25, 20);
        versionRt.sizeDelta = new Vector2(300, 40);

        // ==========================================
        // 7. OPTIONS MODAL & TABS
        // ==========================================
        GameObject modalObj = new GameObject("Options_Modal", typeof(RectTransform));
        modalObj.transform.SetParent(canvasObj.transform, false);
        RectTransform modalRt = modalObj.GetComponent<RectTransform>();
        modalRt.anchorMin = new Vector2(0.5f, 0.5f);
        modalRt.anchorMax = new Vector2(0.5f, 0.5f);
        modalRt.pivot = new Vector2(0.5f, 0.5f);
        modalRt.anchoredPosition = Vector2.zero;
        modalRt.sizeDelta = new Vector2(760, 620);

        // Background Modal
        GameObject modalBgObj = CreateImage(modalObj.transform, "Modal_Background", modalBgSprite, Color.white);
        RectTransform mBgRt = modalBgObj.GetComponent<RectTransform>();
        mBgRt.anchorMin = Vector2.zero;
        mBgRt.anchorMax = Vector2.one;
        mBgRt.sizeDelta = Vector2.zero;

        // Close [X] Button
        Button btnClose = CreateImageButton(modalObj.transform, "Button_Close_X", btnCloseSprite, new Vector2(34, 38), new Vector2(330, 260));
        UnityEventTools.AddPersistentListener(btnClose.onClick, menuController.CloseOptions);

        // Title Header Modal
        GameObject modalTitleObj = CreateText(modalObj.transform, "Modal_Title", "P E N G A T U R A N", fontAsset, 36, new Color(0.95f, 0.8f, 0.35f, 1f), TextAlignmentOptions.Center);
        RectTransform mTitleRt = modalTitleObj.GetComponent<RectTransform>();
        mTitleRt.anchorMin = new Vector2(0.5f, 1f);
        mTitleRt.anchorMax = new Vector2(0.5f, 1f);
        mTitleRt.pivot = new Vector2(0.5f, 1f);
        mTitleRt.anchoredPosition = new Vector2(0, -35);
        mTitleRt.sizeDelta = new Vector2(400, 50);

        // Baris Tab Tombol
        GameObject tabsObj = new GameObject("Tabs_Bar", typeof(RectTransform));
        tabsObj.transform.SetParent(modalObj.transform, false);
        RectTransform tabsRt = tabsObj.GetComponent<RectTransform>();
        tabsRt.anchorMin = new Vector2(0.5f, 1f);
        tabsRt.anchorMax = new Vector2(0.5f, 1f);
        tabsRt.pivot = new Vector2(0.5f, 1f);
        tabsRt.anchoredPosition = new Vector2(0, -100);
        tabsRt.sizeDelta = new Vector2(660, 60);

        Button tabSettings = CreateImageButton(tabsObj.transform, "Tab_Settings", tabSettingsSprite, new Vector2(200, 60), new Vector2(-210, 0));
        Button tabCollections = CreateImageButton(tabsObj.transform, "Tab_Collections", tabCollectionsSprite, new Vector2(200, 60), new Vector2(0, 0));
        Button tabCredits = CreateImageButton(tabsObj.transform, "Tab_Credits", tabCreditsSprite, new Vector2(200, 60), new Vector2(210, 0));

        UnityEventTools.AddPersistentListener(tabSettings.onClick, menuController.ShowSettingsTab);
        UnityEventTools.AddPersistentListener(tabCollections.onClick, menuController.ShowCollectionsTab);
        UnityEventTools.AddPersistentListener(tabCredits.onClick, menuController.ShowCreditsTab);

        // Container Konten Tab
        GameObject contentContainer = new GameObject("Tab_Contents", typeof(RectTransform));
        contentContainer.transform.SetParent(modalObj.transform, false);
        RectTransform ccRt = contentContainer.GetComponent<RectTransform>();
        ccRt.anchorMin = new Vector2(0.5f, 0.5f);
        ccRt.anchorMax = new Vector2(0.5f, 0.5f);
        ccRt.pivot = new Vector2(0.5f, 0.5f);
        ccRt.anchoredPosition = new Vector2(0, -50);
        ccRt.sizeDelta = new Vector2(660, 370);

        // -------------------------------------------------------------
        // TAB 1: SETTINGS (Kecerahan Layar & Volume)
        // -------------------------------------------------------------
        GameObject panelSettings = new GameObject("Panel_Settings", typeof(RectTransform));
        panelSettings.transform.SetParent(contentContainer.transform, false);
        RectTransform pSetRt = panelSettings.GetComponent<RectTransform>();
        pSetRt.anchorMin = Vector2.zero;
        pSetRt.anchorMax = Vector2.one;
        pSetRt.sizeDelta = Vector2.zero;

        // Label Kecerahan Layar
        GameObject lblBright = CreateText(panelSettings.transform, "Label_Brightness", "Kecerahan Layar", fontAsset, 24, Color.white, TextAlignmentOptions.Left);
        RectTransform lbRt = lblBright.GetComponent<RectTransform>();
        lbRt.anchoredPosition = new Vector2(-120, 95);
        lbRt.sizeDelta = new Vector2(300, 35);

        // Nilai Kecerahan Text ("100%")
        GameObject valBright = CreateText(panelSettings.transform, "Text_BrightnessValue", "100%", fontAsset, 24, new Color(0.95f, 0.8f, 0.35f, 1f), TextAlignmentOptions.Right);
        RectTransform vbRt = valBright.GetComponent<RectTransform>();
        vbRt.anchoredPosition = new Vector2(220, 95);
        vbRt.sizeDelta = new Vector2(100, 35);
        TextMeshProUGUI tmpBrightVal = valBright.GetComponent<TextMeshProUGUI>();

        // Slider Kecerahan
        Slider sliderBright = CreateSlider(panelSettings.transform, "Slider_Brightness", 560, 24);
        RectTransform sbrRt = sliderBright.GetComponent<RectTransform>();
        sbrRt.anchoredPosition = new Vector2(0, 50);
        UnityEventTools.AddPersistentListener(sliderBright.onValueChanged, menuController.SetBrightness);

        // Label Volume Suara
        GameObject lblVol = CreateText(panelSettings.transform, "Label_Volume", "Volume Suara", fontAsset, 24, Color.white, TextAlignmentOptions.Left);
        RectTransform lvRt = lblVol.GetComponent<RectTransform>();
        lvRt.anchoredPosition = new Vector2(-120, -15);
        lvRt.sizeDelta = new Vector2(300, 35);

        // Nilai Volume Text ("100%")
        GameObject valVol = CreateText(panelSettings.transform, "Text_VolumeValue", "100%", fontAsset, 24, new Color(0.95f, 0.8f, 0.35f, 1f), TextAlignmentOptions.Right);
        RectTransform vvRt = valVol.GetComponent<RectTransform>();
        vvRt.anchoredPosition = new Vector2(220, -15);
        vvRt.sizeDelta = new Vector2(100, 35);
        TextMeshProUGUI tmpVolVal = valVol.GetComponent<TextMeshProUGUI>();

        // Slider Volume
        Slider sliderVol = CreateSlider(panelSettings.transform, "Slider_Volume", 560, 24);
        RectTransform svrRt = sliderVol.GetComponent<RectTransform>();
        svrRt.anchoredPosition = new Vector2(0, -60);
        UnityEventTools.AddPersistentListener(sliderVol.onValueChanged, menuController.SetVolume);

        // Catatan Simpan Otomatis
        GameObject hintObj = CreateText(panelSettings.transform, "Text_Hint", "* Pengaturan disimpan otomatis dan berlaku di semua arena gameplay", fontAsset, 17, new Color(0.8f, 0.8f, 0.8f, 0.65f), TextAlignmentOptions.Center);
        RectTransform hintRt = hintObj.GetComponent<RectTransform>();
        hintRt.anchoredPosition = new Vector2(0, -140);
        hintRt.sizeDelta = new Vector2(600, 35);

        // -------------------------------------------------------------
        // TAB 2: COLLECTIONS
        // -------------------------------------------------------------
        GameObject panelCollections = new GameObject("Panel_Collections", typeof(RectTransform));
        panelCollections.transform.SetParent(contentContainer.transform, false);
        RectTransform pColRt = panelCollections.GetComponent<RectTransform>();
        pColRt.anchorMin = Vector2.zero;
        pColRt.anchorMax = Vector2.one;
        pColRt.sizeDelta = Vector2.zero;

        string colDesc = "CHRONOS ARCHIVES\n\n" +
            "- THE REMNANT: Pengelana waktu dengan jubah cokelat yang mengitari cincin abadi Aethelgard.\n\n" +
            "- ANOMALY DARAT: Makhluk bayangan permukaan planet yang menghalangi langkah Remnant.\n\n" +
            "- ANOMALY UDARA: Ancaman melayang berkecepatan tinggi dari dimensi kehampaan.\n\n" +
            "- GLUTTONY BOSS: Gerbang purba yang menjaga kunci waktu dunia.";
        GameObject colTextObj = CreateText(panelCollections.transform, "Text_CollectionsInfo", colDesc, fontAsset, 20, new Color(0.92f, 0.92f, 0.9f, 1f), TextAlignmentOptions.TopLeft);
        RectTransform ctRt = colTextObj.GetComponent<RectTransform>();
        ctRt.anchoredPosition = new Vector2(0, 0);
        ctRt.sizeDelta = new Vector2(600, 320);

        // -------------------------------------------------------------
        // TAB 3: CREDITS
        // -------------------------------------------------------------
        GameObject panelCredits = new GameObject("Panel_Credits", typeof(RectTransform));
        panelCredits.transform.SetParent(contentContainer.transform, false);
        RectTransform pCreRt = panelCredits.GetComponent<RectTransform>();
        pCreRt.anchorMin = Vector2.zero;
        pCreRt.anchorMax = Vector2.one;
        pCreRt.sizeDelta = Vector2.zero;

        string credDesc = "CHRONOS: RING OF AETHELGARD\n\n" +
            "Proyek Game 2D Circular Action Runner\n\n" +
            "Game Developer: Remnant Team\n" +
            "Art & Assets: MBC LAS Week 2\n" +
            "Handcrafted Font: Chronos ChildHood\n" +
            "Engine: Unity 2D (URP)";
        GameObject credTextObj = CreateText(panelCredits.transform, "Text_CreditsInfo", credDesc, fontAsset, 22, new Color(0.95f, 0.85f, 0.5f, 1f), TextAlignmentOptions.Center);
        RectTransform crtRt = credTextObj.GetComponent<RectTransform>();
        crtRt.anchoredPosition = new Vector2(0, 0);
        crtRt.sizeDelta = new Vector2(600, 320);

        // Sembunyikan modal di awal
        panelSettings.SetActive(true);
        panelCollections.SetActive(false);
        panelCredits.SetActive(false);
        modalObj.SetActive(false);

        // ==========================================
        // 8. HUBUNGKAN KE MAINMENUCONTROLLER
        // ==========================================
        SerializedObject so = new SerializedObject(menuController);
        so.FindProperty("optionsModal").objectReferenceValue = modalObj;
        so.FindProperty("panelSettings").objectReferenceValue = panelSettings;
        so.FindProperty("panelCredits").objectReferenceValue = panelCredits;
        so.FindProperty("panelCollections").objectReferenceValue = panelCollections;
        so.FindProperty("sliderBrightness").objectReferenceValue = sliderBright;
        so.FindProperty("textBrightnessValue").objectReferenceValue = tmpBrightVal;
        so.FindProperty("sliderVolume").objectReferenceValue = sliderVol;
        so.FindProperty("textVolumeValue").objectReferenceValue = tmpVolVal;
        so.FindProperty("logoTransform").objectReferenceValue = headerRt;
        SerializedProperty propAnim = so.FindProperty("animateLogo");
        if (propAnim != null) propAnim.boolValue = false;
        so.ApplyModifiedProperties();

        // 9. Simpan Prefab Canvas & scene
        PrefabUtility.SaveAsPrefabAsset(canvasObj, "Assets/MainMenu_Canvas.prefab");
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("<color=#00FF00>[Chronos]</color> Main Menu berhasil dibuat dan disimpan secara lengkap!");
    }

    [InitializeOnLoadMethod]
    private static void OnProjectLoad()
    {
        EditorApplication.delayCall += () =>
        {
            // Otomatis bangun dan simpan scene MainMenu.unity
            BuildMainMenu();
        };
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }
    }

    private static GameObject CreateImage(Transform parent, string name, Sprite sprite, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        Image img = obj.GetComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.preserveAspect = true;
        return obj;
    }

    private static Button CreateImageButton(Transform parent, string name, Sprite sprite, Vector2 size, Vector2 anchoredPos)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;

        Image img = obj.GetComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;

        Button btn = obj.GetComponent<Button>();
        btn.targetGraphic = img;

        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1.15f, 1.15f, 1.05f, 1f);
        cb.pressedColor = new Color(0.85f, 0.85f, 0.8f, 1f);
        cb.selectedColor = Color.white;
        btn.colors = cb;

        return btn;
    }

    private static GameObject CreateText(Transform parent, string name, string content, TMP_FontAsset font, float fontSize, Color color, TextAlignmentOptions align)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = content;
        if (font != null) tmp.font = font;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = align;
        tmp.raycastTarget = false;
        return obj;
    }

    private static Slider CreateSlider(Transform parent, string name, float width, float height)
    {
        GameObject sliderObj = new GameObject(name, typeof(RectTransform), typeof(Slider));
        sliderObj.transform.SetParent(parent, false);
        RectTransform sliderRt = sliderObj.GetComponent<RectTransform>();
        sliderRt.sizeDelta = new Vector2(width, height);
        Slider slider = sliderObj.GetComponent<Slider>();

        // Background
        GameObject bgObj = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bgObj.transform.SetParent(sliderObj.transform, false);
        RectTransform bgRt = bgObj.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.sizeDelta = Vector2.zero;
        Image bgImg = bgObj.GetComponent<Image>();
        bgImg.color = new Color(0.12f, 0.11f, 0.1f, 0.85f);

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform faRt = fillArea.GetComponent<RectTransform>();
        faRt.anchorMin = new Vector2(0f, 0.25f);
        faRt.anchorMax = new Vector2(1f, 0.75f);
        faRt.offsetMin = new Vector2(4f, 0f);
        faRt.offsetMax = new Vector2(-4f, 0f);

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fRt = fill.GetComponent<RectTransform>();
        fRt.sizeDelta = Vector2.zero;
        Image fImg = fill.GetComponent<Image>();
        fImg.color = new Color(0.92f, 0.72f, 0.25f, 1f);

        // Handle Slide Area
        GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform haRt = handleArea.GetComponent<RectTransform>();
        haRt.anchorMin = Vector2.zero;
        haRt.anchorMax = Vector2.one;
        haRt.offsetMin = new Vector2(8f, 0f);
        haRt.offsetMax = new Vector2(-8f, 0f);

        GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        handle.transform.SetParent(handleArea.transform, false);
        RectTransform hRt = handle.GetComponent<RectTransform>();
        hRt.sizeDelta = new Vector2(height * 1.25f, height * 1.25f);
        Image hImg = handle.GetComponent<Image>();
        hImg.color = new Color(1f, 0.9f, 0.55f, 1f);

        slider.targetGraphic = hImg;
        slider.fillRect = fRt;
        slider.handleRect = hRt;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        return slider;
    }
}
