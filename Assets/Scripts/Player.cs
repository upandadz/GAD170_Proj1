using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

public class Player : MonoBehaviour
{
    public Enemies enemy;
    public HealthBarManager healthBarManager;
    public GameManager gameManager;
    public RoomManager roomManager;
    public Image xPBarFill;
    public TMP_Text chargeCounter;
    public TMP_Text chargedCounter;
    public TMP_Text perkAvailable;
    public TMP_Text levelUpText;
    public TMP_Text quick;
    public TMP_Text normal;
    public TMP_Text charges;
    public TMP_Text lucky;
    public TMP_Text playerLvlText;
    public TMP_Text enemyLvlText;

    public bool isPlayerTurn = false;
    public bool swordFire = false;
    public bool daggerPoison = false;
    public bool hammerHoly = false;
    public bool canAttack = false;
    public bool isAlive = true;

    public int playerLevel = 0;
    public int storedCharges = 2;
    public int chargeRegen = 2;
    public int maxStored = 6;

    public float xP = 0;
    public float requiredXP = 100;
    public float health = 150f;
    public float maxHealth = 150f;

    private int weaponChoice = 1;
    private int passivePoints = 0;

    private int baseDamage;
    private int elementDamage;
    private int totalDamage;

    private int chargedCharges = 0;
    private int maxCharged = 4;

    private bool didCrit = false;
    private bool hasAttacked = false;


    private bool passiveQuick = false;
    private bool passiveNormal = false;
    private bool passiveCharges = false;
    public bool passiveLuck = false;


    void Start()
    {

    }

    void Update()
    {
        OnScreenText();
        PickWeapons();
        XPBar();

        // if you have selected a weapon type and an enemy is dead, spawn a mob & roll for first turn
        if ((swordFire || daggerPoison || hammerHoly) && enemy.isEnemyDead && Input.GetKeyDown(KeyCode.Space) && playerLevel < 4)
        {
            enemy.RandomiseStats();
            gameManager.FirstTurnRoll();
        }
        else if (playerLevel == 4 && enemy.isEnemyDead && Input.GetKeyDown(KeyCode.Space))
        {
            enemy.SpawnBoss();
            gameManager.FirstTurnRoll();
        }

        if (passiveCharges)
        {
            chargeRegen = 3;
        }

        SwordAttacks();
        DaggersAttacks();
        HammerAttacks();
        EndTurn();
        LevelUp();
        PerkPick();
    }

