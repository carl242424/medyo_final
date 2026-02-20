using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI Settings")]
    public Vector3 localOffset = new Vector3(0f, 1.2f, 0f);
    public Vector2 size = new Vector2(1.2f, 0.18f);
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.6f);
    public Color foregroundColor = Color.red;
    public Color levelTextColor = Color.yellow;

    private GameObject uiRoot;
    private RectTransform bgRect;
    private RectTransform fgRect;
    private TextMeshProUGUI levelText;
    private TextMeshProUGUI hpText;
    private Damageable damageable;
    private Vector3 lastParentScale = Vector3.one;

    private void Start()
    {
        damageable = GetComponent<Damageable>();
        CreateUI();
    }

    private void OnEnable()
    {
        if (damageable == null) damageable = GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.healthChanged.AddListener(OnHPChanged);
            damageable.maxHealthChanged.AddListener(OnMaxHPChanged);
        }
    }

    private void OnDisable()
    {
        if (damageable != null)
        {
            damageable.healthChanged.RemoveListener(OnHPChanged);
            damageable.maxHealthChanged.RemoveListener(OnMaxHPChanged);
        }
    }

    private void CreateUI()
    {
        // 1. Root Setup
        uiRoot = new GameObject("EnemyHealthBar_UI");
        uiRoot.transform.SetParent(transform, false);
        uiRoot.transform.localPosition = localOffset;

        Canvas canvas = uiRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform rootRt = uiRoot.GetComponent<RectTransform>();
        // Add width to the root to accommodate the level text on the left
        rootRt.sizeDelta = new Vector2(size.x + 1.0f, size.y); 

        // 2. Background Bar
        GameObject bg = new GameObject("BG");
        bg.transform.SetParent(uiRoot.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = backgroundColor;
        bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.5f, 0.5f);
        bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.sizeDelta = size;

        // 3. Foreground (Health Fill)
        GameObject fg = new GameObject("FG");
        fg.transform.SetParent(bg.transform, false);
        Image fgImg = fg.AddComponent<Image>();
        fgImg.color = foregroundColor;
        fgRect = fg.GetComponent<RectTransform>();
        fgRect.anchorMin = new Vector2(0, 0);
        fgRect.anchorMax = new Vector2(1, 1);
        fgRect.pivot = new Vector2(0f, 0.5f); 
        fgRect.sizeDelta = Vector2.zero; 
        fgRect.localScale = new Vector3(1f, 1f, 1f);

        // 4. Level Text
        GameObject lt = new GameObject("LevelText");
        lt.transform.SetParent(bg.transform, false); // Parent to BG so it's relative to the bar
        levelText = lt.AddComponent<TextMeshProUGUI>();
        
        levelText.alignment = TextAlignmentOptions.Right; 
        levelText.color = levelTextColor;
        levelText.fontSize = size.y * 0.8f; 
        levelText.textWrappingMode = TextWrappingModes.NoWrap;

        RectTransform ltrt = lt.GetComponent<RectTransform>();
        ltrt.anchorMin = new Vector2(0f, 0.5f); // Anchor to left of bar
        ltrt.anchorMax = new Vector2(0f, 0.5f);
        ltrt.pivot = new Vector2(1f, 0.5f);    // Pivot on the right of the text box
        
        ltrt.anchoredPosition = new Vector2(-0.05f, 0f); // Slight gap to the left
        ltrt.sizeDelta = new Vector2(1.0f, size.y);

        // 4b. HP Text (centered on the bar)
        GameObject ht = new GameObject("HPText");
        ht.transform.SetParent(bg.transform, false);
        hpText = ht.AddComponent<TextMeshProUGUI>();
        
        hpText.alignment = TextAlignmentOptions.Center;
        hpText.color = Color.white;
        hpText.fontSize = size.y * 0.7f;
        hpText.textWrappingMode = TextWrappingModes.NoWrap;

        RectTransform htrt = ht.GetComponent<RectTransform>();
        htrt.anchorMin = Vector2.zero;
        htrt.anchorMax = Vector2.one;
        htrt.offsetMin = Vector2.zero;
        htrt.offsetMax = Vector2.zero;

        // 5. Add Billboard Script
        uiRoot.AddComponent<FaceCamera>();

        if (damageable != null)
            UpdateBar(damageable.Health, damageable.MaxHealth);

        UpdateLevelText();
    }

    private void OnHPChanged(int newHP, int maxHP) => UpdateBar(newHP, maxHP);
    private void OnMaxHPChanged(int hp, int newMax) => UpdateBar(hp, newMax);

    private void Update()
    {
        // Counter-flip the UI when the parent enemy flips
        if (transform.localScale.x != lastParentScale.x)
        {
            lastParentScale = transform.localScale;
            if (uiRoot != null)
            {
                uiRoot.transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1f, 1f);
            }
        }
    }

    private void UpdateBar(int hp, int maxHp)
    {
        if (fgRect == null) return;
        float pct = maxHp > 0 ? (float)hp / (float)maxHp : 0f;
        pct = Mathf.Clamp01(pct);
        fgRect.localScale = new Vector3(pct, 1f, 1f);
        
        // Update HP text display
        if (hpText != null)
        {
            hpText.text = $"{hp}/{maxHp}";
        }
    }

    private void UpdateLevelText()
    {
        EnemyStats stats = GetComponentInParent<EnemyStats>();
        if (levelText != null)
        {
            int lvl = (stats != null) ? stats.level : 1;
            levelText.text = "Lv " + lvl;
        }
    }
}