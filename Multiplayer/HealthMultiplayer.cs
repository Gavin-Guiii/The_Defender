using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Base class to be inherited
public class HealthMultiplayer : NetworkBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Add health
    public virtual void IncrementHealth(int value)
    {
        currentHealth += value;
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    // Get hurt
    public virtual void DecrementHealth(int value)
    {
        currentHealth -= value;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Die
    public virtual void Die()
    {
        currentHealth = 0;
    }

    // Revive
    public void Revive()
    {
        currentHealth = maxHealth;
    }

    // Change health
    public virtual void OnHealthChange(int oldHP, int newHP)
    {
        currentHealth = newHP;
    }
}