    void PickWeapons() // allows the player to press a button to choose a weapon at the start
    {
        if (weaponChoice == 1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                weaponChoice = 0;
                swordFire = true;
                Debug.Log("You choose the sword, it feels almost <color=red>hot</color> to the touch.");
                Debug.Log("A message seems to burn its way into your head -");
                Debug.Log("Press <color=yellow>i</color> for more information.");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                weaponChoice = 0;
                daggerPoison = true;
                Debug.Log("As you pick the daggers up you notice a <color=green>sticky substance</color> on the blade, it doesn't seem to rub off.");
                Debug.Log("A message comes to you as if dripping into your mind -");
                Debug.Log("Press <color=yellow>i</color> for more information.");

            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                weaponChoice = 0;
                hammerHoly = true;
                Debug.Log("You brace to pick up the warhammer but to your suprise it does not feel heavy.");
                Debug.Log("A voice speaks to you from <color=yellow>all directions</color> -");
                Debug.Log("Press <color=yellow>i</color> for more information.");
            }
        }
        
    }

    void SwordAttacks() // Attack list for sword type
    {
        if (swordFire == true)
        {
            ChargeAttack();
            Attacks(5, 4, 10, 10, 15, 25);
        }
    }

    void DaggersAttacks() // Attack list for daggers type
    {
        if (daggerPoison == true)
        {
            ChargeAttack();
            Attacks(3, 4, 7, 10, 20, 20);
        }
    }

    void HammerAttacks() // Attack list for Hammer type
    {
        if (hammerHoly == true)
        {
            ChargeAttack();
            Attacks(5, 5, 8, 14, 35, 35);
        }
    }

    void ChargeAttack()
    {
        if (isAlive && enemy.isEnemyDead == false)
        {
            if ((chargedCharges == 2 && storedCharges < 2) || storedCharges == 0 || chargedCharges == maxCharged) // stops player wasting charges if they don't have enough to charges for a special attack or have enough charges in general
            {
                return;
            }
            else if (Input.GetKeyDown(KeyCode.A) && storedCharges >= 1 && isPlayerTurn && canAttack)
            {
                storedCharges -= 1;
                chargedCharges++;
                Debug.Log("You charge up, total charges: " + chargedCharges + '.');
            }
        }
    }

    void Attacks(int quickBase, int quickElement, int normalBase, int normalElement, int specialBase, int specialElement)
    {
        if (isPlayerTurn && enemy.isEnemyDead == false && canAttack && isAlive)
        {
            if (Input.GetKeyDown(KeyCode.Return) && chargedCharges == 1) // Quick Attack
            {
                chargedCharges -= 1;
                baseDamage = quickBase;
                elementDamage = quickElement;
                DamageModifier();
                CritStrike();
                SwordHeal();
                DamageEnemy();
                healthBarManager.UpdateEnemyHealthBar(enemy.health, enemy.maxHealth);
                Debug.Log("You quickly strike for <color=red>" + totalDamage + "</color> damage.");
                if (daggerPoison)
                {
                    PoisonStack();
                }
                if (didCrit)
                {
                    Debug.LogError("You critically strike!");
                }
                Debug.Log("They have <color=red>" + enemy.health + "</color> health remaining.");
                QuickPassive();

            }
            if (Input.GetKeyDown(KeyCode.Return) && chargedCharges == 2) // Normal Attack
            {
                chargedCharges -= 2;
                baseDamage = normalBase;
                elementDamage = normalElement;
                DamageModifier();
                if (passiveNormal)
                {
                    baseDamage = Mathf.CeilToInt(1.5f * baseDamage);
                    elementDamage = Mathf.CeilToInt(1.5f * elementDamage);
                }
                CritStrike();
                SwordHeal();
                DamageEnemy();
                healthBarManager.UpdateEnemyHealthBar(enemy.health, enemy.maxHealth);
                Debug.Log("You swing at them for <color=red>" + totalDamage + "</color> damage.");
                if (daggerPoison)
                {
                    PoisonStack();
                }
                if (didCrit)
                {
                    Debug.LogError("You critically strike!");
                }
                Debug.Log("They have <color=red>" + enemy.health + "</color> health remaining.");
                QuickPassive();
            }
            if (Input.GetKeyDown(KeyCode.Return) && chargedCharges == 4) // Special Attack
            {
                int poisonDamage = (3 * playerLevel) * enemy.poisonStacks;
                chargedCharges -= 4;
                baseDamage = specialBase;
                elementDamage = specialElement;
                DamageModifier();
                CritStrike();
                SwordHeal();
                DamageEnemy();
                if (enemy.isBandit)
                {
                    poisonDamage *= 2;
                }
                enemy.health -= poisonDamage;
                healthBarManager.UpdateEnemyHealthBar(enemy.health, enemy.maxHealth);

                if (didCrit && (swordFire || hammerHoly))
                {
                    Debug.Log("You lash out with everything for <color=red>" + totalDamage + "</color> damage.");
                    Debug.LogError("You critically strike!");
                }
                else if (swordFire || hammerHoly)
                {
                    Debug.Log("You lash out with everything for <color=red>" + totalDamage + "</color> damage.");
                }
                else
                {
                    Debug.Log("You slash as them with a flurry of strikes!");
                    Debug.Log("Your poisons boil in their blood for <color=green>" + poisonDamage + "</color> damage.");
                    enemy.poisonStacks += 5;
                }
                Debug.Log("They have <color=red>" + enemy.health + "</color> health remaining.");
                QuickPassive();
            }
        }
    }
    /// <summary>
    /// changes damage based on level AND modifies element damage based on what enemy you're fighting
    /// </summary>
    void DamageModifier() 
    {
        baseDamage = baseDamage + Mathf.CeilToInt(baseDamage * (0.25f * (float)playerLevel));
        elementDamage = elementDamage + Mathf.CeilToInt(elementDamage * (0.25f * (float)playerLevel));

        if (swordFire && enemy.isWolf)
        {
            elementDamage *= 2;
        }
        if (daggerPoison && enemy.isBandit)
        {
            elementDamage *= 2;
        }
        if (hammerHoly && enemy.isUndead)
        {
            elementDamage *= 2;
        }
    }
    /// <summary>
    /// Press L to level up, also shows text on screen when enough xp to level up
    /// </summary>
    void LevelUp()
    {
        if (Input.GetKeyDown(KeyCode.L) && enemy.isEnemyDead && xP >= requiredXP && playerLevel == 3)
        {
            xP -= xP;
            requiredXP += 100;
            playerLevel++;
            passivePoints++;
            maxHealth += 60;
            health = maxHealth;
            healthBarManager.UpdatePlayerHealthBar(health, maxHealth);
            Debug.Log("You have gained a perk point.");
            Debug.Log("Press 1 for the Quick perk, 2 for the Normal attack perk, 3 to gain 3 charges per round, or 4 for the Luck perk.");
            Debug.Log("You are now level 5, defeat the boss before you finish the game.");
        }
        else if (Input.GetKeyDown(KeyCode.L) && enemy.isEnemyDead && xP >= requiredXP)
        {
            xP -= requiredXP;
            requiredXP += 100;
            playerLevel++;
            passivePoints++;
            maxHealth += 60;
            health = maxHealth;
            healthBarManager.UpdatePlayerHealthBar(health, maxHealth);
            Debug.Log("You are now level <color=yellow>" + (playerLevel + 1) + "</color>.");
            Debug.Log("You have gained a perk point.");
            Debug.Log("Press 1 for the Quick perk, 2 for the Normal attack perk, 3 to gain 3 charges per round, or 4 for the Luck perk.");
        }
    }

    void PerkPick()
    {
        if (passivePoints > 0 && enemy.isEnemyDead)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && !passiveQuick)
            {
                passivePoints--;
                passiveQuick = true;
                Debug.Log("You have gained the quick perk, you can now continue to attack as long as you have charges.");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && !passiveNormal)
            {
                passivePoints--;
                passiveNormal = true;
                Debug.Log("You have gained the normal attack perk, your normal attacks now do 1.5 x damage.");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && !passiveCharges)
            {
                passivePoints--;
                passiveCharges = true;
                Debug.Log("You now gain 3 charges per round.");
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && !passiveLuck)
            {
                passivePoints--;
                passiveLuck = true;
                Debug.Log("You have gained the luck perk, you may find everything to be a bit easier now.");
            }
        }
    }

    /// <summary>
    /// If they have the quick passive they can continue to attack untill they end their turn.
    /// </summary>
    void QuickPassive()
    {
        if (passiveQuick == false)
        {
            canAttack = false;
            hasAttacked = true;
        }
        else
        {
            hasAttacked = true;
            canAttack = true;
        }
    }

    /// <summary>
    /// press space to end turn, also regen your charges
    /// </summary>
    void EndTurn()
    {
        if (hasAttacked && Input.GetKeyDown(KeyCode.Space))
        {
            enemy.health -= (playerLevel + 5) * enemy.poisonStacks;
            if (enemy.isBandit)
            {
                enemy.health -= playerLevel + 5 * enemy.poisonStacks; // bandits take 2x poison damage
            }
            healthBarManager.UpdateEnemyHealthBar(enemy.health, enemy.maxHealth);
            hasAttacked = false;
            canAttack = true;
            isPlayerTurn = false;
            enemy.isEnemyTurn = true;
            if (storedCharges > maxStored - chargeRegen)
            {
                storedCharges = maxStored;
            }
            else
            {
                storedCharges += chargeRegen;
            }
        }
    }
    /// <summary>
    /// Rolls for critical strike, if crit, times base damage by 2
    /// </summary>
    void CritStrike()
    {
        int roll = Random.Range(1, 21) + playerLevel;
        if (passiveLuck)
        {
            roll += 4;
        }
        if (swordFire)
        {
            roll += 4;
        }
        if (roll >= 20)
        {
            didCrit = true;
        }
        else
        {
            didCrit = false;
        }
        if (didCrit)
        {
            baseDamage *= 2;
        }
    }

    void DamageEnemy()
    {
        totalDamage = baseDamage + elementDamage;
        enemy.health -= totalDamage;
    }

    void PoisonStack()
    {
        enemy.poisonStacks++;
        Debug.Log("The enemy has " + enemy.poisonStacks + " stacks of poison.");
    }

    public void XPBar()
    {
        xPBarFill.fillAmount = xP / requiredXP;

        if (xP >= requiredXP)
        {
            xPBarFill.color = new Color(1, 0.9104f, 0.1179f, 1);
        }
        else
        {
            xPBarFill.color = new Color(0, 0.0960f, 0.7830f, 1);
        }
    }

    void OnScreenText()
    {
        if (xP >= requiredXP)
        {
            levelUpText.text = "Level Up!";
        }
        else
        {
            levelUpText.text = "";
        }

        chargeCounter.text = "Available Charges: " + storedCharges;
        chargedCounter.text = "Charged: " + chargedCharges;

        if (passivePoints > 0) // shows when perk is available
        {
            perkAvailable.text = "Perk Point Available";
        }
        else
        {
            perkAvailable.text = "";
        }

        if (passiveQuick)
        {
            quick.text = "Quick";
        }
        else
        {
            quick.text = "";
        }

        if (passiveNormal)
        {
            normal.text = "Normal";
        }
        else
        {
            normal.text = "";
        }

        if (passiveCharges)
        {
            charges.text = "Charges";
        }
        else
        {
            charges.text = "";
        }

        if (passiveLuck)
        {
            lucky.text = "Lucky";
        }
        else
        {
            lucky.text = "";
        }

        playerLvlText.text = "Lv" + (playerLevel + 1);
        enemyLvlText.text = "Lv" + enemy.enemyLevel;
    }
    /// <summary>
    /// Allows sword/fire to heal on critical strikes
    /// </summary>
    void SwordHeal()
    {
        if (didCrit && swordFire)
        {
            health += 5 * (playerLevel + 1);
            Debug.Log("Your critical strikes fuels your bloodlust! You heal for <color=green> " + (5 * (playerLevel + 1)) + "</color> HP.");
        }

        healthBarManager.UpdatePlayerHealthBar(health, maxHealth);
    }
}
