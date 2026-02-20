using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKill : MonoBehaviour
{
    ExpSystem expSystem;

    public GameObject[] itemDrops;
    public float[] dropChances;
    public int experience;
    
    [Header("Level-Based EXP")]
    [Tooltip("If true, calculates exp based on enemy level. If false, uses fixed experience value.")]
    public bool useLevelBasedExp = true;
    [Tooltip("Base EXP to multiply by level. E.g., level 1 = 5 EXP, level 2 = 10 EXP, level 3 = 15 EXP")]
    public int baseExpPerLevel = 5;

    public void Awake()
    {
        expSystem = GameObject.Find("Player").GetComponent<ExpSystem>();
        
        // Calculate exp based on enemy level if enabled
        if (useLevelBasedExp)
        {
            EnemyStats stats = GetComponent<EnemyStats>();
            if (stats != null && stats.level > 0)
            {
                experience = baseExpPerLevel * stats.level;
            }
        }
    }

    public void KillEnemy()
    {
        expSystem.IncreaseExp(experience);
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    public void ItemDrop()
    {
        for (int i = 0; i < itemDrops.Length; i++)
        {
            if (Random.value <= dropChances[i])
            {
                Instantiate(itemDrops[i], transform.position, Quaternion.identity);
            }
        }
    }
}
