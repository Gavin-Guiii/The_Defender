using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using ARLocation;
using DG.Tweening;

// Enemy's health. Inheriting from the base "HealthMultiplayer"
public class EnemyMultiplayer : HealthMultiplayer
{
    public float animTime = 0.3f;

    public GameObject Smoke;
    public float destroyTime = 2f;

    private Animator anim;
    private Slider HPSlider;

    // When the client first starts
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isClientOnly)
        {
            transform.parent = FindObjectOfType<PlaceAtLocation>().transform;
            transform.localPosition = new Vector3(0f, transform.position.y, 0f);
            transform.localRotation = Quaternion.identity;
        }
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;
        HPSlider = GetComponentInChildren<Slider>();
        if (HPSlider != null)
        {
            HPSlider.value = currentHealth / maxHealth;
        }
    }

    // Overriding the base DecrementHealth() function
    // Only the player has authority to do that. So it calls the player's damageNetworkActor() function.
    public override void DecrementHealth(int value)
    {
        GameObject.Find("Local").GetComponent<PlayerHealthMultiplayer>().damageNetworkActor(value, netId, false);
    }

    // Overriding the base Die() function
    public override void Die()
    {
        base.Die();
        // Play the Die animation
        Instantiate(Smoke, transform.position, Quaternion.identity);
        Destroy(gameObject, destroyTime);
        EnemyManagerMultiplayer.instance.decrementEnemyCount();
    }

    // Overriding the base OnHealthChange() function
    public override void OnHealthChange(int oldHP, int newHP)
    {
        base.OnHealthChange(oldHP, newHP);
        if (HPSlider != null)
        {
            HPSlider.DOValue((float)currentHealth / maxHealth, animTime);
        }
    }

    // If the other player hurts the enemy, server will call this ClientRpc function so that the enemy's health will also be updated here
    [ClientRpc]
    public void RpcDecreHealth(int value)
    {
        base.DecrementHealth(value);
        if (HPSlider != null)
        {
            HPSlider.DOValue((float)currentHealth / maxHealth, animTime);
        }
    }
}
