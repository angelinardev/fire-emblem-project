using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{

    public event EventHandler OnHealthChanged;
    public event EventHandler OnDead;

    private int maxHealth;
    private int health;
    private int maxSP;
    private int sp;
    private bool dead = false;

    //player health
    public HealthSystem(int currentHealth, int healthMax, int currentSP, int maxSP)
    {
        this.health = currentHealth;
        this.maxHealth = healthMax;
        this.sp = currentSP;
        this.maxSP = maxSP;
        if (health == 0)
        {
            dead = true;
        }
        else
            dead = false;
    }

    //enemy health
    public HealthSystem(int healthMax)
    {
        this.maxHealth = healthMax;
        health = healthMax;
    }

    public void SetHealthAmount(int health)
    {
        this.health = health;
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthPercent()
    {
        return (float)health / maxHealth;
    }

    public void Damage(int amount)
    {
        health -= amount;
        if(health <=0)
        {
            health = 0;
        }

        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);

        if(health <=0)
        {
            dead = true;
        }
    }

    public void Heal(int hp, int sp)
    {
        health += hp;
        if(health > maxHealth)
        {
            health = maxHealth;
        }

        if (health <= 0)
        {
            health = 0;
        }

        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }

    public bool IsDead()
    {
        if (dead)
            return true;
        else
            return false;
    }
}