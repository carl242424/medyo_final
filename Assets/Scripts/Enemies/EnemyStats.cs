using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Identity")]
    public string enemyName = "Enemy";

    [Header("Level & Type")]
    [Tooltip("If 0, level may be assigned by the spawner/scene logic (World Demo 1-1).")]
    public int level = 0;
    [Tooltip("Enemy type affects HP/EXP: Normal=1x, Elite=1.5x, MiniBoss=2x, Boss=4x")]
    public EnemyType enemyType = EnemyType.Normal;

    [Header("Base Combat Stats")]
    public int baseAttack = 10;
    public int baseDefense = 0;

    public float fireResistance;
    public float windResistance;
    public float electricResistance;

    [Header("Level Gap Modifiers (Player vs Enemy)")]
    [Tooltip("Green: Player 2+ above. Lower = easier (enemy takes more damage, has less HP).")]
    public float greenDamageMultiplier = 1.2f;  // Player deals 20% more
    public float greenHPMultiplier = 0.8f;     // Enemy has 80% HP
    [Tooltip("Yellow: Even fight. Normal stats.")]
    public float yellowMultiplier = 1.0f;
    [Tooltip("Red: Player 4+ below. Higher = harder (enemy takes less damage, has more HP, deals more).")]
    public float redDamageMultiplier = 0.8f;    // Player deals 80% damage
    public float redHPMultiplier = 1.2f;       // Enemy has 120% HP
    public float redAttackMultiplier = 1.2f;   // Enemy deals 20% more damage

    // Current level gap color
    private LevelGapColor _currentLevelGapColor = LevelGapColor.Yellow;

    public enum EnemyType
    {
        Normal = 0,   // 1x
        Elite = 1,    // 1.5x
        MiniBoss = 2, // 2x
        Boss = 3      // 4x
    }
    
    public enum LevelGapColor
    {
        Green,  // Player 2+ levels above
        Yellow, // Player 1+ levels above, same level
        Red     // Player 4+ levels below
    }

    public LevelGapColor GetLevelGapColor(int playerLevel)
    {
        int levelDifference = playerLevel - level;

        if (levelDifference >= 2)
        {
            return LevelGapColor.Green;
        }
        else if (levelDifference >= -3)
        {
            return LevelGapColor.Yellow;
        }
        else
        {
            return LevelGapColor.Red;
        }
    }

    /// <summary>Used when enemy attacks player. Red = enemy deals more, Green = enemy deals less.</summary>
    public float GetStatMultiplier(int playerLevel)
    {
        _currentLevelGapColor = GetLevelGapColor(playerLevel);
        return GetAttackMultiplier();
    }

    /// <summary>Multiplier for damage player deals to enemy. Green = more, Red = less.</summary>
    public float GetDamageMultiplier(int playerLevel)
    {
        _currentLevelGapColor = GetLevelGapColor(playerLevel);
        switch (_currentLevelGapColor)
        {
            case LevelGapColor.Green: return greenDamageMultiplier;
            case LevelGapColor.Yellow: return yellowMultiplier;
            case LevelGapColor.Red: return redDamageMultiplier;
            default: return yellowMultiplier;
        }
    }

    /// <summary>Multiplier for enemy HP. Green = less, Red = more.</summary>
    public float GetHPMultiplier(int playerLevel)
    {
        _currentLevelGapColor = GetLevelGapColor(playerLevel);
        switch (_currentLevelGapColor)
        {
            case LevelGapColor.Green: return greenHPMultiplier;
            case LevelGapColor.Yellow: return yellowMultiplier;
            case LevelGapColor.Red: return redHPMultiplier;
            default: return yellowMultiplier;
        }
    }

    private float GetAttackMultiplier()
    {
        switch (_currentLevelGapColor)
        {
            case LevelGapColor.Green: return 0.8f;  // Enemy deals 80%
            case LevelGapColor.Yellow: return yellowMultiplier;
            case LevelGapColor.Red: return redAttackMultiplier;
            default: return yellowMultiplier;
        }
    }

    public float GetTypeMultiplier()
    {
        switch (enemyType)
        {
            case EnemyType.Normal: return 1f;
            case EnemyType.Elite: return 1.5f;
            case EnemyType.MiniBoss: return 2f;
            case EnemyType.Boss: return 4f;
            default: return 1f;
        }
    }

    /// <summary>Base HP from formula: 25 + (level × 12)</summary>
    public int GetBaseHP()
    {
        return 25 + (level * 12);
    }

    /// <summary>Base EXP from formula: 12 × level²</summary>
    public int GetBaseExp()
    {
        return 12 * level * level;
    }

    public LevelGapColor GetCurrentLevelGapColor()
    {
        return _currentLevelGapColor;
    }

    public Color GetLevelGapColorValue()
    {
        switch (_currentLevelGapColor)
        {
            case LevelGapColor.Green:
                return Color.green;
            case LevelGapColor.Yellow:
                return Color.yellow;
            case LevelGapColor.Red:
                return Color.red;
            default:
                return Color.white;
        }
    }

    public int ModifyDamage(int damage, DamageType damageType)
    {
        float resistance = 0f;

        switch (damageType)
        {
            case DamageType.Fire:
                resistance = fireResistance;
                break;
            case DamageType.Wind:
                resistance = windResistance;
                break;
            case DamageType.Electric:
                resistance = electricResistance;
                break;
        }
        int modifiedDamage = Mathf.RoundToInt(damage * (1 - resistance));

        return modifiedDamage;
    }

    public enum DamageType
    {
        Fire,
        Wind,
        Electric,
    }
}
