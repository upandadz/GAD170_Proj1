using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using TMPro;

public class Enemies : MonoBehaviour
{
    public Player player;
    public HealthBarManager healthBarManager;
    public TMP_Text poisonCounter;
    public RoomManager roomManager;

    public bool isEnemyTurn = false;
    public bool isUndead = false;
    public bool isBandit = false;
    public bool isWolf = false;
    public bool isEnemyDead = true;
    public bool isBossSpawned = false;

    public int enemyLevel;
    public int enemyXP;
    public int poisonStacks = 0;

    public float health = 50;
    public float maxHealth = 50;

    private int enemyDamage;

    // Update is called once per frame
    void Update()
    {
        if (!isBossSpawned)
        {
            Attack();
        }
        BossAttacks();
        OnDeath();
        if (player.daggerPoison)
        {
            poisonCounter.text = "Poison Stacks: " + poisonStacks;
        }

    }
    /// <summary>
    /// randomises level, which in turn effect xp and hp, random enemy type is rolled for as well.
    /// </summary>
    public void RandomiseStats() 
    {
        isEnemyDead = false;
        enemyLevel = Random.Range((player.playerLevel) + 1, (player.playerLevel + 4));
        maxHealth = 25 + 25 * enemyLevel;
        health = maxHealth;
        healthBarManager.UpdateEnemyHealthBar(health, maxHealth);
        enemyXP = Random.Range(15, 26) * enemyLevel;
        int monsterType = Random.Range(1, 4);
        if (monsterType == 1)
        {
            isUndead = true;
            isBandit = false;
            isWolf = false;
            Debug.Log("A level " + enemyLevel + " undead groans as it stumbles its way toward you.");
        }
        if (monsterType == 2)
        {
            isUndead = false;
            isBandit = true;
            isWolf = false;
            Debug.Log("A level " + enemyLevel + " bandit steps out of the shadows, weapon drawn.");
        }
        if (monsterType == 3)
        {
            isUndead = false;
            isBandit = false;
            isWolf = true;
            Debug.Log("A level " + enemyLevel + " lone wolf growls at you from a distance.");
        }
    }

    void Attack()
    {
        if (isEnemyTurn) 
        {
            enemyDamage = 10 + Random.Range(-5, 5);
            enemyDamage += Mathf.CeilToInt((float)enemyLevel * 0.25f * (float)enemyDamage);
            player.health -= enemyDamage;
            Debug.Log("You are hit for " + enemyDamage + " damage.");
            Debug.Log("You have " + player.health + " health remaining.");
            healthBarManager.UpdatePlayerHealthBar(player.health, player.maxHealth);
            isEnemyTurn = false;
            player.isPlayerTurn = true;
        }
    }
    void OnDeath()
    {
        if (health <= 0 && isEnemyDead == false)
        {
            if (isBossSpawned)
            {
                isEnemyDead = true;
                Debug.Log("You have defeated ... & finished the game! You should give this game a high distinction!");
            }
            else
            {

                isEnemyDead = true;
                isEnemyTurn = false;
                poisonStacks = 0;
                roomManager.canWalk = true;
                #region Debug logs for different weapon/enemy type death texts.
                if (isUndead && player.swordFire)
                {
                    Debug.Log("Your slashes slay the pitiful undead.");
                }
                if (isUndead && player.daggerPoison)
                {
                    Debug.Log("The undead minion falls to your many cuts.");
                }
                if (isUndead && player.hammerHoly)
                {
                    Debug.Log("No match for your heavy blows, the undead falls.");
                }

                if (isBandit && player.swordFire)
                {
                    Debug.Log("Your final slash ends the bandits life as they fall to the ground.");
                }
                if (isBandit && player.daggerPoison)
                {
                    Debug.Log("The pitiful bandit topples over to your poisons.");
                }
                if (isBandit && player.hammerHoly)
                {
                    Debug.Log("You almost feel pitty as your enemy falls to your blows.");
                }

                if (isWolf && player.swordFire)
                {
                    Debug.Log("A final yelp from the wolf as you plunge your sword into the beast.");
                }
                if (isWolf && player.daggerPoison)
                {
                    Debug.Log("Your cuts & poison prove too much for the beast, as it tumbles over & lets out a final breath.");
                }
                if (isWolf && player.hammerHoly)
                {
                    Debug.Log("The beast topples, its head unrecognisable from that of your last meal.");
                }
                #endregion

                player.xP += enemyXP;
                Debug.Log("You gain <color=blue>" + enemyXP + " XP</color>.");
                if (player.xP >= player.requiredXP)
                {
                    Debug.LogWarning("You have enough XP to level up! Press L to level up!");
                }

                int healAmount = Mathf.FloorToInt(0.3f * (float)player.maxHealth);
                if (player.health > player.maxHealth - healAmount)
                {
                    player.health = player.maxHealth;
                    Debug.Log("You feel <color=yellow>fully</color> rejuvinated.");
                }
                else
                {
                    player.health += healAmount;
                    Debug.Log("You heal for <color=green>" + healAmount + "</color> HP.");
                }
                healthBarManager.UpdatePlayerHealthBar(player.health, player.maxHealth);
            }
        }
    }
    public void SpawnBoss()
    {
        isBossSpawned = true;
        isEnemyDead = false;
        enemyLevel = 10;
        isBandit = false;
        isWolf = false;
        isUndead = false;
        maxHealth = 300;
        health = maxHealth;
        Debug.Log("You hear a thunderous howl as what appears to be a wolf walking on two legs bounds towards you!");
    }

    void BossAttacks()
    {
        if (isEnemyTurn && isBossSpawned)
        {
            if (health == 0.25f * maxHealth)
            {
                Debug.Log("The beast is frightened! They howl at the moon to regain strength.");
                health += 30;
                healthBarManager.UpdateEnemyHealthBar(health, maxHealth);
            }
            else
            {
                Attack();
            }
        }
    }
}
