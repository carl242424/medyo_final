using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Shows a nickname input panel when Start is clicked. On confirm, saves nickname and loads the game.
/// Attach to the Start button and wire the Button's onClick to ShowNicknamePanel().
/// </summary>
[RequireComponent(typeof(Button))]
public class NicknamePrompt : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("Scene build index to load after nickname is set (e.g. Level Selection = 2)")]
    public int sceneBuildIndex = 2;

    [Header("Optional - leave empty to create at runtime")]
    [Tooltip("Panel with input field and confirm button. If null, a simple panel is created.")]
    public GameObject nicknamePanel;
    [Tooltip("TMP Input field for nickname. If null, one will be created.")]
    public TMP_InputField nicknameInput;
    [Tooltip("Legacy InputField - use if not using TMP. Takes precedence over nicknameInput if both exist.")]
    public InputField legacyInputField;
    [Tooltip("Confirm button. If null, one will be created.")]
    public Button confirmButton;
    [Tooltip("Objects to hide when nickname panel is shown (e.g. Start/Settings/Quit buttons)")]
    public GameObject[] hideWhenPromptShown;

    private GameObject _panelRoot;
    private bool _panelCreated;

    private void Awake()
    {
        if ((hideWhenPromptShown == null || hideWhenPromptShown.Length == 0) && transform.parent != null)
        {
            hideWhenPromptShown = new[] { transform.parent.gameObject };
        }
    }

    public void ShowNicknamePanel()
    {
        if (nicknamePanel != null)
        {
            nicknamePanel.SetActive(true);
            SetNicknameText(PlayerNickname.Get());
            SetHidden(true);
            var canvas = FindObjectOfType<Canvas>();
            var runner = canvas?.GetComponent<MonoBehaviour>();
            if (runner != null) runner.StartCoroutine(SelectInputNextFrame(legacyInputField, nicknameInput));
            return;
        }

        if (_panelRoot != null)
        {
            _panelRoot.SetActive(true);
            if (_panelCreated)
            {
                SetNicknameText(PlayerNickname.Get());
                var canvas = FindObjectOfType<Canvas>();
                var runner = canvas?.GetComponent<MonoBehaviour>();
                if (runner != null) runner.StartCoroutine(SelectInputNextFrame(legacyInputField, nicknameInput));
            }
            SetHidden(true);
            return;
        }

        CreateNicknamePanel();
    }

    public void OnConfirmNickname()
    {
        string name = GetNicknameText();
        PlayerNickname.Set(name);

        if (nicknamePanel != null)
            nicknamePanel.SetActive(false);
        if (_panelRoot != null)
            _panelRoot.SetActive(false);

        SetHidden(false);
        LoadGame();
    }

    public void OnCancel()
    {
        if (nicknamePanel != null)
            nicknamePanel.SetActive(false);
        if (_panelRoot != null)
            _panelRoot.SetActive(false);
        SetHidden(false);
    }

    private string GetNicknameText()
    {
        if (legacyInputField != null) return legacyInputField.text;
        if (nicknameInput != null) return nicknameInput.text;
        return "";
    }

    private void SetNicknameText(string text)
    {
        if (legacyInputField != null) legacyInputField.text = text;
        else if (nicknameInput != null) nicknameInput.text = text;
    }

    private void SetHidden(bool hidden)
    {
        if (hideWhenPromptShown == null) return;
        foreach (var go in hideWhenPromptShown)
        {
            if (go != null) go.SetActive(!hidden);
        }
    }

    private void CreateNicknamePanel()
    {
        if (_panelRoot != null) return;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        _panelRoot = new GameObject("NicknamePanel");
        _panelRoot.transform.SetParent(canvas.transform, false);

        RectTransform rootRect = _panelRoot.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        Image bg = _panelRoot.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);
        bg.raycastTarget = false;

        GameObject blockBtn = new GameObject("BlockButton");
        blockBtn.transform.SetParent(_panelRoot.transform, false);
        RectTransform blockRect = blockBtn.AddComponent<RectTransform>();
        blockRect.anchorMin = Vector2.zero;
        blockRect.anchorMax = Vector2.one;
        blockRect.offsetMin = Vector2.zero;
        blockRect.offsetMax = Vector2.zero;
        Button blockButton = blockBtn.AddComponent<Button>();
        Image blockImg = blockBtn.AddComponent<Image>();
        blockImg.color = new Color(0, 0, 0, 0.01f);
        blockImg.raycastTarget = true;
        blockButton.targetGraphic = blockImg;
        blockButton.onClick.AddListener(OnCancel);

        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(_panelRoot.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(400, 200);
        panelRect.anchoredPosition = Vector2.zero;
        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);

        GameObject closeObj = new GameObject("CloseButton");
        closeObj.transform.SetParent(panel.transform, false);
        RectTransform closeRect = closeObj.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 1f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.pivot = new Vector2(1f, 1f);
        closeRect.anchoredPosition = new Vector2(-10, -10);
        closeRect.sizeDelta = new Vector2(36, 36);
        Button closeBtn = closeObj.AddComponent<Button>();
        Image closeImg = closeObj.AddComponent<Image>();
        closeImg.color = new Color(0.6f, 0.2f, 0.2f, 1f);

        GameObject closeTextObj = new GameObject("Text");
        closeTextObj.transform.SetParent(closeObj.transform, false);
        RectTransform closeTextRect = closeTextObj.AddComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.offsetMin = Vector2.zero;
        closeTextRect.offsetMax = Vector2.zero;
        TextMeshProUGUI closeText = closeTextObj.AddComponent<TextMeshProUGUI>();
        closeText.text = "X";
        closeText.fontSize = 24;
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.color = Color.white;
        closeBtn.targetGraphic = closeImg;
        closeBtn.onClick.AddListener(OnCancel);

        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(panel.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.5f, 1f);
        labelRect.anchorMax = new Vector2(0.5f, 1f);
        labelRect.pivot = new Vector2(0.5f, 1f);
        labelRect.anchoredPosition = new Vector2(0, -20);
        labelRect.sizeDelta = new Vector2(360, 30);
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = "Enter your nickname";
        label.fontSize = 24;
        label.alignment = TextAlignmentOptions.Center;

        GameObject inputObj = new GameObject("InputField");
        inputObj.transform.SetParent(panel.transform, false);
        RectTransform inputRect = inputObj.AddComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0.5f, 0.5f);
        inputRect.anchorMax = new Vector2(0.5f, 0.5f);
        inputRect.anchoredPosition = new Vector2(0, 10);
        inputRect.sizeDelta = new Vector2(340, 40);

        GameObject inputBg = new GameObject("Background");
        inputBg.transform.SetParent(inputObj.transform, false);
        RectTransform inputBgRect = inputBg.AddComponent<RectTransform>();
        inputBgRect.anchorMin = Vector2.zero;
        inputBgRect.anchorMax = Vector2.one;
        inputBgRect.offsetMin = Vector2.zero;
        inputBgRect.offsetMax = Vector2.zero;
        Image inputBgImg = inputBg.AddComponent<Image>();
        inputBgImg.color = new Color(0.2f, 0.2f, 0.25f, 1f);
        inputBgImg.raycastTarget = true;

        GameObject inputTextObj = new GameObject("Text");
        inputTextObj.transform.SetParent(inputObj.transform, false);
        RectTransform inputTextRect = inputTextObj.AddComponent<RectTransform>();
        inputTextRect.anchorMin = new Vector2(0, 0);
        inputTextRect.anchorMax = new Vector2(1, 1);
        inputTextRect.offsetMin = new Vector2(10, 6);
        inputTextRect.offsetMax = new Vector2(-10, -6);
        Text inputText = inputTextObj.AddComponent<Text>();
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font != null) inputText.font = font;
        inputText.fontSize = 20;
        inputText.color = Color.white;
        inputText.text = "";
        inputText.supportRichText = false;
        inputText.raycastTarget = false;

        legacyInputField = inputObj.AddComponent<InputField>();
        legacyInputField.targetGraphic = inputBgImg;
        legacyInputField.textComponent = inputText;
        legacyInputField.text = PlayerNickname.Get();
        legacyInputField.characterLimit = 16;
        legacyInputField.contentType = InputField.ContentType.Standard;
        legacyInputField.lineType = InputField.LineType.SingleLine;

        GameObject confirmObj = new GameObject("ConfirmButton");
        confirmObj.transform.SetParent(panel.transform, false);
        RectTransform confirmRect = confirmObj.AddComponent<RectTransform>();
        confirmRect.anchorMin = new Vector2(0.5f, 0f);
        confirmRect.anchorMax = new Vector2(0.5f, 0f);
        confirmRect.pivot = new Vector2(0.5f, 0f);
        confirmRect.anchoredPosition = new Vector2(0, 20);
        confirmRect.sizeDelta = new Vector2(160, 44);
        confirmButton = confirmObj.AddComponent<Button>();
        Image confirmImg = confirmObj.AddComponent<Image>();
        confirmImg.color = new Color(0.2f, 0.6f, 0.3f, 1f);

        GameObject confirmTextObj = new GameObject("Text");
        confirmTextObj.transform.SetParent(confirmObj.transform, false);
        RectTransform confirmTextRect = confirmTextObj.AddComponent<RectTransform>();
        confirmTextRect.anchorMin = Vector2.zero;
        confirmTextRect.anchorMax = Vector2.one;
        confirmTextRect.offsetMin = Vector2.zero;
        confirmTextRect.offsetMax = Vector2.zero;
        TextMeshProUGUI confirmText = confirmTextObj.AddComponent<TextMeshProUGUI>();
        confirmText.text = "Start";
        confirmText.fontSize = 22;
        confirmText.alignment = TextAlignmentOptions.Center;
        confirmText.color = Color.white;

        confirmButton.targetGraphic = confirmImg;
        confirmButton.onClick.AddListener(OnConfirmNickname);

        _panelCreated = true;
        SetHidden(true);
        var runner = canvas.GetComponent<MonoBehaviour>();
        if (runner != null) runner.StartCoroutine(SelectInputNextFrame(legacyInputField, nicknameInput));
    }

    private static IEnumerator SelectInputNextFrame(InputField legacy, TMP_InputField tmp)
    {
        yield return null;
        if (legacy != null)
        {
            legacy.ActivateInputField();
            EventSystem.current?.SetSelectedGameObject(legacy.gameObject);
        }
        else if (tmp != null)
        {
            tmp.ActivateInputField();
            EventSystem.current?.SetSelectedGameObject(tmp.gameObject);
        }
    }

    private void LoadGame()
    {
        DontDestroy.DestroyPersistingObjects();
        SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
    }
}
