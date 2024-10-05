using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player player;
    public Enemies enemy;
    public GameObject restartGameButton;
   
    // Start is called before the first frame update
    void Start()
    {
        restartGameButton.SetActive(false);
        Debug.Log("...You awaken in a damp dim lit cave, covered in scars & bruises, unsure of who you are or how you got there.");
        Debug.Log("Before you on the ground lay 3 weapons, a <color=red>sword</color>, a set of <color=green>daggers</color>, and a <color=yellow>warhammer</color>.");
        Debug.Log("Press 1 to grab the sword, 2 to grab the daggers, 3 to grab the warhammer.");

    }

    // Update is called once per frame
    void Update()
    {
        ShowControls();
        OnPlayerDeath();
    }
    void ShowControls() // Debug logs that show controlls/Perk info
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("In combat, press <color=red>A</color> to charge up.");
            Debug.Log("1 charge performs a quick attack, 2 a normal attack, 4 charges a special attack.");
            Debug.Log("A player may only have 6 charges at any given time, a player recieves 2 charges per round.");
            Debug.Log("Once charged up, press <color=red>ENTER</color> to attack, once you wish to end your turn or search for another enemy press <color=red>SPACEBAR</color>.");
            Debug.Log("After an enemy has been defeated, press an arrow key to go for a wonder. See what you find!");
            Debug.Log("Press <color=yellow>P</color> to see what the different perks do.");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("The Quick perk allows you to attack as many times as you like before you end your turn.");
            Debug.Log("The Normal perk increases your normal attack damage by 1.5x.");
            Debug.Log("The Charges perk gives you 3 charges per round instead of 2.");
            Debug.Log("The Lucky perk increases your chance to crit & chance to attack first.");
        }
    }

    public void FirstTurnRoll() // rolls for first turn, taking into account luck passive effect
    {
        int roll = Random.Range(1, 21);
        if (player.passiveLuck == true)
        {
            roll += 4;
        }
        if (roll > 10)
        {
            player.isPlayerTurn = true;
        }
        else
        {
            enemy.isEnemyTurn = true;
            player.isPlayerTurn = false;
        }
        player.canAttack = true;
    }

    void OnPlayerDeath()
    {
        if (player.health <= 0 && player.isAlive)
        {
            player.isAlive = false;
            if (enemy.isBandit)
            {
                Debug.Log("The bandit plunges their blade through your stomach, you feel yourself beggining to fade...");
            }
            else if (enemy.isUndead)
            {
                Debug.Log("The undead's jaws clamp down onto your neck, you feel yourself getting cold...");
            }
            else if (enemy.isWolf)
            {
                Debug.Log("");
            }
            else
            {
                Debug.Log("You have been slain.");
            }
            restartGameButton.SetActive(true);
        }
    }

    public void StartOver()
    {
        SceneManager.LoadScene("StartGame");
    }
}
