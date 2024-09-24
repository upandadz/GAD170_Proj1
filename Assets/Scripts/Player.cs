using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Enemies enemy;

    public bool isPlayerTurn = false;
    public bool swordFire = false;
    public bool daggerPoison = false;
    public bool hammerHoly = false;

    public int playerLevel = 0;
    public int playerHealth = 100;
    public int maxHealth = 100;
    public int playerXP = 0;
    public int requiredXP = 100;

    private int weaponChoice = 1;
    private int passivePoints = 0;

    private int baseDamage;
    private int elementDamage;
    private int totalDamage;

    private int storedCharges = 2;
    private int maxStored = 6;
    private int chargedCharges = 0;
    private int maxCharged = 4;
    private int chargeRegen = 2;

    private bool didCrit = false;
    private bool hasAttacked = false;
    private bool canAttack = false;

    private bool passiveQuick = false;
    private bool passiveNormal = false;
    private bool passiveCharges = false;
    private bool passiveLuck = false;



    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("You awaken in a damp dim lit cave, unsure how you got there.");
        Debug.Log("Before you on the ground lay 3 weapons, a sword, a set of daggers, and a warhammer.");
        Debug.Log("Press 1 to grab the sword, 2 to grab the daggers, 3 to grab the warhammer.");
    }

    // Update is called once per frame
    void Update()
    {
        
        PickWeapons();
        ShowControls();

        // if you have selected a weapon type and an enemy is dead, spawn a mob & roll for first turn
        if ((swordFire || daggerPoison || hammerHoly) && enemy.isEnemyDead && Input.GetKeyDown(KeyCode.Space))
        {
            enemy.RandomiseStats();
            FirstTurnRoll();
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

    void FirstTurnRoll() // rolls for first turn, taking into account luck passive effect
    {
        int roll = Random.Range(1, 21);
        if (passiveLuck == true)
        {
            roll += 4;
        }
        if (roll > 10)
        {
            isPlayerTurn = true;
        }
        else
        {
            enemy.isEnemyTurn = true;
            isPlayerTurn = false;
        }
        canAttack = true;
    }

    void ShowControls()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("In combat, press A to charge up.");
            Debug.Log("1 charge performs a quick attack, 2 a normal attack, 4 charges a special attack.");
            Debug.Log("A player may only have 6 charges at any given time, a player recieves 2 charges per round.");
            Debug.Log("Once charged up, press ENTER to attack, once you wish to end your turn press SPACEBAR");
        }
    }
    void SwordAttacks() // Attack list for sword type
    {
        if (swordFire == true)
        {
            ChargeAttack();
            Attacks(5, 4, 10, 10, 25, 20);
        }
    }

    void DaggersAttacks() // Attack list for daggers type
    {
        if (daggerPoison == true)
        {
            ChargeAttack();
        }
    }

    void HammerAttacks() // Attack list for Hammer type
    {
        if (hammerHoly == true)
        {
            ChargeAttack();
            Attacks(4, 4, 8, 10, 30, 30);
        }
    }

    void ChargeAttack()
    {
        if ((chargedCharges == 2 && storedCharges < 2) || storedCharges == 0 || chargedCharges == 4) // stops player wasting charges if they don't have enough to charges for a special attack or have enough charges in general
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

    void Attacks(int quickBase, int quickElement, int normalBase, int normalElement, int specialBase, int specialElement)
    {
        if (isPlayerTurn && enemy.isEnemyDead == false && canAttack)
        {
            if (Input.GetKeyDown(KeyCode.Return) && chargedCharges == 1) // Quick Attack
            {
                chargedCharges -= 1;
                baseDamage = quickBase;
                elementDamage = quickElement;
                DamageModifier();
                CritStrike();
                if (didCrit)
                {
                    baseDamage *= 2;
                }
                totalDamage = baseDamage + elementDamage;
                enemy.enemyHP -= totalDamage;
                Debug.Log("You quickly strike for " + totalDamage + " damage.");
                if (didCrit)
                {
                    Debug.LogError("You critically strike!");
                }
                Debug.Log("They have <color=red>" + enemy.enemyHP + "</color> health remaining.");
                QuickPassive();

            }
            if (Input.GetKeyDown(KeyCode.Return) && chargedCharges == 2) // Normal Attack
            {
                chargedCharges -= 2;
                baseDamage = normalBase;
                elementDamage = normalElement;
                DamageModifier();
                CritStrike();
                if (didCrit)
                {
                    baseDamage *= 2;
                }
                totalDamage = baseDamage + elementDamage;
                enemy.enemyHP -= totalDamage;
                Debug.Log("You swing at them for " + totalDamage + " damage.");
                if (didCrit)
                {
                    Debug.LogError("You critically strike!");
                }
                Debug.Log("They have <color=red>" + enemy.enemyHP + "</color> health remaining.");
                QuickPassive();
            }
            if (Input.GetKeyDown(KeyCode.Return) && chargedCharges == 4) // Special Attack
            {
                chargedCharges -= 4;
                baseDamage = specialBase;
                elementDamage = specialElement;
                DamageModifier();
                CritStrike();
                if (didCrit)
                {
                    baseDamage *= 2;
                }
                totalDamage = baseDamage + elementDamage;
                enemy.enemyHP -= totalDamage;
                Debug.Log("You lash out with everything for " + totalDamage + " damage.");
                if (didCrit)
                {
                    Debug.LogError("You critically strike!");
                }
                Debug.Log("They have <color=red>" + enemy.enemyHP + "</color> health remaining.");
                QuickPassive();
            }
        }
    }
    /// <summary>
    /// changes damage based on level AND modifies element damage based on what enemy you're fighting
    /// </summary>
    void DamageModifier() 
    {
        baseDamage = Mathf.CeilToInt(1.25f * (float)baseDamage);
        elementDamage = Mathf.CeilToInt(1.25f * (float)elementDamage);

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
    void LevelUp()
    {
        if (playerXP >= requiredXP && Input.GetKeyDown(KeyCode.L))
        {
            playerXP -= requiredXP;
            playerLevel++;
            Debug.Log("You are now level <color=yellow> " + (playerLevel + 1) + "</color>.");
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
    void CritStrike()
    {
        int roll = Random.Range(1, 21) + playerLevel;
        if (passiveLuck)
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
    }
}
