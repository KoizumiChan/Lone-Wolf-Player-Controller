using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UserInterface : MonoBehaviour
{
    public TextMeshProUGUI stateText;

    public float stamina;
    public float maxStamina = 100;
    public PlayerInput PlayerInput;
    public Image staminaBar;

    public float health;
    public float maxHealth = 100;
    public Image healthBar;
    
    public float hunger;
    public float maxHunger = 100;
    public Image hungerBar;

    public float thirst;
    public float maxThirst = 100;
    public Image thirstBar;
    private void Start() 
    {
        PlayerInput.OnPlayerStateChange += UpdateStateText;
    }

    private void OnDestroy() 
    {
        PlayerInput.OnPlayerStateChange -= UpdateStateText;
    }
    private void UpdateStateText(PlayerInput.MovementState state) 
    {
        if(state == PlayerInput.MovementState.sneaking) 
        {
            stateText.text = "Sneaking...";
        }
        else 
        {
            stateText.text = "";  // Clear the text when not sneaking.
        }
    }
    void Update()
    {
        stamina = PlayerInput.stamina;
        staminaBar.fillAmount = stamina / maxStamina;   

        health = PlayerInput.health;
        healthBar.fillAmount = health / maxHealth;   

        hunger = PlayerInput.hunger;
        hungerBar.fillAmount = hunger / maxHunger;   

        thirst = PlayerInput.thirst;
        thirstBar.fillAmount = thirst / maxThirst;   
    }
}
