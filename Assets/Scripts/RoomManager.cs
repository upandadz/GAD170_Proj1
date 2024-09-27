using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomManager : MonoBehaviour
{
    public Enemies enemy;
    public Player player;
    public HealthBarManager healthBarManager;
    public TMP_Text ventureOn;

    public bool canWalk = false;

    private int walkRoll;
    private int dropRoll;

    void Update()
    {
        GoForWalk();
    }

    void GoForWalk()
    {

        if (canWalk && enemy.isEnemyDead)
        {
            ventureOn.text = "Venture on!";
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                canWalk = false;
                Debug.Log("You make a left turn.");
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                canWalk = false;
                Debug.Log("You continue forward.");
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                canWalk = false;
                Debug.Log("You make a right turn.");
            }
            if (!canWalk) // after you have "walked a direction" it will execute this
            {
                walkRoll = Random.Range(1, 4);
                if (walkRoll == 1)
                {
                    Debug.Log("You squeeze through an opening and find yourself in a mouldy dank cavern."); // cave
                }
                if (walkRoll == 2)
                {
                    Debug.Log("The path opens up to an ecosystem of unfamiliar shrubbery & moss covered trees."); // underground forest
                }
                if (walkRoll == 3)
                {
                    Debug.Log("As you continue on you come across what seems to be an abandoned campsite."); // abandoned campsite
                }

                dropRoll = Random.Range(1, 4);
                if (dropRoll == 1) // XP drop
                {
                    Debug.Log("During your travels you gained confidence in your abilities.");
                    player.xP += 15 * player.playerLevel;
                    player.XPBar();
                }
                if (dropRoll == 2) // HP drop
                {
                    Debug.Log("A long walk is just what you needed, you feel rejuvinated.");
                    player.health += Mathf.CeilToInt(0.3f * player.maxHealth);
                    healthBarManager.UpdatePlayerHealthBar(player.health, player.maxHealth);
                }
                if (dropRoll == 3) // Charges drop
                {
                    Debug.Log("You feel hungry & ready to face anything.");
                    player.storedCharges += player.chargeRegen;
                }
            }
        }
        else
        {
            ventureOn.text = "";
        }
    }
}
