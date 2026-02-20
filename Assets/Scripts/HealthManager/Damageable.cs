using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Damageable and HealthManager Script work together
public class Damageable : MonoBehaviour
{
    Animator animator;
    Attack playerAttack;

    [SerializeField] private bool _isAlive = true;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _health = 100;
    private float timeSinceHit = 0;
    public float invincibilityTime = 1f;

    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent<int, int> healthChanged;
    public UnityEvent<int, int> maxHealthChanged;


    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set " +  value);
        }
    }

    // Velocity should not be changed while LockVelocity is true, but needs to be respected by other physics components like PlayerController
    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
            maxHealthChanged?.Invoke(Health, _maxHealth);
        }
    }

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, MaxHealth);

            if(_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    public bool Hit(int damage, Vector2 knockback)
    {
        if(IsAlive && !isInvincible)
        {
            Health -= damage;

            // Health can't drop below 0
            Health = Mathf.Max(0, Health);

            isInvincible = true;

            // Notify other components that the damageable was hit to handle the knockback etc.
            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            
            // CharacterEvents of the damage is handled in Attack and EnemyAttack scripts.
            damageableHit?.Invoke(damage, knockback);

            return true;
        }

        return false;
    }

    public void Heal(int healthRestore)
    {
        if(IsAlive)
        {
            int maxHealthRestore = Mathf.Max(MaxHealth - Health, 0);
            int successfulHealthRestore = Mathf.Min(maxHealthRestore, healthRestore);
            Health += successfulHealthRestore;
            CharacterEvents.characterHealed(gameObject, successfulHealthRestore);
        }
    }

    public void IncreaseMaxHealth(int increase)
    {
        if (IsAlive)
        {
            MaxHealth += increase;
        }
    }

    public void DecreaseMaxHealth(int decrease)
    {
        if (IsAlive)
        {
            MaxHealth -= decrease;
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerAttack = GameObject.Find("Player").GetComponentInChildren<Attack>();
        // If this GameObject has EnemyStats, initialize level/hp for World Demo 1-1
        EnemyStats stats = GetComponent<EnemyStats>();
        if (stats != null)
        {
            // If we're in World Demo 1-1, ensure enemies range from level 1..3
            if (SceneManager.GetActiveScene().name == "World Demo 1-1")
            {
                if (stats.level < 1)
                {
                    stats.level = Random.Range(1, 4); // 1,2,3
                }
            }

            // Map level to HP (use the values on EnemyStats)
            int assignedHP = stats.hpLevel1;
            switch (stats.level)
            {
                case 1:
                    assignedHP = stats.hpLevel1;
                    break;
                case 2:
                    assignedHP = stats.hpLevel2;
                    break;
                case 3:
                    assignedHP = stats.hpLevel3;
                    break;
            }

            MaxHealth = assignedHP;
            Health = MaxHealth;

            // Add a runtime health bar if missing
            if (GetComponent<EnemyHealthBar>() == null)
            {
                gameObject.AddComponent<EnemyHealthBar>();
            }
        }
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }
}
