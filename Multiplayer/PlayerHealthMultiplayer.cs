using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using DG.Tweening;

// Player's health. Inheriting from the base "HealthMultiplayer"
public class PlayerHealthMultiplayer : HealthMultiplayer
{
    public bool hasOtherPlayerDied;
    private SecondaryHealthManager secondaryHealthManager;
    private void Start()
    {
        gameObject.transform.SetParent(Camera.main.gameObject.transform);
        secondaryHealthManager = FindObjectOfType<SecondaryHealthManager>();
        if (isLocalPlayer)
        {
            currentHealth = maxHealth;
            gameObject.name = "Local";
            ShootUIManager.Instance.SetMaxHealth(maxHealth);
            ShootUIManager.Instance.SetCurrentHealth(currentHealth);
        }
        else
        {
            gameObject.name = "Other";
            Destroy(GetComponent<BoxCollider>());
        }
    }
    
    private void Update()
    {
        if (!isLocalPlayer)
            return;
    }

    // Overriding the base Die() function
    public override void Die()
    {
        base.Die();
        FindObjectOfType<SecondaryHealthManager>().hasMainPlayerDied = true;

        // If local player dies, check whether the other player is alive
        if (secondaryHealthManager.currentHealth <= 0)
        {
            // If the other player has dead, they lose this game
            ShootingManager.instance.Lose();
        }
        else
        {
            // If the other player is still alive, tell local player "You still have chance"
            ShootUIManager.Instance.ShowMultiplayerDiePrompt();
        }
        ShootUIManager.Instance.DisableAllButtons();
        Destroy(GetComponent<BoxCollider>());
    }

    // Overriding the base IncrementHealth() function
    public override void IncrementHealth(int value)
    {
        AddHealthOnNetworkActor(value, netId);
    }

    // Overriding the base DecrementHealth() function
    public override void DecrementHealth(int value)
    {
        damageNetworkActor(value, netId, true);
    }

    // Hurt an actor in multiplayer environment
    public void damageNetworkActor(int damage, uint id, bool isPlayer)
    {
        CmdTakeDamage(damage, id, isPlayer);
        if (!isPlayer)
        {
            BroadCastMessageFrom(netId, "Emeny HP -" + damage + " by other player!");
        }
        else
        {
            BroadCastMessageFrom(netId, "Other player HP -" + damage + " by enemy!");
        }
    }

    // Add an actor's health in multiplayer environment
    public void AddHealthOnNetworkActor(int value, uint id)
    {
        CmdAddHealth(value, id);
        BroadCastMessageFrom(netId, "Other player HP +" + value + "!");
    }

    // Implement the "Add Time" button
    public void ExtentTime()
    {
        // Add remaining time on both local and network clients
        CmdExtentTime(netId);

        // Show the message only on network client
        BroadCastMessageFrom(netId, "Other player adds more time!");
    }

    // When players win the game
    public void ShowWinPanel()
    {
        // Show the win panel on both local and network clients
        CmdShowWinPanel(netId);
    }

    // Show message only on network client
    public void BroadCastMessageFrom(uint id, string message)
    {
        CmdBroadcastMessageFrom(id, message);
    }

    // Call clients' ClientRpc functions to update health
    [Command]
    public void CmdTakeDamage(int damage, uint id, bool isPlayer)
    {
        if (!isPlayer)
        {
            NetworkClient.spawned[id].GetComponent<EnemyMultiplayer>().RpcDecreHealth(damage);
        }
        else
        {
            NetworkClient.spawned[id].GetComponent<PlayerHealthMultiplayer>().RpcDecreHealth(damage);
        }
    }

    // Call clients' ClientRpc functions to add health
    [Command]
    public void CmdAddHealth(int value, uint id)
    {
        NetworkClient.spawned[id].GetComponent<PlayerHealthMultiplayer>().RpcIncreHealth(value);
    }

    // Call clients' ClientRpc functions to update remaining time
    [Command]
    public void CmdExtentTime(uint id)
    {
        NetworkClient.spawned[id].GetComponent<PlayerHealthMultiplayer>().RpcExtentTime();
    }

    // Call clients' ClientRpc functions to show win panel
    [Command]
    public void CmdShowWinPanel(uint id)
    {
        NetworkClient.spawned[id].GetComponent<PlayerHealthMultiplayer>().RpcShowWinPanel();
    }

    // Call clients' ClientRpc functions to show the message
    [Command]
    public void CmdBroadcastMessageFrom(uint id, string message)
    {
        RpcBroadcastMessageFrom(id, message);
    }

    // As a client, if the other player gets hurt, server will call this ClientRpc function so that player's health will also be updated here
    [ClientRpc]
    public void RpcDecreHealth(int value)
    {
        if (isLocalPlayer)
        {
            base.DecrementHealth(value);
            GetComponent<AudioSource>().Play();
            ShootUIManager.Instance.SetCurrentHealth(currentHealth);
            ShootUIManager.Instance.ShowHurtUI();
        }
        else
        {
            secondaryHealthManager.DecrementHealth(value);
        }
    }

    // As a client, if the other player adds health, server will call this ClientRpc function so that player's health will also be updated here
    [ClientRpc]
    public void RpcIncreHealth(int value)
    {
        if (isLocalPlayer)
        {
            base.IncrementHealth(value);
            ShootUIManager.Instance.SetCurrentHealth(currentHealth);
        }
        else
        {
            secondaryHealthManager.IncrementHealth(value);
        }
    }

    // As a client, if the other player adds time, server will call this ClientRpc function so that remaining time will also be updated here
    [ClientRpc]
    public void RpcExtentTime()
    {
        ShootingManager.instance.ExtentTime();
    }

    // As a client, if players win the game, server will call this ClientRpc function to show the win panel
    [ClientRpc]
    public void RpcShowWinPanel()
    {
        ShootingManager.instance.Win();
    }

    // As a client, if the other player sends a message, server will call this ClientRpc function to show the message here
    [ClientRpc]
    public void RpcBroadcastMessageFrom(uint id, string message)
    {
        if (NetworkClient.spawned[id].gameObject.name != "Local")
        {
            StartCoroutine(ShootUIManager.Instance.SetMultiplayerPrompt(message));
        }
    }
}
