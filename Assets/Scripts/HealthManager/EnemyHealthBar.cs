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
    private TextMeshProUGUI nameText;
    private Image bgImg;
    private Image fgImg;
    private Damageable damageable;
    private EnemyStats enemyStats;
    private Vector3 lastParentScale = Vector3.one;

    private void Start()
    {
        damageable = GetComponent<Damageable>();
        enemyStats = GetComponent<EnemyStats>();
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
        rootRt.sizeDelta = new Vector2(size.x + 1.0f, size.y + 0.3f);

        // 2. Enemy Name Text (above the bar)
        GameObject nameObj = new GameObject("NameText");
        nameObj.transform.SetParent(uiRoot.transform, false);
        nameText = nameObj.AddComponent<TextMeshProUGUI>();
        
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = Color.white;
        nameText.fontSize = size.y * 0.6f;
        nameText.textWrappingMode = TextWrappingModes.NoWrap;

        RectTransform nameRect = nameObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.5f, 1f);
        nameRect.anchorMax = new Vector2(0.5f, 1f);
        nameRect.pivot = new Vector2(0.5f, 1f);
        nameRect.anchoredPosition = new Vector2(0, 0);
        nameRect.sizeDelta = new Vector2(size.x, size.y * 0.5f);

        if (enemyStats != null)
        {
            nameText.text = enemyStats.enemyName;
        }
        else
        {
            nameText.text = "Enemy";
        }

        // 3. Background Bar
        GameObject bg = new GameObject("BG");
        bg.transform.SetParent(uiRoot.transform, false);
        bgImg = bg.AddComponent<Image>();
        bgImg.color = backgroundColor;
        bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.5f, 0.5f);
        bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.sizeDelta = size;
        bgRect.anchoredPosition = new Vector2(0, -0.15f);

        // 4. Foreground (Health Fill)
        GameObject fg = new GameObject("FG");
        fg.transform.SetParent(bg.transform, false);
        fgImg = fg.AddComponent<Image>();
        fgImg.color = foregroundColor;
        fgRect = fg.GetComponent<RectTransform>();
        fgRect.anchorMin = new Vector2(0, 0);
        fgRect.anchorMax = new Vector2(1, 1);
        fgRect.pivot = new Vector2(0f, 0.5f); 
        fgRect.sizeDelta = Vector2.zero; 
        fgRect.localScale = new Vector3(1f, 1f, 1f);

        // 5. Level Text
        GameObject lt = new GameObject("LevelText");
        lt.transform.SetParent(bg.transform, false);
        levelText = lt.AddComponent<TextMeshProUGUI>();
        
        levelText.alignment = TextAlignmentOptions.Right; 
        levelText.color = levelTextColor;
        levelText.fontSize = size.y * 0.8f; 
        levelText.textWrappingMode = TextWrappingModes.NoWrap;

        RectTransform ltrt = lt.GetComponent<RectTransform>();
        ltrt.anchorMin = new Vector2(0f, 0.5f);
        ltrt.anchorMax = new Vector2(0f, 0.5f);
        ltrt.pivot = new Vector2(1f, 0.5f);
        ltrt.anchoredPosition = new Vector2(-0.05f, 0f);
        ltrt.sizeDelta = new Vector2(1.0f, size.y);

        // 6. HP Text (centered on the bar)
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

        // 7. Add Billboard Script
        uiRoot.AddComponent<FaceCamera>();

        if (damageable != null)
            UpdateBar(damageable.Health, damageable.MaxHealth);

        UpdateLevelText();
        UpdateLevelGapColor();
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

        // Update level gap color in case player level changes
        UpdateLevelGapColor();
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

    private void UpdateLevelGapColor()
    {
        if (enemyStats == null || bgImg == null || fgImg == null)
            return;

        // Get player level
        ExpSystem expSystem = GameObject.Find("Player")?.GetComponent<ExpSystem>();
        if (expSystem == null)
            return;

        int playerLevel = expSystem.level;
        EnemyStats.LevelGapColor gapColor = enemyStats.GetLevelGapColor(playerLevel);
        
        Color barColor;
        Color textColor;

        switch (gapColor)
        {
            case EnemyStats.LevelGapColor.Green:
                barColor = new Color(0f, 1f, 0f, 0.8f);  // Bright green
                textColor = Color.green;
                break;
            case EnemyStats.LevelGapColor.Yellow:
                barColor = new Color(1f, 1f, 0f, 0.8f);  // Bright yellow
                textColor = Color.yellow;
                break;
            case EnemyStats.LevelGapColor.Red:
                barColor = new Color(1f, 0f, 0f, 0.8f);  // Bright red
                textColor = Color.red;
                break;
            default:
                barColor = new Color(1f, 1f, 0f, 0.8f);
                textColor = Color.yellow;
                break;
        }

        fgImg.color = barColor;
        levelText.color = textColor;
    }
}