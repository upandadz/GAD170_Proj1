using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class OnScreenText : MonoBehaviour
{
    public Player player;
    public Enemies enemy;
    public TMP_Text poisonCounter;
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
    public TMP_Text attack;
    public TMP_Text endTurn;
    void Update()
    {
        ShowText();
    }

    void ShowText()
    {
        if (player.xP >= player.requiredXP)
        {
            levelUpText.text = "Level Up!";
        }
        else
        {
            levelUpText.text = "";
        }

        chargeCounter.text = "Available Charges: " + player.storedCharges;
        chargedCounter.text = "Charged: " + player.chargedCharges;

        if (player.passivePoints > 0) // shows when perk is available
        {
            perkAvailable.text = "Perk Point Available";
        }
        else
        {
            perkAvailable.text = "";
        }

        #region shows current player perks 
        if (player.passiveQuick)
        {
            quick.text = "Quick";
        }
        else
        {
            quick.text = "";
        }

        if (player.passiveNormal)
        {
            normal.text = "Normal";
        }
        else
        {
            normal.text = "";
        }

        if (player.passiveCharges)
        {
            charges.text = "Charges";
        }
        else
        {
            charges.text = "";
        }

        if (player.passiveLuck)
        {
            lucky.text = "Lucky";
        }
        else
        {
            lucky.text = "";
        }
        #endregion

        if (player.hasAttacked && player.isPlayerTurn && !enemy.isEnemyDead)
        {
            endTurn.text = "Space: End Turn";
        }
        else
        {
            endTurn.text = "";
        }

        if (player.chargedCharges > 0)
        {
            attack.text = "Enter: Attack";
        }
        else
        {
            attack.text = "";
        }

        if (player.daggerPoison)
        {
            poisonCounter.text = "Poison Stacks: " + enemy.poisonStacks;
        }

        playerLvlText.text = "Lv" + (player.playerLevel + 1);
        enemyLvlText.text = "Lv" + enemy.enemyLevel;
    }
}
