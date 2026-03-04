using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private PlayerStats playerStats;
    private EnemyStats enemyStats;

    public int attackDamage = 10;
    private float randomMultiplier = 1f;
    public Vector2 knockback = Vector2.zero;
    
    private void Start()
    {
        playerStats = GameObject.Find("StatManager").GetComponent<PlayerStats>();
        enemyStats = GetComponentInParent<EnemyStats>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            if (playerStats == null)
                playerStats = GameObject.Find("StatManager")?.GetComponent<PlayerStats>();

            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            randomMultiplier = Random.Range(0.8f, 1.2f);
            
            float statMultiplier = 1f;
            if (enemyStats != null)
            {
                ExpSystem expSystem = GameObject.Find("Player")?.GetComponent<ExpSystem>();
                if (expSystem != null)
                    statMultiplier = enemyStats.GetStatMultiplier(expSystem.level);
            }

            int finalAttackDamage = Mathf.RoundToInt(attackDamage * statMultiplier);
            float defense = (playerStats != null) ? (0.8f * playerStats.defense) : 0f;
            int damageDealt = Mathf.Max(0, Mathf.RoundToInt((finalAttackDamage * randomMultiplier) - defense));

            bool gotHit = damageable.Hit(damageDealt, deliveredKnockback);
            if (gotHit)
            {
                Debug.Log(collision.name + " hit for " + damageDealt);
                CharacterEvents.characterDamaged.Invoke(gameObject, damageDealt);
            }
        }
    }
}
