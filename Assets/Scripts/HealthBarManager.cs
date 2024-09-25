using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{

    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image enemyHealthBarFill;
    public void UpdatePlayerHealthBar(float health, float maxHealth)
    {
        healthBarFill.fillAmount = health / maxHealth;
    }

    public void UpdateEnemyHealthBar(float health, float maxHealth)
    {
        enemyHealthBarFill.fillAmount = health / maxHealth;
    }
}
