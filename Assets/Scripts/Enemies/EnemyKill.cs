using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKill : MonoBehaviour
{
    ExpSystem expSystem;

    public GameObject[] itemDrops;
    public float[] dropChances;
    public int experience;
    
    [Header("EXP Formula")]
    [Tooltip("If true, uses formula: EXP = 12 × level² × typeMultiplier. If false, uses fixed experience value.")]
    public bool useFormulaExp = true;
    [Tooltip("Fixed EXP when useFormulaExp is false.")]
    public int fixedExperience = 50;

    [Header("Level Gap EXP Balancing")]
    [Tooltip("If true, reduces exp to 15% when player is 4+ levels above enemy (Green).")]
    public bool useLevelGapBalancing = true;
    [Tooltip("Level difference before exp is reduced to 15%. E.g., 4 = player 4+ levels above.")]
    public int levelGapThreshold = 4;
    [Tooltip("EXP multiplier when player is too far above (e.g. 0.15 = 15%).")]
    [Range(0f, 1f)]
    public float expReductionMultiplier = 0.15f;

    public void Awake()
    {
        expSystem = GameObject.Find("Player").GetComponent<ExpSystem>();
        
        if (useFormulaExp)
        {
            EnemyStats stats = GetComponent<EnemyStats>();
            if (stats != null && stats.level > 0)
            {
                // EXP = 12 × level² × typeMultiplier
                experience = Mathf.RoundToInt(stats.GetBaseExp() * stats.GetTypeMultiplier());
                
                if (useLevelGapBalancing)
                {
                    ExpSystem playerExpSystem = GameObject.Find("Player").GetComponent<ExpSystem>();
                    if (playerExpSystem != null)
                    {
                        int levelDifference = playerExpSystem.level - stats.level;
                        if (levelDifference >= levelGapThreshold)
                        {
                            experience = Mathf.Max(1, Mathf.RoundToInt(experience * expReductionMultiplier));
                        }
                    }
                }
            }
            else
            {
                experience = fixedExperience;
            }
        }
        else
        {
            experience = fixedExperience;
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
